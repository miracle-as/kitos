﻿using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using System.Linq;
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
                Validate(user); // this will set the ModelState if not valid - it doesn't http://stackoverflow.com/questions/39484185/model-validation-in-odatacontroller
            }

            var organizationId = 0;
            if (parameters.ContainsKey("organizationId"))
            {
                organizationId = (int)parameters["organizationId"];
                // TODO check if user is allowed to add users to this organization
            }

            var sendMailOnCreation = false;
            if (parameters.ContainsKey("sendMailOnCreation"))
            {
                sendMailOnCreation = (bool)parameters["sendMailOnCreation"];
            }

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

            var createdUser = _userService.AddUser(user, sendMailOnCreation, organizationId);

            return Created(createdUser);
        }

        [ODataRoute("Users/IsEmailAvailable(email={email})")]
        public IHttpActionResult GetIsEmailAvailable(string email)
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

        private bool EmailExists(string email)
        {
            var matchingEmails = Repository.Get(x => x.Email == email);
            return matchingEmails.Any();
        }
    }
}
