﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AutoMapper;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class ItSystemUsageOrgUnitUsageController : BaseApiController
    {
        private readonly IGenericRepository<ItSystemUsageOrgUnitUsage> _responsibleOrgUnitRepository;
        private readonly IGenericRepository<ItSystemUsage> _systemUsageRepository;

        public ItSystemUsageOrgUnitUsageController(IGenericRepository<ItSystemUsageOrgUnitUsage> responsibleOrgUnitRepository, IGenericRepository<ItSystemUsage> systemUsageRepository)
        {
            _responsibleOrgUnitRepository = responsibleOrgUnitRepository;
            _systemUsageRepository = systemUsageRepository;
        }

        public HttpResponseMessage GetOrgUnitsBySystemUsage(int id)
        {
            try
            {
                var items = _responsibleOrgUnitRepository.Get(x => x.ItSystemUsageId == id);
                var orgUnits = items.Select(x => x.OrganizationUnit);
                var dtos = Mapper.Map<IEnumerable<SimpleOrgUnitDTO>>(orgUnits);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage GetResponsibleBySystemUsage(int id, bool? responsible)
        {
            try
            {
                var systemUsage = _systemUsageRepository.GetByKey(id);

                if (systemUsage.ResponsibleUsage == null) return Ok(); // TODO should be NotFound but ui router resolve redirects to mainpage on 404

                var organizationUnit = systemUsage.ResponsibleUsage.OrganizationUnit;
                var dtos = Mapper.Map<SimpleOrgUnitDTO>(organizationUnit);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage PostSetResponsibleOrgUnit(int usageId, int orgUnitId, bool? responsible)
        {
            try
            {
                var entity = _responsibleOrgUnitRepository.GetByKey(new object[] {usageId, orgUnitId});
                var systemUsage = _systemUsageRepository.GetByKey(usageId);

                systemUsage.ResponsibleUsage = entity;

                _responsibleOrgUnitRepository.Save();

                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        public HttpResponseMessage DeleteResponsibleOrgUnit(int usageId, bool? responsible)
        {
            try
            {
                var systemUsage = _systemUsageRepository.GetByKey(usageId);
                // WARNING: force loading so setting it to null will be tracked
                var forceLoad = systemUsage.ResponsibleUsage;
                systemUsage.ResponsibleUsage = null;

                _systemUsageRepository.Save();

                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }
    }
}
