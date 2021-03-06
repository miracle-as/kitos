﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItSystemRolesController : LocalOptionBaseController<LocalItSystemRole, ItSystemRight, ItSystemRole>
    {
        public LocalItSystemRolesController(IGenericRepository<LocalItSystemRole> repository, IAuthenticationService authService, IGenericRepository<ItSystemRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
