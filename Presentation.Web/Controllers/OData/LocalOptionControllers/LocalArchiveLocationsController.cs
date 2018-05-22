using System;
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

    public class LocalArchiveLocationsController : LocalOptionBaseController<LocalArchiveLocation, ItSystemUsage, ArchiveLocation, LocalArchiveLocationDTO, ItSystemUsageDTO, ArchiveLocationDTO>
    {
        public LocalArchiveLocationsController(IGenericRepository<LocalArchiveLocation> repository, IAuthenticationService authService, IGenericRepository<ArchiveLocation> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}