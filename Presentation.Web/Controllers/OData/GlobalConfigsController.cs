using Core.ApplicationServices;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class GlobalConfigsController : BaseEntityController<Core.DomainModel.GlobalConfig, GlobalConfigDTO>
    {
        public GlobalConfigsController(IGenericRepository<Core.DomainModel.GlobalConfig> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}
