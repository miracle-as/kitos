﻿using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Core.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.OData;
using System.Web.OData.Routing;

namespace Presentation.Web.Controllers.OData.AttachedOptions
{
    using System.Net;

    public class AttachedOptionsRegularPersonalDataController : AttachedOptionsFunctionController<ItSystem, RegularPersonalDataType, LocalRegularPersonalDataType>
    {
        public AttachedOptionsRegularPersonalDataController(IGenericRepository<AttachedOption> repository, IAuthenticationService authService,
            IGenericRepository<RegularPersonalDataType> regularPersonalDataTypeRepository,
            IGenericRepository<LocalRegularPersonalDataType> localregularPersonalDataTypeRepository)
           : base(repository, authService, regularPersonalDataTypeRepository,
                 localregularPersonalDataTypeRepository){}

        [System.Web.Http.HttpGet]
        [EnableQuery]
        //[ODataRoute("GetRegularPersonalDataByObjectID(id={id},entitytype={entitytype})")]
        public IHttpActionResult GetOptionsByObjectID(int id, EntityType entitytype)
        {
            return base.GetOptionsByObjectIDAndType(id, entitytype, OptionType.REGULARPERSONALDATA);
        }
    }
}