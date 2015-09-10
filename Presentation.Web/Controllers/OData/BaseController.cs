﻿using System;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using Core.DomainServices;

namespace Presentation.Web.Controllers.OData
{
    public abstract class BaseController<T> : ODataController where T : class
    {
        protected ODataValidationSettings ValidationSettings;
        protected IGenericRepository<T> Repository;

        protected BaseController(IGenericRepository<T> repository)
        {
            ValidationSettings = new ODataValidationSettings {AllowedQueryOptions = AllowedQueryOptions.All};
            Repository = repository;
        }

        [EnableQuery]
        public IHttpActionResult Get()
        {
            return Ok(Repository.AsQueryable());
        }

        [EnableQuery(MaxExpansionDepth = 4)]
        public IHttpActionResult Get(int key)
        {
            var entity = Repository.GetByKey(key);
            return Ok(entity);
        }

        // TODO for now only read actions are allowed, in future write will be enabled - but keep security in mind!

        //protected IHttpActionResult Put(int key, T entity)
        //{
        //    return StatusCode(HttpStatusCode.NotImplemented);
        //}

        //protected IHttpActionResult Post(T entity)
        //{
        //    Validate(entity);

        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    try
        //    {
        //        entity = Repository.Insert(entity);
        //        Repository.Save();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }

        //    return Created(entity);
        //}

        //protected IHttpActionResult Patch(int key, Delta<T> delta)
        //{
        //    Validate(delta.GetEntity());

        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    var entity = Repository.GetByKey(key);
        //    if(entity == null) 
        //        return NotFound();

        //    try
        //    {
        //        delta.Patch(entity);
        //        Repository.Save();
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }

        //    return Updated(entity);
        //}

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
