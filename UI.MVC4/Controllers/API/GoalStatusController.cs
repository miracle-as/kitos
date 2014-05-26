﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel.ItProject;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class GoalStatusController : GenericApiController<GoalStatus, int, GoalStatusDTO>
    {
        public GoalStatusController(IGenericRepository<GoalStatus> repository) : base(repository)
        {
        }
    }
}