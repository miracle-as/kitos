﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class LocalRegularPersonalDataTypesController : LocalOptionBaseController<LocalRegularPersonalDataType, ItSystem, RegularPersonalDataType>
    {
        public LocalRegularPersonalDataTypesController(IGenericRepository<LocalRegularPersonalDataType> repository, IAuthenticationService authService, IGenericRepository<RegularPersonalDataType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}