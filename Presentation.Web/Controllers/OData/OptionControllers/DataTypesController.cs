﻿using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Presentation.Web.Controllers.OData
{
    public class DataTypesController : BaseEntityController<DataType>
    {
        public DataTypesController(IGenericRepository<DataType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}