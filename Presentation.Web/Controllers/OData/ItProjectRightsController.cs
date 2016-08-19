using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainModel.ItProject;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    public class ItProjectRightsController : BaseController<ItProjectRight>
    {
        public ItProjectRightsController(IGenericRepository<ItProjectRight> repository)
            : base(repository)
        {
        }

        // GET /Organizations(1)/ItProjects(1)/Rights
        [EnableQuery]
        [ODataRoute("Organizations({orgId})/ItProjects({projId})/Rights")]
        public IHttpActionResult GetByItProject(int orgId, int projId)
        {
            // TODO figure out how to check auth
            var result = Repository.AsQueryable().Where(x => x.Object.OrganizationId == orgId && x.ObjectId == projId);
            return Ok(result);
        }

        //[EnableQuery]
        [ODataRoute("ItProjectRights/Default.Test(orgKey={orgKey})")]
        public IHttpActionResult GetTest(int orgKey)
        {
            // TODO this is an example of how to implement an OData function
            var result = Repository.AsQueryable().Where(x => x.Object.OrganizationId == orgKey);
            return Ok(result);
        }

        // GET /Users(1)/ItProjectRights
        [EnableQuery]
        [ODataRoute("Users({userId})/ItProjectRights")]
        public IHttpActionResult GetByUser(int userId)
        {
            // TODO figure out how to check auth
            var result = Repository.AsQueryable().Where(x => x.UserId == userId);
            return Ok(result);
        }
    }
}
