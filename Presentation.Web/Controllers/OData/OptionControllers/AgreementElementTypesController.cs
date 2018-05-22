using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class AgreementElementTypesController: BaseOptionController<AgreementElementType, ItContract, OptionDTO, ItContractDTO>
    {
        public AgreementElementTypesController(IGenericRepository<AgreementElementType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}