using Core.ApplicationServices;
using Core.DomainModel.ItProject;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItProjectRolesController : LocalOptionBaseController<LocalItProjectRole, ItProjectRight, ItProjectRole, LocalItProjectRoleDTO, RightInputDTO, RoleDTO>
    {
        public LocalItProjectRolesController(IGenericRepository<LocalItProjectRole> repository, IAuthenticationService authService, IGenericRepository<ItProjectRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
