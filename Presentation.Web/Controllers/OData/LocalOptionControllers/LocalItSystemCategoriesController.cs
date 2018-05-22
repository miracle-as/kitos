﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    using Core.DomainModel.LocalOptions;
    using Models;

    public class LocalItSystemCategoriesController : LocalOptionBaseController<LocalItSystemCategories, ItSystemUsage, ItSystemCategories, LocalItSystemCategoriesDTO, ItSystemUsageDTO, ItSystemCategoriesDTO>
    {
        public LocalItSystemCategoriesController(IGenericRepository<LocalItSystemCategories> repository, IAuthenticationService authService, IGenericRepository<ItSystemCategories> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}