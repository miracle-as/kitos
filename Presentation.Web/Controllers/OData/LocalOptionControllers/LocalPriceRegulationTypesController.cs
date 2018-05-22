using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalPriceRegulationTypesController : LocalOptionBaseController<LocalPriceRegulationType, ItContract, PriceRegulationType, LocalPriceRegulationTypeDTO, ItContractDTO, OptionDTO>
    {
        public LocalPriceRegulationTypesController(IGenericRepository<LocalPriceRegulationType> repository, IAuthenticationService authService, IGenericRepository<PriceRegulationType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
