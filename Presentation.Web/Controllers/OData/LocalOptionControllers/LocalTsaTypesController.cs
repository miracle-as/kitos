using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalTsaTypesController : LocalOptionBaseController<LocalTsaType, ItInterface, TsaType, LocalTsaTypeDTO, ItInterfaceDTO, OptionDTO>
    {
        public LocalTsaTypesController(IGenericRepository<LocalTsaType> repository, IAuthenticationService authService, IGenericRepository<TsaType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
