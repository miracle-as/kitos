﻿using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    [Authorize]
    public abstract class BaseController<T> : ODataController where T : class
    {
        protected ODataValidationSettings ValidationSettings;
        protected readonly IGenericRepository<T> Repository;

        protected BaseController(IGenericRepository<T> repository)
        {
            ValidationSettings = new ODataValidationSettings {AllowedQueryOptions = AllowedQueryOptions.All};
            Repository = repository;
        }

        protected int UserId
        {
            get
            {
                int userId;
                int.TryParse(User.Identity.Name, out userId);

                return userId;
            }
        }

        [EnableQuery]
        public virtual IHttpActionResult Get()
        {
            return Ok(Repository.AsQueryable());
        }

        [EnableQuery(MaxExpansionDepth = 4)]
        public virtual IHttpActionResult Get(int key)
        {
            var entity = Repository.GetByKey(key);
            return Ok(entity);
        }
    }
}
