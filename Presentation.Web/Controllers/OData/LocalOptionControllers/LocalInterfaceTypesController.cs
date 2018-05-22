using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalInterfaceTypesController : LocalOptionBaseController<LocalInterfaceType, ItInterface, InterfaceType, LocalInterfaceTypeDTO, ItInterfaceDTO, InterfaceTypeDTO>
    {
        public LocalInterfaceTypesController(IGenericRepository<LocalInterfaceType> repository, IAuthenticationService authService, IGenericRepository<InterfaceType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
