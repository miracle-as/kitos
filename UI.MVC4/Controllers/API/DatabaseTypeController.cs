﻿using System.Collections.Generic;
using Core.DomainModel.ItSystem;
using Core.DomainServices;

namespace UI.MVC4.Controllers.API
{
    public class DatabaseTypeController : GenericOptionApiController<DatabaseType, Technology>
    {
        public DatabaseTypeController(IGenericRepository<DatabaseType> repository) 
            : base(repository)
        {
        }

        protected override IEnumerable<DatabaseType> GetAllQuery()
        {
            return Repository.Get(x => x.IsActive);
        }
    }
}