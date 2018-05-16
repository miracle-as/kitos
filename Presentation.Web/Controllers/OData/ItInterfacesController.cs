using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using Core.ApplicationServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class ItInterfacesController : BaseEntityController<ItInterface, ItInterfaceDTO>
    {
        private readonly IAuthenticationService _authService;

        public ItInterfacesController(IGenericRepository<ItInterface> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
            _authService = authService;
        }

        // GET /Organizations(1)/ItInterfaces
        [EnableQuery]
        [ODataRoute("Organizations({key})/ItInterfaces")]
        public IHttpActionResult GetItInterfaces(int key)
        {
            var result = Repository.AsQueryable().Where(m => m.OrganizationId == key || m.AccessModifier == AccessModifier.Public);
            return Ok(result);
        }

        // GET /Organizations(1)/ItInterfaces(1)
        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/ItInterfaces({interfaceKey})")]
        public IHttpActionResult GetItInterfaces(int orgKey, int interfaceKey)
        {
            var entity = Repository.AsQueryable().SingleOrDefault(m => m.OrganizationId == orgKey && m.Id == interfaceKey);
            if (entity == null)
                return NotFound();

            if (_authService.HasReadAccess(UserId, entity))
                return Ok(entity);

            return StatusCode(HttpStatusCode.Forbidden);
        }
    }
}
