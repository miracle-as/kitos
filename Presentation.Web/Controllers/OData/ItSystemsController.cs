﻿using System.Linq;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainModel;
using Core.DomainModel.ItSystem;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    public class ItSystemsController : BaseController<ItSystem>
    {
        public ItSystemsController(IGenericRepository<ItSystem> repository)
            : base(repository)
        {
        }

        // GET /Organizations(1)/ItSystems
        [EnableQuery]
        [ODataRoute("Organizations({key})/ItSystems")]
        public IHttpActionResult GetItSystems(int key)
        {
            var result = Repository.AsQueryable().Where(m => m.OrganizationId == key || m.AccessModifier == AccessModifier.Public);
            return Ok(result);
        }

        // GET /Organizations(1)/ItSystems(1)
        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/ItSystems({sysKey})")]
        public IHttpActionResult GetItSystems(int orgKey, int sysKey)
        {
            var result = Repository.AsQueryable().Where(m => m.Id == sysKey && (m.OrganizationId == orgKey || m.AccessModifier == AccessModifier.Public));
            return Ok(result);
        }
    }
}
