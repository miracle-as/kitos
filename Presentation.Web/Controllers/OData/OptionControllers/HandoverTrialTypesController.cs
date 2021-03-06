﻿using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class HandoverTrialTypesController : BaseOptionController<HandoverTrialType, HandoverTrial>
    {
        public HandoverTrialTypesController(IGenericRepository<HandoverTrialType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}