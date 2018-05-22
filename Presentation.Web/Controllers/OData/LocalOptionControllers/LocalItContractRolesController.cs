using Core.ApplicationServices;
using Core.DomainModel.ItContract;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItContractRolesController : LocalOptionBaseController<LocalItContractRole, ItContractRight, ItContractRole, LocalItContractRoleDTO, RightOutputDTO, RoleDTO>
    {
        public LocalItContractRolesController(IGenericRepository<LocalItContractRole> repository, IAuthenticationService authService, IGenericRepository<ItContractRole> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
