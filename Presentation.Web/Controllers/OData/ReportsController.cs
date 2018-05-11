using System.Web.Http;
using System.Web.OData.Routing;
using Core.DomainModel.Reports;
using Core.DomainServices;
using Core.ApplicationServices;
using Presentation.Web.Models;
using System.Web.OData;

namespace Presentation.Web.Controllers.OData
{
    public class ReportsController : BaseEntityController<Report, ReportDTO>
    {
        public ReportsController(IGenericRepository<Report> repository, IAuthenticationService authService)
            : base(repository, authService)
        {}

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/Reports")]
        public IHttpActionResult GetReports([FromODataUri] int key)
        {
            return GetByOrganizationKey(key);
        }
    }
}
