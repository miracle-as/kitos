using Core.ApplicationServices;
using Core.DomainModel.AdviceSent;
using Core.DomainServices;
using Presentation.Web.Controllers.OData;
using Presentation.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Web.Controllers.OData
{
    public class AdviceSentController : BaseEntityController<AdviceSent, AdviceSentDTO>
    {
        public AdviceSentController(IGenericRepository<AdviceSent> repository, IAuthenticationService authService): 
        base(repository,authService){ }
    }
}