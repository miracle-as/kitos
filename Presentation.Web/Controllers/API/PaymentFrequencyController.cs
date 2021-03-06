﻿using Core.DomainModel.ItContract;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class PaymentFrequencyController : GenericOptionApiController<PaymentFreqencyType, ItContract, OptionDTO>
    {
        public PaymentFrequencyController(IGenericRepository<PaymentFreqencyType> repository)
            : base(repository)
        {
        }
    }
}
