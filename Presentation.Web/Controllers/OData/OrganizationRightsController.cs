using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainServices;
using Core.DomainModel.Organization;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationRightsController : BaseController<OrganizationRight>
    {
        public OrganizationRightsController(IGenericRepository<OrganizationRight> repository)
            : base(repository)
        {
        }

        // GET /Organizations(1)/Rights
        [EnableQuery]
        [ODataRoute("Organizations({orgId})/Rights")]
        public IHttpActionResult GetRights(int orgId)
        {
            var result = Repository.AsQueryable().Where(x => x.OrganizationId == orgId);
            return Ok(result);
        }
    }
}
