﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainModel.Organization;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class ItSystemUsageController : GenericContextAwareApiController<ItSystemUsage, ItSystemUsageDTO>
    {
        private readonly IGenericRepository<OrganizationUnit> _orgUnitRepository;
        private readonly IGenericRepository<TaskRef> _taskRepository;
        private readonly IItSystemUsageService _itSystemUsageService;
        private readonly IGenericRepository<ItSystemRole> _roleRepository;

        public ItSystemUsageController(IGenericRepository<ItSystemUsage> repository,
            IGenericRepository<OrganizationUnit> orgUnitRepository,
            IGenericRepository<TaskRef> taskRepository,
            IItSystemUsageService itSystemUsageService,
            IGenericRepository<ItSystemRole> roleRepository)
            : base(repository)
        {
            _orgUnitRepository = orgUnitRepository;
            _taskRepository = taskRepository;
            _itSystemUsageService = itSystemUsageService;
            _roleRepository = roleRepository;
        }

        public HttpResponseMessage GetSearchByOrganization(int organizationId, string q)
        {
            try
            {
                var usages = Repository.Get(
                    u =>
                        // filter by system usage name
                        u.ItSystem.Name.Contains(q) &&
                        // system usage is only within the context
                        u.OrganizationId == organizationId
                    );

                return Ok(Map(usages));
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage GetByOrganization(int organizationId, [FromUri] PagingModel<ItSystemUsage> pagingModel, [FromUri] string q, bool? overview)
        {
            try
            {
                pagingModel.Where(
                    u =>
                        // system usage is only within the context
                        u.OrganizationId == organizationId
                    );

                if (!string.IsNullOrEmpty(q)) pagingModel.Where(usage => usage.ItSystem.Name.Contains(q));

                var usages = Page(Repository.AsQueryable(), pagingModel);

                return Ok(Map(usages));
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage GetExcel(bool? csv, int organizationId)
        {
            try
            {
                var usages = Repository.Get(
                    u =>
                        // system usage is only within the context
                        u.OrganizationId == organizationId
                    );

                //if (!string.IsNullOrEmpty(q)) pagingModel.Where(usage => usage.ItSystem.Name.Contains(q));
                //var usages = Page(Repository.AsQueryable(), pagingModel);

                // mapping to DTOs for easy lazy loading of needed properties
                var dtos = Map(usages);

                var roles = _roleRepository.Get().ToList();

                var list = new List<dynamic>();
                var header = new ExpandoObject() as IDictionary<string, Object>;
                header.Add("Aktiv", "Aktiv");
                header.Add("IT System", "IT System");
                header.Add("OrgUnit", "Ansv. organisationsenhed");
                foreach (var role in roles)
                    header.Add(role.Name, role.Name);
                header.Add("AppType", "Applikationtype");
                header.Add("BusiType", "Forretningstype");
                header.Add("Anvender", "Anvender");
                header.Add("Udstiller", "Udstiller");
                header.Add("Overblik", "Overblik");
                list.Add(header);
                foreach (var usage in dtos)
                {
                    var obj = new ExpandoObject() as IDictionary<string, Object>;
                    obj.Add("Aktiv", usage.MainContractIsActive);
                    obj.Add("IT System", usage.ItSystem.Name);
                    obj.Add("OrgUnit", usage.ResponsibleOrgUnitName);
                    foreach (var role in roles)
                    {
                        var roleId = role.Id;
                        obj.Add(role.Name,
                                String.Join(",", usage.Rights.Where(x => x.RoleId == roleId).Select(x => x.User.FullName)));
                    }
                    obj.Add("AppType", usage.ItSystem.AppTypeOptionName);
                    obj.Add("BusiType", usage.ItSystem.BusinessTypeName);
                    obj.Add("Anvender", usage.ActiveInterfaceUseCount + "(" + usage.InterfaceUseCount+ ")");
                    obj.Add("Udstiller", usage.InterfaceExhibitCount);
                    obj.Add("Overblik", usage.OverviewItSystemName);
                    list.Add(obj);
                }

                var s = list.ToCsv();
                var bytes = Encoding.Unicode.GetBytes(s);
                var stream = new MemoryStream();
                stream.Write(bytes, 0, bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileNameStar = "itsystemanvendelsesoversigt.csv", DispositionType = "ISO-8859-1" };
                return result;
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage GetByItSystemAndOrganization(int itSystemId, int organizationId)
        {
            try
            {
                var usage = Repository.Get(u => u.ItSystemId == itSystemId && u.OrganizationId == organizationId).FirstOrDefault();

                return usage == null ? NotFound() : Ok(Map(usage));
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public override HttpResponseMessage Post(ItSystemUsageDTO dto)
        {
            try
            {
                if (Repository.Get(usage => usage.ItSystemId == dto.ItSystemId
                                            && usage.OrganizationId == dto.OrganizationId).Any())
                    return Conflict("Usage already exist");

                var sysUsage = _itSystemUsageService.Add(dto.ItSystemId, dto.OrganizationId, KitosUser);

                return Created(Map(sysUsage), new Uri(Request.RequestUri + "?itSystemId=" + dto.ItSystemId + "&organizationId" + dto.OrganizationId));

            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage DeleteByItSystemId(int itSystemId, int organizationId)
        {
            try
            {
                var usage = Repository.Get(u => u.ItSystemId == itSystemId && u.OrganizationId == organizationId).FirstOrDefault();
                if (usage == null) return NotFound();

                //This will make sure we check for permissions and such...
                return base.Delete(usage.Id, organizationId);

            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage PostOrganizationUnitsUsingThisSystem(int id, [FromUri] int organizationUnit, int organizationId)
        {
            try
            {
                var usage = Repository.GetByKey(id);
                if (usage == null) return NotFound();
                if (!HasWriteAccess(usage, organizationId)) return Unauthorized();

                var orgUnit = _orgUnitRepository.GetByKey(organizationUnit);
                if (orgUnit == null) return NotFound();


                usage.UsedBy.Add(new ItSystemUsageOrgUnitUsage { ItSystemUsageId = id, OrganizationUnitId = organizationUnit });

                usage.LastChanged = DateTime.UtcNow;
                usage.LastChangedByUser = KitosUser;

                Repository.Save();

                return Created(Map<OrganizationUnit, OrgUnitDTO>(orgUnit));
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage DeleteOrganizationUnitsUsingThisSystem(int id, [FromUri] int organizationUnit, int organizationId)
        {
            try
            {
                var usage = Repository.GetByKey(id);
                if (usage == null) return NotFound();

                if (!HasWriteAccess(usage, organizationId)) return Unauthorized();

                var orgUnit = _orgUnitRepository.GetByKey(organizationUnit);
                if (orgUnit == null) return NotFound();

                var entity = usage.UsedBy.SingleOrDefault(x => x.ItSystemUsageId == id && x.OrganizationUnitId == organizationUnit);
                if (entity == null) return NotFound();

                usage.UsedBy.Remove(entity);

                usage.LastChanged = DateTime.UtcNow;
                usage.LastChangedByUser = KitosUser;

                Repository.Save();

                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage PostTasksUsedByThisSystem(int id, int organizationId, [FromUri] int? taskId)
        {
            try
            {
                var usage = Repository.GetByKey(id);
                if (usage == null) return NotFound();
                if (!HasWriteAccess(usage, organizationId)) return Unauthorized();

                List<TaskRef> tasks;
                if (taskId.HasValue)
                {
                    // get child leaves of taskId that havn't got a usage in the system
                    tasks = _taskRepository.Get(
                        x =>
                            (x.Id == taskId || x.ParentId == taskId || x.Parent.ParentId == taskId) && !x.Children.Any() &&
                            x.AccessModifier == AccessModifier.Public &&
                            x.ItSystemUsages.All(y => y.Id != id)).ToList();
                }
                else
                {
                    // no taskId was specified so get everything
                    tasks = _taskRepository.Get(
                        x =>
                            !x.Children.Any() &&
                            x.AccessModifier == AccessModifier.Public &&
                            x.ItSystemUsages.All(y => y.Id != id)).ToList();
                }

                if (!tasks.Any())
                    return NotFound();

                foreach (var task in tasks)
                {
                    usage.TaskRefs.Add(task);
                }
                usage.LastChanged = DateTime.UtcNow;
                usage.LastChangedByUser = KitosUser;
                Repository.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage DeleteTasksUsedByThisSystem(int id, int organizationId, [FromUri] int? taskId)
        {
            try
            {
                var usage = Repository.GetByKey(id);
                if (usage == null) return NotFound();
                if (!HasWriteAccess(usage, organizationId)) return Unauthorized();

                List<TaskRef> tasks;
                if (taskId.HasValue)
                {
                    tasks =
                        usage.TaskRefs.Where(
                            x =>
                                (x.Id == taskId || x.ParentId == taskId || x.Parent.ParentId == taskId) &&
                                !x.Children.Any())
                            .ToList();
                }
                else
                {
                    // no taskId was specified so get everything
                    tasks = usage.TaskRefs.ToList();
                }

                if (!tasks.Any())
                    return NotFound();

                foreach (var task in tasks)
                {
                    usage.TaskRefs.Remove(task);
                }
                usage.LastChanged = DateTime.UtcNow;
                usage.LastChangedByUser = KitosUser;
                Repository.Save();
                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        /// <summary>
        /// Returns a list of task ref and whether or not they are currently associated with a given IT system usage
        /// </summary>
        /// <param name="id">ID of the IT system usage</param>
        /// <param name="tasks">Routing qualifer</param>
        /// <param name="onlySelected">If true, only return those task ref that are associated with the I system usage. If false, return all task ref.</param>
        /// <param name="taskGroup">Optional filtering on task group</param>
        /// <param name="pagingModel">Paging model</param>
        /// <returns>List of TaskRefSelectedDTO</returns>
        public HttpResponseMessage GetTasks(int id, bool? tasks, bool onlySelected, int? taskGroup, [FromUri] PagingModel<TaskRef> pagingModel)
        {
            try
            {
                var usage = Repository.GetByKey(id);

                IQueryable<TaskRef> taskQuery;
                if (onlySelected)
                {
                    var taskQuery1 = Repository.AsQueryable().Where(p => p.Id == id).SelectMany(p => p.TaskRefs);
                    var taskQuery2 =
                        Repository.AsQueryable()
                                  .Where(p => p.Id == id)
                                  .Select(p => p.ItSystem)
                                  .SelectMany(s => s.TaskRefs);

                    taskQuery = taskQuery1.Union(taskQuery2);
                }
                else
                {
                    taskQuery = _taskRepository.AsQueryable();
                }

                //if a task group is given, only find the tasks in that group
                if (taskGroup.HasValue)
                    pagingModel.Where(taskRef => (taskRef.ParentId.Value == taskGroup.Value ||
                                                  taskRef.Parent.ParentId.Value == taskGroup.Value) &&
                                                 !taskRef.Children.Any() &&
                                                 taskRef.AccessModifier == AccessModifier.Public); // TODO add support for normal
                else
                    pagingModel.Where(taskRef => taskRef.Children.Count == 0);

                var theTasks = Page(taskQuery, pagingModel).ToList();

                var dtos = theTasks.Select(task => new TaskRefSelectedDTO()
                {
                    TaskRef = Map<TaskRef, TaskRefDTO>(task),
                    IsSelected = onlySelected || usage.TaskRefs.Any(t => t.Id == task.Id),
                    IsLocked = usage.ItSystem.TaskRefs.Any(t => t.Id == task.Id)
                }).ToList(); // must call .ToList here else the output will be wrapped in $type,$values

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        protected override void DeleteQuery(ItSystemUsage entity)
        {
            _itSystemUsageService.Delete(entity.Id);
        }
    }
}
