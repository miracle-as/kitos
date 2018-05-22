using Core.ApplicationServices;
using Core.DomainServices;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class AccessTypesController : BaseEntityController<AccessType, AccessTypeDTO>
    {
        public AccessTypesController(IGenericRepository<AccessType> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }


    }
}