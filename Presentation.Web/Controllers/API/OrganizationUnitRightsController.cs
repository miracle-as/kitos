﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Core.DomainModel;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class OrganizationUnitRightsController : GenericRightsController<OrganizationUnit, OrganizationRight, OrganizationRole>
    {
        private readonly IOrgUnitService _orgUnitService;

        public OrganizationUnitRightsController(IGenericRepository<OrganizationRight> rightRepository,
            IGenericRepository<OrganizationUnit> objectRepository, IOrgUnitService orgUnitService)
            : base(rightRepository, objectRepository)
        {
            _orgUnitService = orgUnitService;
        }

        /// <summary>
        /// Returns all colllecteds rights for an organization unit and all sub units
        /// </summary>
        /// <param name="id">Id of the unit</param>
        /// <returns>List of rights</returns>
        public override HttpResponseMessage GetRights(int id)
        {
            try
            {
                var theRights = GetOrganizationRights(id);

                var dtos = AutoMapper.Mapper.Map<ICollection<OrganizationRight>, ICollection<RightOutputDTO>>(theRights);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        /// <summary>
        /// Returns all colllecteds rights for an organization unit and all sub units for a specific user
        /// </summary>
        /// <param name="orgId">Id of the unit</param>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of rights</returns>
        public HttpResponseMessage GetRightsForUser(int orgId, int userId)
        {
            try
            {
                var theRights = GetOrganizationRights(orgId).Where(r => r.UserId == userId).ToList();

                var dtos = AutoMapper.Mapper.Map<ICollection<OrganizationRight>, ICollection<RightOutputDTO>>(theRights);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        private List<OrganizationRight> GetOrganizationRights(int id)
        {
            var orgUnits = _orgUnitService.GetSubTree(id);

            var theRights = new List<OrganizationRight>();
            foreach (var orgUnit in orgUnits)
            {
                theRights.AddRange(GetRightsQuery(orgUnit.Id));
            }
            return theRights;
        }
    }
}