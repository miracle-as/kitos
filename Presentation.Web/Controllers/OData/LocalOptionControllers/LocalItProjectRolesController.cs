﻿using Core.ApplicationServices;
using Core.DomainModel.ItProject;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItProjectRolesController : LocalOptionBaseController<LocalItProjectRole, ItProjectRight, ItProjectRole>
    {
        public LocalItProjectRolesController(IGenericRepository<LocalItProjectRole> repository, IAuthenticationService authService, IGenericRepository<ItProjectRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
