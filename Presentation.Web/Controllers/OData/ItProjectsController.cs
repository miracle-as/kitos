﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainModel;
using Core.DomainModel.ItProject;
using Core.DomainServices;
using System.Net;
using Core.DomainModel.Organization;
using Core.ApplicationServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    [Authorize]
    public class ItProjectsController : BaseEntityController<ItProject, ItProjectDTO>
    {
        private readonly IGenericRepository<OrganizationUnit> _orgUnitRepository;
        private readonly IAuthenticationService _authService;

        public ItProjectsController(IGenericRepository<ItProject> repository, IGenericRepository<OrganizationUnit> orgUnitRepository, IAuthenticationService authService)
            : base(repository, authService)
        {
            _orgUnitRepository = orgUnitRepository;
            _authService = authService;
        }

        [EnableQuery]
        //[ODataRoute("ItProjects")]
        public override IHttpActionResult Get()
        {
            return base.Get();

            //if (AuthenticationService.HasReadAccessOutsideContext(UserId))
            //    return base.Get();

            //var orgId = CurrentOrganizationId;
            //return Ok(Repository.AsQueryable().Where(x => x.OrganizationId == orgId));
        }

        // GET /Organizations(1)/ItProjects
        [EnableQuery]
        //[ODataRoute("Organizations({key})/ItProjects")]
        public IHttpActionResult GetItProjects(int key)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (!_authService.HasReadAccessOutsideContext(UserId))
            {
                if (loggedIntoOrgId != key)
                    return StatusCode(HttpStatusCode.Forbidden);

                var result = Repository.AsQueryable().Where(m => m.OrganizationId == key);
                return Ok(result);
            }
            else
            {
                var result = Repository.AsQueryable().Where(m => m.OrganizationId == key || m.AccessModifier == AccessModifier.Public);
                return Ok(result);
            }
        }

        // GET /Organizations(1)/ItProjects(1)
        [EnableQuery]
        //[ODataRoute("Organizations({orgKey})/ItProjects({projKey})")]
        public IHttpActionResult GetItProjects(int orgKey, int projKey)
        {
            var entity = Repository.AsQueryable().SingleOrDefault(m => m.Id == projKey);
            if (entity == null)
                return NotFound();

            if (_authService.HasReadAccess(UserId, entity))
                return Ok(entity);

            return StatusCode(HttpStatusCode.Forbidden);
        }

        // TODO for now only read actions are allowed, in future write will be enabled - but keep security in mind!
        // GET /Organizations(1)/OrganizationUnits(1)/ItProjects
        [EnableQuery]
        //[ODataRoute("Organizations({orgKey})/OrganizationUnits({unitKey})/ItProjects")]
        public IHttpActionResult GetItProjectsByOrgUnit(int orgKey, int unitKey)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != orgKey && !_authService.HasReadAccessOutsideContext(UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            var projects = new List<ItProject>();

            // using iteration instead of recursion else we're running into
            // an "multiple DataReaders open" issue and MySQL doesn't support MARS

            var queue = new Queue<int>();
            queue.Enqueue(unitKey);
            while (queue.Count > 0)
            {
                var orgUnitKey = queue.Dequeue();
                var orgUnit = _orgUnitRepository.AsQueryable()
                    .Include(x => x.Children)
                    .Include(x => x.UsingItProjects.Select(y => y.ResponsibleItProject))
                    .First(x => x.OrganizationId == orgKey && x.Id == orgUnitKey);

                var responsibles = orgUnit.UsingItProjects.Select(x => x.ResponsibleItProject).Where(x => x != null);
                projects.AddRange(responsibles);

                var childIds = orgUnit.Children.Select(x => x.Id);
                foreach (var childId in childIds)
                {
                    queue.Enqueue(childId);
                }
            }

            return Ok(projects);
        }
    }
}
