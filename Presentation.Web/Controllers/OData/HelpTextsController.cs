using Core.ApplicationServices;
using Core.DomainServices;
using Core.DomainModel;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class HelpTextsController : BaseEntityController<HelpText, HelpTextDTO>
    {
        public HelpTextsController(IGenericRepository<HelpText> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}