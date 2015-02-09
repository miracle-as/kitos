﻿using Core.DomainModel;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class MilestoneController : GenericApiController<Milestone, MilestoneDTO>
    {
        public MilestoneController(IGenericRepository<Milestone> repository) 
            : base(repository)
        {
        }
    }
}