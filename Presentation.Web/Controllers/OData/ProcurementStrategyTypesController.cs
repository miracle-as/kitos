﻿using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainServices;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Presentation.Web.Controllers.OData
{
    public class ProcurementStrategyTypesController : BaseEntityController<ProcurementStrategyType>
    {
        public ProcurementStrategyTypesController(IGenericRepository<ProcurementStrategyType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}