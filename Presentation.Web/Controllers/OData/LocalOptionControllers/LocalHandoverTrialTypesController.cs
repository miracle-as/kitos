using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalHandoverTrialTypesController : LocalOptionBaseController<LocalHandoverTrialType, HandoverTrial, HandoverTrialType, LocalHandoverTrialTypeDTO, HandoverTrialDTO, HandoverTrialTypeDTO>
    {
        public LocalHandoverTrialTypesController(IGenericRepository<LocalHandoverTrialType> repository, IAuthenticationService authService, IGenericRepository<HandoverTrialType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
