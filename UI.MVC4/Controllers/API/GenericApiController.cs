﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;

namespace UI.MVC4.Controllers
{
    public abstract class GenericApiController<TModel, TKeyType> : ApiController // TODO perhaps it's possible to infer the TKeyType from TModel somehow
        where TModel : class, IEntity<TKeyType>
    {
        protected readonly IGenericRepository<TModel> Repository;

        protected GenericApiController(IGenericRepository<TModel> repository)
        {
            Repository = repository;
        }

        // GET api/T
        public virtual TModel Get(TKeyType id)
        {
            return Repository.GetById(id);
        }

        // POST api/T
        [Authorize(Roles = "Admin")]
        public virtual HttpResponseMessage Post(TModel item)
        {
            try
            {
                Repository.Insert(item);
                Repository.Save();

                var msg = new HttpResponseMessage(HttpStatusCode.Created);
                msg.Headers.Location = new Uri(Request.RequestUri + item.Id.ToString());
                return msg;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
        }

        // PUT api/T
        [Authorize(Roles = "Admin")]
        public virtual HttpResponseMessage Put(TKeyType id, TModel item)
        {
            item.Id = id;
            try
            {
                Repository.Update(item);
                Repository.Save();

                return new HttpResponseMessage(HttpStatusCode.OK); // TODO correct?
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        // DELETE api/T
        [Authorize(Roles = "Admin")]
        public virtual HttpResponseMessage Delete(TKeyType id)
        {
            try
            {
                Repository.DeleteById(id);
                Repository.Save();

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
