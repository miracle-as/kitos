﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using Newtonsoft.Json.Linq;

namespace UI.MVC4.Controllers.API
{
    public abstract class GenericApiController<TModel, TKeyType, TDto> : BaseApiController // TODO perhaps it's possible to infer the TKeyType from TModel somehow
        where TModel : class, IEntity<TKeyType>
    {
        protected readonly IGenericRepository<TModel> Repository;

        protected GenericApiController(IGenericRepository<TModel> repository)
        {
            Repository = repository;
        }

        protected virtual TDest Map<TSource, TDest>(TSource item)
        {
            return AutoMapper.Mapper.Map<TDest>(item);
        }

        protected virtual IEnumerable<TModel> GetAllQuery()
        {
            return Repository.Get();
        }

        public HttpResponseMessage GetAll()
        {
            var items = GetAllQuery();

            return Ok(Map<IEnumerable<TModel>, IEnumerable<TDto>>(items));
        }

        // GET api/T
        public HttpResponseMessage GetSingle(TKeyType id)
        {
            var item = Repository.GetByKey(id);

            if (item == null)
                return NotFound();

            return Ok(Map<TModel, TDto>(item));
        }

        protected virtual TModel PostQuery(TModel item)
        {
            Repository.Insert(item);
            Repository.Save();

            return item;
        }

        // POST api/T
        //[Authorize(Roles = "GlobalAdmin")] TODO: FIX!
        public virtual HttpResponseMessage Post(TDto dto)
        {
            var item = Map<TDto, TModel>(dto);
            try
            {
                PostQuery(item);

                //var msg = new HttpResponseMessage(HttpStatusCode.Created);
                return Created(item, new Uri(Request.RequestUri + "/" + item.Id));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        protected virtual TModel PutQuery(TModel item)
        {
            Repository.Update(item);
            Repository.Save();

            return item;
        }

        // PUT api/T
        //[Authorize(Roles = "GlobalAdmin")] TODO: FIX!
        public virtual HttpResponseMessage Put(TKeyType id, TDto dto)
        {
            var item = Map<TDto, TModel>(dto);
            item.Id = id;
            try
            {
                PutQuery(item);

                return Ok(); //TODO correct?
            }
            catch (Exception)
            {
                return NoContent(); // TODO catch correct expection
            }
        }

        protected virtual void DeleteQuery(TKeyType id)
        {
            Repository.DeleteByKey(id);
            Repository.Save();
        }

        // DELETE api/T
        //[Authorize(Roles = "GlobalAdmin")] TODO: FIX!
        public virtual HttpResponseMessage Delete(TKeyType id)
        {
            try
            {
                DeleteQuery(id);

                return Ok();
            }
            catch (Exception)
            {
                return NoContent(); // TODO catch correct expection
            }
        }

        protected virtual TModel PatchQuery(TModel item)
        {
            Repository.Update(item);
            Repository.Save();

            return item;
        }

        // PATCH api/T
        //[Authorize(Roles = "GlobalAdmin")] TODO: FIX!
        public virtual HttpResponseMessage Patch(TKeyType id, JObject obj)
        {
            var item = Repository.GetByKey(id);
            var itemType = item.GetType();

            foreach (var valuePair in obj)
            {
                // get name of mapped property
                var map =
                    AutoMapper.Mapper.FindTypeMapFor<TDto, TModel>()
                              .GetPropertyMaps();
                var nonNullMaps = map.Where(x => x.SourceMember != null);
                var mapMember = nonNullMaps.SingleOrDefault(x => x.SourceMember.Name == valuePair.Key);
                if (mapMember == null) 
                    continue; // abort if no map found

                var destName = mapMember.DestinationProperty.Name;
                var jToken = valuePair.Value;

                var propRef = itemType.GetProperty(destName);
                var t = propRef.PropertyType;
                // use reflection to call obj.Value<t>("keyName");
                var genericMethod = jToken.GetType().GetMethod("Value").MakeGenericMethod(new Type[] { t });
                var value = genericMethod.Invoke(obj, new object[] { valuePair.Key });
                // update the entity
                propRef.SetValue(item, value);
            }

            try
            {
                PatchQuery(item);

                //pretty sure we'll get a merge conflict here???
                return Ok(); // TODO correct?
            }
            catch (Exception)
            {
                return NoContent(); // TODO catch correct expection
            }
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
            base.Dispose(disposing);
        }
    }
}