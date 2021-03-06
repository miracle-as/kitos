﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalDataTypesController : LocalOptionBaseController<LocalDataType, DataRow, DataType>
    {
        public LocalDataTypesController(IGenericRepository<LocalDataType> repository, IAuthenticationService authService, IGenericRepository<DataType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
