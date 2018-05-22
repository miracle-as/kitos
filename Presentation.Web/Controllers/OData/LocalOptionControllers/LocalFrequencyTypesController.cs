using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalFrequencyTypesController : LocalOptionBaseController<LocalFrequencyType, DataRowUsage, FrequencyType, LocalFrequencyTypeDTO, DataRowUsageDTO, FrequencyTypeDTO>
    {
        public LocalFrequencyTypesController(IGenericRepository<LocalFrequencyType> repository, IAuthenticationService authService, IGenericRepository<FrequencyType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
