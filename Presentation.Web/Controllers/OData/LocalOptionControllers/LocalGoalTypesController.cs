using Core.ApplicationServices;
using Core.DomainModel.ItProject;
using Core.DomainModel.LocalOptions;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData.LocalOptionControllers
{
    public class LocalGoalTypesController : LocalOptionBaseController<LocalGoalType, Goal, GoalType, LocalGoalTypeDTO, GoalDTO, GoalTypeDTO>
    {
        public LocalGoalTypesController(IGenericRepository<LocalGoalType> repository, IAuthenticationService authService, IGenericRepository<GoalType> optionsRepository)
            : base(repository, authService, optionsRepository)
        {
        }
    }
}
