using Core.DomainModel.ItProject;
using Core.DomainServices;
using Core.ApplicationServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class ItProjectStatusUpdatesController : BaseEntityController<ItProjectStatusUpdate, ItProjectStatusUpdateDTO>
    {
    public ItProjectStatusUpdatesController(IGenericRepository<ItProjectStatusUpdate>
        repository, IAuthenticationService authService)
        : base(repository, authService)
        {
        }
    }
}
