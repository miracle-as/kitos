using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class LocalSensistivePersonalDataTypesController : LocalOptionBaseController<LocalSensitivePersonalDataType, ItSystem, SensitivePersonalDataType, LocalSensitivePersonalDataTypeDTO, ItSystemDTO, SensitivePersonalDataTypeDTO>
    {
        public LocalSensistivePersonalDataTypesController(IGenericRepository<LocalSensitivePersonalDataType> repository, IAuthenticationService authService, IGenericRepository<SensitivePersonalDataType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}