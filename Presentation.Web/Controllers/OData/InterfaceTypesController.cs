﻿using Core.DomainModel.ItSystem;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    public class InterfaceTypesController : BaseController<InterfaceType>
    {
        public InterfaceTypesController(IGenericRepository<InterfaceType> repository)
            : base(repository)
        {
        }
    }
}