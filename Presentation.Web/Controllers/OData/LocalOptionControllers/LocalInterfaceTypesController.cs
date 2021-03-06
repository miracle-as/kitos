﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalInterfaceTypesController : LocalOptionBaseController<LocalInterfaceType, ItInterface, InterfaceType>
    {
        public LocalInterfaceTypesController(IGenericRepository<LocalInterfaceType> repository, IAuthenticationService authService, IGenericRepository<InterfaceType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
