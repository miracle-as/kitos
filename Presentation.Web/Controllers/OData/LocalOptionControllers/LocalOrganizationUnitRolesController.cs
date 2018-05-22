using Core.ApplicationServices;
using Core.DomainModel.LocalOptions;
using Core.DomainModel.Organization;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{

    public class LocalOrganizationUnitRolesController : LocalOptionBaseController<LocalOrganizationUnitRole, OrganizationUnitRight, OrganizationUnitRole, LocalOrganizationUnitRoleDTO, RightInputDTO, RoleDTO>
    {
        public LocalOrganizationUnitRolesController(IGenericRepository<LocalOrganizationUnitRole> repository, IAuthenticationService authService, IGenericRepository<OrganizationUnitRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}