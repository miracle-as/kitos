﻿using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainServices;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Presentation.Web.Controllers.OData
{
    public class ItContractTemplateTypesController : BaseEntityController<ItContractTemplateType>
    {
        public ItContractTemplateTypesController(IGenericRepository<ItContractTemplateType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}