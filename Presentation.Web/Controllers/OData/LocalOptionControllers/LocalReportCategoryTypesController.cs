using Core.ApplicationServices;
using Core.DomainModel.LocalOptions;
using Core.DomainModel.Reports;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalReportCategoryTypesController : LocalOptionBaseController<LocalReportCategoryType, Report, ReportCategoryType, LocalReportCategoryTypeDTO, ReportDTO, ReportCategoryTypeDTO>
    {
        public LocalReportCategoryTypesController(IGenericRepository<LocalReportCategoryType> repository, IAuthenticationService authService, IGenericRepository<ReportCategoryType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
