﻿using System;
using System.Net.Http;
using Core.DomainModel.ItProject;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class RiskController : GenericApiController<Risk, RiskDTO>
    {
        public RiskController(IGenericRepository<Risk> repository) : base(repository)
        {
        }

        public HttpResponseMessage GetByProject(bool? getByProject, int projectId)
        {
            try
            {
                var risks = Repository.Get(r => r.ItProjectId == projectId);

                return Ok(Map(risks));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }
    }
}
