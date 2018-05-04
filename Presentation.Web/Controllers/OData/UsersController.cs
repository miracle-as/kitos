using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.Organization;
using Core.DomainServices;
using Presentation.Web.Models;
using Presentation.Web.Models.CreateModels.User;
using Swashbuckle.Swagger.Annotations;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Presentation.Web.Controllers.OData
{
    public class UsersController : BaseEntityController<User, UserDTO>
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;
        private readonly IGenericRepository<User> _repository;
        private readonly IGenericRepository<OrganizationUnitRight> _orgUnitRightsrepository;

        public UsersController(IGenericRepository<User> repository, IAuthenticationService authService, IUserService userService,
            IGenericRepository<OrganizationUnitRight> orgUnitRightsrepository)
            : base(repository, authService)
        {
            _authService = authService;
            _userService = userService;
            _repository = repository;
            _orgUnitRightsrepository = orgUnitRightsrepository;
        }
        [SwaggerResponse(HttpStatusCode.MethodNotAllowed, "This method is not allowed user the Action Create instead")]
        public override IHttpActionResult Post(UserDTO entity)
        {
            return StatusCode(HttpStatusCode.MethodNotAllowed);
        }
        
        [SwaggerResponse(HttpStatusCode.Created, "Returns the created user", typeof(UserDTO))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Returned if an error occurs")]
        public IHttpActionResult Create(CreateUserPayload payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User user = AutoMapper.Mapper.Map<User>(payload.User);

            Validate(payload.User); // this will set the ModelState if not valid - it doesn't http://stackoverflow.com/questions/39484185/model-validation-in-odatacontroller
      
            if (user?.Email != null && EmailExists(user.Email))
            {
                ModelState.AddModelError(nameof(user.Email), "Email is already in use.");
            }

            // user is being created as global admin
            if (user?.IsGlobalAdmin == true)
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

            var createdUser = _userService.AddUser(user, payload.SendMailOnCreation, payload.OrganizationId);
            var userToReturn = AutoMapper.Mapper.Map<UserDTO>(createdUser);
            return Created(createdUser);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Returns a bool saying if the email is available", typeof(bool))]
        public IHttpActionResult IsEmailAvailable(string email)
        {
            // strip strange single quotes from parameter
            // http://stackoverflow.com/questions/39510551/string-parameter-to-bound-function-contains-single-quotes
            var strippedEmail = email.Remove(0, 1);
            strippedEmail = strippedEmail.Remove(strippedEmail.Length-1);

            if (EmailExists(strippedEmail))
                return Ok(false);
            else
                return Ok(true);
        }

        [SwaggerResponse(HttpStatusCode.OK, "Returns user with matching email", typeof(UserDTO))]
        [SwaggerResponse(HttpStatusCode.NotFound, "returns notFound if no user is found")]
        public IHttpActionResult GetUserByEmail(string email)
        {
            // strip strange single quotes from parameter
            // http://stackoverflow.com/questions/39510551/string-parameter-to-bound-function-contains-single-quotes
            var strippedEmail = email.Remove(0, 1);
            strippedEmail = strippedEmail.Remove(strippedEmail.Length - 1);

            var userToReturn = this._repository.AsQueryable().FirstOrDefault(u => u.Email.ToLower() == strippedEmail.ToLower());
            if(userToReturn != null)
            {
                var result = AutoMapper.Mapper.Map<UserDTO>(userToReturn);
                return Ok(userToReturn);
            }
            return NotFound();
        }
        
        [EnableQuery]
        [SwaggerResponse(HttpStatusCode.OK, "Returns organizationUnit rights for a specific user", typeof(UserDTO))]
        public IHttpActionResult OrganizationUnitRights([FromODataUri] int userId)
        {
            // TODO figure out how to check auth
            var result = _orgUnitRightsrepository.AsQueryable().Where(x => x.UserId == userId);
            return Ok(result);
        }

        private bool EmailExists(string email)
        {
            var matchingEmails = Repository.Get(x => x.Email == email);
            return matchingEmails.Any();
        }
    }
}
