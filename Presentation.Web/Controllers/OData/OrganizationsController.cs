using Core.DomainModel.Organization;
using Core.DomainServices;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationsController : BaseEntityController<Organization>
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IGenericRepository<Organization> repository, IOrganizationService organizationService)
            : base(repository)
        {
            _organizationService = organizationService;
        }

        [ODataRoute("Organizations({orgKey})/RemoveUser")]
        public IHttpActionResult DeleteRemoveUserFromOrganization(int orgKey, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = 0;
            if (parameters.ContainsKey("userId"))
            {
                userId = (int)parameters["userId"];
                // TODO check if user is allowed to remove users from this organization
            }

            _organizationService.RemoveUser(orgKey, userId);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ODataRoute("Organizations({orgKey})/AddUser")]
        public IHttpActionResult PostAddUserToOrganization(int orgKey, ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = 0;
            if (parameters.ContainsKey("userId"))
            {
                userId = (int)parameters["userId"];
                // TODO check if user is allowed to add users to this organization
            }

            //_organizationService.RemoveUser(orgKey, userId); TODO

            return StatusCode(HttpStatusCode.NoContent);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/LastChangedByUser")]
        public virtual IHttpActionResult GetLastChangedByUser(int orgKey)
        {
            var loggedIntoOrgId = CurrentOrganizationId;
            if (loggedIntoOrgId != orgKey && !AuthenticationService.HasReadAccessOutsideContext(UserId))
                return new StatusCodeResult(HttpStatusCode.Forbidden, this);

            var result = Repository.GetByKey(orgKey).LastChangedByUser;
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/ObjectOwner")]
        public virtual IHttpActionResult GetObjectOwner(int orgKey)
        {
            var loggedIntoOrgId = CurrentOrganizationId;
            if (loggedIntoOrgId != orgKey && !AuthenticationService.HasReadAccessOutsideContext(UserId))
            {
                return new StatusCodeResult(HttpStatusCode.Forbidden, this);
            }

            var result = Repository.GetByKey(orgKey).ObjectOwner;
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/Type")]
        public virtual IHttpActionResult GetType(int orgKey)
        {
            var loggedIntoOrgId = CurrentOrganizationId;
            if (loggedIntoOrgId != orgKey && !AuthenticationService.HasReadAccessOutsideContext(UserId))
            {
                return new StatusCodeResult(HttpStatusCode.Forbidden, this);
            }

            var result = Repository.GetByKey(orgKey).Type;
            return Ok(result);
        }
    }
}
