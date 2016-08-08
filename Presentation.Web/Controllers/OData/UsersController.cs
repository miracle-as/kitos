using Core.DomainModel;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    public class UsersController : BaseController<User>
    {
        public UsersController(IGenericRepository<User> repository)
            : base(repository)
        {
        }
    }
}
