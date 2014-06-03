﻿using System;
using System.Linq;
using System.Net.Http;
using System.Security;
using Core.DomainModel;
using Core.DomainServices;
using Newtonsoft.Json.Linq;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class TaskUsageController : GenericApiController<TaskUsage, TaskUsageDTO>
    {
        private readonly IOrgUnitService _orgUnitService;

        public TaskUsageController(IGenericRepository<TaskUsage> repository,IOrgUnitService orgUnitService) 
            : base(repository)
        {
            _orgUnitService = orgUnitService;
        }

        public HttpResponseMessage Get(int orgUnitId)
        {
            return Get(orgUnitId, false);
        }

        public HttpResponseMessage Get(int orgUnitId, bool onlyStarred)
        {
            try
            {
                var usages = Repository.Get(u => u.OrgUnitId == orgUnitId);

                if (onlyStarred) usages = usages.Where(u => u.Starred);

                var delegationDtos = usages.Select(CompileDelegation);

                return Ok(delegationDtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        //Given a task usage, compile the task delegation down the org unit tree
        private TaskDelegationDTO CompileDelegation(TaskUsage usage)
        {
            if (usage == null) throw new ArgumentNullException();

            var delegations = usage.OrgUnit.Children.Select(child => CompileDelegation(child, usage.TaskRef))
                                   .Where(childDelegation => childDelegation != null).ToList();

            var delegation = new TaskDelegationDTO()
            {
                Usage = Map(usage),
                Delegations = delegations,
                HasDelegations = delegations.Any()
            };

            return delegation;
        }

        //Given a unit and a task, compile the task delegation down the org unit tree
        private TaskDelegationDTO CompileDelegation(OrganizationUnit unit, TaskRef task)
        {
            var unitId = unit.Id;
            var taskId = task.Id;

            var usage = Repository.Get(u => u.OrgUnitId == unitId && u.TaskRefId == taskId).FirstOrDefault();
            if (usage == null) return null;

            var delegations = unit.Children.Select(child => CompileDelegation(child, task))
                                  .Where(childDelegation => childDelegation != null).ToList();

            var delegation = new TaskDelegationDTO()
                {
                    Usage = Map(usage),
                    Delegations = delegations,
                    HasDelegations = delegations.Any()
                };

            return delegation;
        }

        private void DeleteTaskOnChildren(OrganizationUnit orgUnit, int taskRefId)
        {
            foreach (var unit in orgUnit.Children)
            {
                var temp = unit;
                var usages = Repository.Get(u => u.TaskRefId == taskRefId && u.OrgUnitId == temp.Id);

                foreach (var taskUsage in usages)
                {
                    Repository.DeleteByKey(taskUsage.Id);
                }

                DeleteTaskOnChildren(unit, taskRefId);
            }
        }

        protected override void DeleteQuery(int id)
        {
            var entity = Repository.GetByKey(id);

            var taskRefId = entity.TaskRefId;
            var unit = entity.OrgUnit;

            Repository.DeleteByKey(entity.Id);
            DeleteTaskOnChildren(unit, taskRefId);

            Repository.Save();
        }
        
        public override HttpResponseMessage Put(int id, JObject jObject)
        {
            return NotAllowed();
        }
    }
}
