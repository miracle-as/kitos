using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalProcurementStrategyTypesController : LocalOptionBaseController<LocalProcurementStrategyType, ItContract, ProcurementStrategyType, LocalProcurementStrategyTypeDTO, ItContractDTO, OptionDTO>
    {
        public LocalProcurementStrategyTypesController(IGenericRepository<LocalProcurementStrategyType> repository, IAuthenticationService authService, IGenericRepository<ProcurementStrategyType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
