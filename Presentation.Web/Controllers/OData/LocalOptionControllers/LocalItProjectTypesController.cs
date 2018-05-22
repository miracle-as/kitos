using Core.ApplicationServices;
using Core.DomainModel.ItProject;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalItProjectTypesController : LocalOptionBaseController<LocalItProjectType, ItProject, ItProjectType, LocalItProjectTypeDTO, ItProjectDTO, OptionDTO>
    {
        public LocalItProjectTypesController(IGenericRepository<LocalItProjectType> repository, IAuthenticationService authService, IGenericRepository<ItProjectType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
