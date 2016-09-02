using System.Web.Http;
using System.Web.OData;
using Core.DomainServices;
using Core.ApplicationServices;
using System.Net;
using System;
using Core.DomainModel;

namespace Presentation.Web.Controllers.OData
{
    public abstract class BaseEntityController<T> : BaseController<T> where T : Entity
    {
        private readonly IAuthenticationService _authService;

        protected BaseEntityController(IGenericRepository<T> repository, IAuthenticationService authService)
            : base(repository)
        {
            _authService = authService;
        }

        public IHttpActionResult Put(int key, T entity)
        {
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // TODO how do we check access here?
        public virtual IHttpActionResult Post(T entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                entity = Repository.Insert(entity);
                Repository.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Created(entity);
        }

        public virtual IHttpActionResult Patch(int key, Delta<T> delta)
        {
            var entity = Repository.GetByKey(key);

            // does the entity exist?
            if (entity == null)
                return NotFound();

            // check if user is allowed to write to the entity
            if (!_authService.HasWriteAccess(UserId, entity))
                return StatusCode(HttpStatusCode.Forbidden);

            // check model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // patch the entity
                delta.Patch(entity);
                Repository.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            // add the request header "Prefer: return=representation"
            // if you want the updated entity returned,
            // else you'll just get 204 (No Content) returned
            return Updated(entity);
        }

        // TODO for now only read actions are allowed, in future write will be enabled - but keep security in mind!
        //protected IHttpActionResult Delete(int key)
        //{
        //    var entity = Repository.GetByKey(key);
        //    if (entity == null)
        //        return NotFound();

        //    try
        //    {
        //        Repository.DeleteByKey(key);
        //        Repository.Save();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}
    }
}
