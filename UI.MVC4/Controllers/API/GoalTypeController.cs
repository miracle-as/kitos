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
    public class GoalTypeController : GenericOptionApiController<GoalType, Goal, OptionDTO>
    {
        public GoalTypeController(IGenericRepository<GoalType> repository) : base(repository)
        {
        }
    }
}