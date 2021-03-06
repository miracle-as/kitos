﻿using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItContractTemplateTypesController : LocalOptionBaseController<LocalItContractTemplateType, ItContract, ItContractTemplateType>
    {
        public LocalItContractTemplateTypesController(IGenericRepository<LocalItContractTemplateType> repository, IAuthenticationService authService, IGenericRepository<ItContractTemplateType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
