﻿using Core.DomainModel.ItSystem;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class DataTypeController : GenericOptionApiController<DataType, DataRow, OptionDTO>
    {

        public DataTypeController(IGenericRepository<DataType> repository)
            : base(repository)
        {
        }
    }
}
