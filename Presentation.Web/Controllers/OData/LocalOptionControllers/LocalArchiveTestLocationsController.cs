﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    using Core.ApplicationServices;
    using Core.DomainModel.ItSystem;
    using Core.DomainModel.ItSystemUsage;
    using Core.DomainModel.LocalOptions;
    using Core.DomainServices;
    using Models;

    public class LocalArchiveTestLocationsController : LocalOptionBaseController<LocalArchiveTestLocation, ItSystemUsage, ArchiveTestLocation, LocalArchiveTestLocationDTO, ItSystemUsageDTO, ArchiveTestLocationDTO>
    {
        public LocalArchiveTestLocationsController(IGenericRepository<LocalArchiveTestLocation> repository, IAuthenticationService authService, IGenericRepository<ArchiveTestLocation> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}