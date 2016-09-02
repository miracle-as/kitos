using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Presentation.Web.Controllers.OData
{
    public class UsersController : BaseEntityController<User>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;

        public UsersController(IGenericRepository<User> repository, IAuthenticationService authService, IUserService userService)
            : base(repository, authService)
        {
            _authService = authService;
            _userService = userService;
        }

        public override IHttpActionResult Post(User entity)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }

        [ODataRoute("Users({userKey})/Remove()")]
        public IHttpActionResult PostRemove(int userKey)
        {
            var user = Repository.GetByKey(userKey);
            return Ok(user);
        }

        [ODataRoute("Users/Create")]
        public IHttpActionResult PostCreate(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = null;
            if (parameters.ContainsKey("user"))
            {
                user = parameters["user"] as User;
                Validate(user);
            }

            int organizationId = 0;
            if (parameters.ContainsKey("organizationId"))
            {
                organizationId = (int)parameters["organizationId"];
            }

            bool sendMailOnCreation = false;
            if (parameters.ContainsKey("sendMailOnCreation"))
            {
                sendMailOnCreation = (bool)parameters["sendMailOnCreation"];
            }

            // user is being created as global admin
            if (user.IsGlobalAdmin)
            {
                // only other global admins can create global admin users
                if (!_authService.IsGlobalAdmin(UserId))
                {
                    ModelState.AddModelError(nameof(user.IsGlobalAdmin), "You don't have permission to create a global admin user.");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            user.ObjectOwnerId = UserId;
            user.LastChangedByUserId = UserId;

            var createdUser = _userService.AddUser(user, sendMailOnCreation, organizationId);

            return Created(createdUser);
        }
    }
}
