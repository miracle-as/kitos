using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class ItInterfaceExhibitsController : BaseEntityController<ItInterfaceExhibit, ItInterfaceExhibitDTO>
    {
        public ItInterfaceExhibitsController(IGenericRepository<ItInterfaceExhibit> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}
