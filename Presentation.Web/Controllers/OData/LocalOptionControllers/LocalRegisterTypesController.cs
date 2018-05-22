using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.ItSystemUsage;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalRegisterTypesController : LocalOptionBaseController<LocalRegisterType, ItSystemUsage, RegisterType, LocalRegisterTypeDTO, ItSystemUsageDTO, OptionDTO>
    {
        public LocalRegisterTypesController(IGenericRepository<LocalRegisterType> repository, IAuthenticationService authService, IGenericRepository<RegisterType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}