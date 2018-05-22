using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItContractTypesController : LocalOptionBaseController<LocalItContractType, ItContract, ItContractType, LocalItContractTypeDTO, ItContractDTO, OptionDTO>
    {
        public LocalItContractTypesController(IGenericRepository<LocalItContractType> repository, IAuthenticationService authService, IGenericRepository<ItContractType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
