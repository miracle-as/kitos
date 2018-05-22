﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItSystemTypesController : LocalOptionBaseController<LocalItSystemType, ItSystem, ItSystemType, LocalItSystemTypeDTO, ItSystemDTO, OptionDTO>
    {
        public LocalItSystemTypesController(IGenericRepository<LocalItSystemType> repository, IAuthenticationService authService, IGenericRepository<ItSystemType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
