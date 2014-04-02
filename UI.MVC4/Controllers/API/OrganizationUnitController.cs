﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using Newtonsoft.Json.Linq;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class OrganizationUnitController : GenericApiController<OrganizationUnit, int, OrgUnitDTO>
    {
        private readonly IOrgUnitService _orgUnitService;

        public OrganizationUnitController(IGenericRepository<OrganizationUnit> repository, IOrgUnitService orgUnitService) : base(repository)
        {
            _orgUnitService = orgUnitService;
        }

        public HttpResponseMessage GetByUser(int userId)
        {
            try
            {
                var user = KitosUser;

                if(user.Id != userId) throw new SecurityException();

                var orgUnits = _orgUnitService.GetByUser(user);

                return Ok(Map<ICollection<OrganizationUnit>, ICollection<OrgUnitDTO>>(orgUnits));

            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetByOrganization(int organization)
        {
            try
            {
                var orgUnit = Repository.Get(o => o.Organization_Id == organization && o.Parent == null).FirstOrDefault();

                if (orgUnit == null) return NotFound();

                var item = Map<OrganizationUnit, OrgUnitDTO>(orgUnit);

                return Ok(item);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public override HttpResponseMessage Patch(int id, Newtonsoft.Json.Linq.JObject obj)
        {
            try
            {
                JToken jtoken;
                if (obj.TryGetValue("Parent_Id", out jtoken))
                {
                    var parentId = jtoken.Value<int>();

                    //if the new parent is actually a descendant of the item, don't update - this would create a loop!
                    if (_orgUnitService.IsAncestorOf(parentId, id))
                    {
                        throw new ArgumentException("Self reference loop");
                    }
                }

            }
            catch (Exception e)
            {
                return Error(e);
            }
            
            return base.Patch(id, obj);
        }

        protected override OrganizationUnit PatchQuery(OrganizationUnit item)
        {
            //if the new parent is actually a descendant of the item, don't update - this would create a loop!
            if (item.Parent_Id.HasValue && _orgUnitService.IsAncestorOf(item.Parent_Id.Value, item.Id))
            {
                throw new ArgumentException("Self reference loop");
            }



            return base.PatchQuery(item);
        }
    }
}
