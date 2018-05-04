using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainServices;
using System.Net;
using Core.DomainModel.Organization;
using Core.ApplicationServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationUnitsController : BaseEntityController<OrganizationUnit, OrganizationUnitDTO>
    {
        private readonly IAuthenticationService _authService;

        public OrganizationUnitsController(IGenericRepository<OrganizationUnit> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
            _authService = authService;
        }
        /*USE INHERITED GETS instead
        [EnableQuery]
        [ODataRoute("OrganizationUnits")]
        public IHttpActionResult GetOrganizationUnits()
        {
            return base.Get();
        }

        //GET /OrganizationUnits(1)
        [EnableQuery]
        [ODataRoute("OrganizationUnits({unitKey})")]
        public IHttpActionResult GetOrganizationUnit(int unitKey)
        {
            return base.Get(unitKey);
        }
        */
    }
}
