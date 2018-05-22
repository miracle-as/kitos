using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalPurchaseFormTypesController : LocalOptionBaseController<LocalPurchaseFormType, ItContract, PurchaseFormType, LocalPurchaseFormTypeDTO, ItContractDTO, OptionDTO>
    {
        public LocalPurchaseFormTypesController(IGenericRepository<LocalPurchaseFormType> repository, IAuthenticationService authService, IGenericRepository<PurchaseFormType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
