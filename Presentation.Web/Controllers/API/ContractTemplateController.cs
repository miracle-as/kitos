﻿using Core.DomainModel.ItContract;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class ContractTemplateController : GenericOptionApiController<ItContractTemplateType, ItContract, OptionDTO>
    {
        public ContractTemplateController(IGenericRepository<ItContractTemplateType> repository)
            : base(repository)
        {
        }
    }
}
