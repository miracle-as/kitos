using System.Web.Http;
using System.Web.OData.Routing;
using Core.DomainModel.Reports;
using Core.DomainServices;
using Core.ApplicationServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class ReportsController : BaseEntityController<Report, ReportDTO>
    {
        public ReportsController(IGenericRepository<Report> repository, IAuthenticationService authService)
            : base(repository, authService)
        {}
    }
}
