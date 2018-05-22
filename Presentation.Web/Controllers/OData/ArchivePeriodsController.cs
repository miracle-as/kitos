using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.ApplicationServices;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class ArchivePeriodsController : BaseEntityController<ArchivePeriod, ArchivePeriodDTO>
    {
        // GET: ArchivePeriode
        public ArchivePeriodsController(IGenericRepository<ArchivePeriod> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
        }
    }
}