using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalMethodTypesController : LocalOptionBaseController<LocalMethodType, ItInterface, MethodType, LocalMethodTypeDTO, ItInterfaceDTO, OptionDTO>
    {
        public LocalMethodTypesController(IGenericRepository<LocalMethodType> repository, IAuthenticationService authService, IGenericRepository<MethodType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
