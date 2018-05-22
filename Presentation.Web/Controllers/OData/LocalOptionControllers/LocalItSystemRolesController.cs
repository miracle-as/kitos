using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItSystemRolesController : LocalOptionBaseController<LocalItSystemRole, ItSystemRight, ItSystemRole, LocalItSystemRoleDTO, RightInputDTO, RoleDTO>
    {
        public LocalItSystemRolesController(IGenericRepository<LocalItSystemRole> repository, IAuthenticationService authService, IGenericRepository<ItSystemRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
