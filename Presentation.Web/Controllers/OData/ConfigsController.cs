using Core.ApplicationServices;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class ConfigsController : BaseEntityController<Core.DomainModel.Config, ConfigDTO>
    {
        public ConfigsController(IGenericRepository<Core.DomainModel.Config> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}
