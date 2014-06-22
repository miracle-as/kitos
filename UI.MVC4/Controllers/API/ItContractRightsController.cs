﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel.ItContract;
using Core.DomainServices;

namespace UI.MVC4.Controllers.API
{
    public class ItContractRightsController : GenericRightsController<ItContract, ItContractRight, ItContractRole>
    {
        public ItContractRightsController(IGenericRepository<ItContractRight> rightRepository, IGenericRepository<ItContract> objectRepository) : base(rightRepository, objectRepository)
        {
        }
    }
}