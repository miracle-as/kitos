using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItInterfaceTypesController : LocalOptionBaseController<LocalItInterfaceType, ItInterface, ItInterfaceType, LocalItInterfaceTypeDTO, ItInterfaceDTO, OptionDTO>
    {
        public LocalItInterfaceTypesController(IGenericRepository<LocalItInterfaceType> repository, IAuthenticationService authService, IGenericRepository<ItInterfaceType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
