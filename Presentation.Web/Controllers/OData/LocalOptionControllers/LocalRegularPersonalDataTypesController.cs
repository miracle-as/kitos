using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.OptionControllers
{
    public class LocalRegularPersonalDataTypesController : LocalOptionBaseController<LocalRegularPersonalDataType, ItSystem, RegularPersonalDataType, LocalRegularPersonalDataTypeDTO, ItSystemDTO, RegularPersonalDataTypeDTO>
    {
        public LocalRegularPersonalDataTypesController(IGenericRepository<LocalRegularPersonalDataType> repository, IAuthenticationService authService, IGenericRepository<RegularPersonalDataType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}