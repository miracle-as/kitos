﻿using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalPriceRegulationTypesController : LocalOptionBaseController<LocalPriceRegulationType, ItContract, PriceRegulationType>
    {
        public LocalPriceRegulationTypesController(IGenericRepository<LocalPriceRegulationType> repository, IAuthenticationService authService, IGenericRepository<PriceRegulationType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
