using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItContractTemplateTypesController : LocalOptionBaseController<LocalItContractTemplateType, ItContract, ItContractTemplateType, LocalItContractTemplateTypeDTO, ItContractDTO, OptionDTO>
    {
        public LocalItContractTemplateTypesController(IGenericRepository<LocalItContractTemplateType> repository, IAuthenticationService authService, IGenericRepository<ItContractTemplateType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
