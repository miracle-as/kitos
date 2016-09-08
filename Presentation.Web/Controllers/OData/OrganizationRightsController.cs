using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.ApplicationServices;
using Core.DomainServices;
using Core.DomainModel.Organization;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationRightsController : BaseController<OrganizationRight>
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;

        public OrganizationRightsController(IGenericRepository<OrganizationRight> repository, IUserService userService, IAuthenticationService authService)
            : base(repository)
        {
            _userService = userService;
            _authService = authService;
        }

        // GET /Organizations(1)/Rights
        [EnableQuery]
        [ODataRoute("Organizations({orgId})/Rights")]
        public IHttpActionResult GetRights(int orgId)
        {
            var result = Repository.AsQueryable().Where(x => x.OrganizationId == orgId);
            return Ok(result);
        }

        // DELETE /Organizations(1)/Rights(1)
        [EnableQuery]
        [ODataRoute("Organizations({orgId})/Rights({id})")]
        public IHttpActionResult DeleteRights(int orgId, int id)
        {
            var entity = Repository.AsQueryable().SingleOrDefault(m => m.OrganizationId == orgId && m.Id == id);
            if (entity == null)
                return NotFound();

            if (!_authService.HasWriteAccess(UserId, entity))
                return new StatusCodeResult(HttpStatusCode.Forbidden, this);

            try
            {
                Repository.DeleteByKey(id);
                Repository.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ODataRoute("Organizations({orgId})/Rights/User")]
        public IHttpActionResult DeleteRightsForUser(int orgId, ODataActionParameters parameters)
        {
            var test = false;
            if (parameters.ContainsKey("test"))
            {
                test = (bool)parameters["test"];
            }

            return Ok();
        }
    }
}
