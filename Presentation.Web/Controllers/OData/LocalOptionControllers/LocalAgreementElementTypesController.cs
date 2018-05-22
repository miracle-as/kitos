using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalAgreementElementTypesController : LocalOptionBaseController<LocalAgreementElementType, ItContract, AgreementElementType, LocalOptionEntityDTO, ItContractDTO>
    {
        public LocalAgreementElementTypesController(IGenericRepository<LocalAgreementElementType> repository, IAuthenticationService authService, IGenericRepository<AgreementElementType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
