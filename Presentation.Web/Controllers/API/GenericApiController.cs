﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using Core.DomainModel;
using Newtonsoft.Json.Linq;
using Presentation.Web.Models;
using Presentation.Web.Models.Exceptions;
using Core.DomainServices;

namespace Presentation.Web.Controllers.API
{
    public abstract class GenericApiController<TModel, TDto> : BaseApiController
        where TModel : class, IEntity
    {
        protected readonly IGenericRepository<TModel> Repository;

        protected GenericApiController(IGenericRepository<TModel> repository)
        {
            Repository = repository;
        }

        protected virtual IQueryable<TModel> GetAllQuery()
        {
            return Repository.AsQueryable();
        }

        public virtual HttpResponseMessage GetAll([FromUri] PagingModel<TModel> paging)
        {
            try
            {
                var hasOrg = typeof(IHasOrganization).IsAssignableFrom(typeof(TModel));
                var result = GetAllQuery().AsEnumerable();

                if (AuthenticationService.HasReadAccessOutsideContext(KitosUser.Id) || hasOrg == false)
                {
                    if (typeof(IHasAccessModifier).IsAssignableFrom(typeof(TModel)) && !AuthenticationService.IsGlobalAdmin(KitosUser.Id))
                    {
                        if (hasOrg)
                        {
                            result = result.Where(x => ((IHasAccessModifier)x).AccessModifier == AccessModifier.Public || ((IHasOrganization)x).OrganizationId == KitosUser.DefaultOrganizationId);
                        }
                        else
                        {
                            result = result.Where(x => ((IHasAccessModifier)x).AccessModifier == AccessModifier.Public);
                        }
                    }
                }
                else
                {
                    result = result.Where(x => ((IHasOrganization)x).OrganizationId == KitosUser.DefaultOrganizationId);
                }

                var query = Page(result.AsQueryable(), paging);
                var dtos = Map(query);
                return Ok(dtos);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        // GET api/T
        public virtual HttpResponseMessage GetSingle(int id)
        {
            try
            {
                var item = Repository.GetByKey(id);

                if(!AuthenticationService.HasReadAccess(KitosUser.Id, item))
                {
                    return Unauthorized();
                }

                if (item == null) return NotFound();

                var dto = Map(item);
                return Ok(dto);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        /// <summary>
        /// GET api/T/id?hasWriteAccess
        /// Returns whether the current authenticated user has write access
        /// to the object with the given id
        /// </summary>
        /// <param name="id">The id of the object</param>
        /// <param name="organizationId"></param>
        /// <param name="hasWriteAccess">Route qualifier</param>
        /// <returns>True or false</returns>
        public HttpResponseMessage GetHasWriteAccess(int id, int organizationId, bool? hasWriteAccess)
        {
            try
            {
                return Ok(HasWriteAccess(id, organizationId));
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        protected virtual TModel PostQuery(TModel item)
        {
            var insertedItem = Repository.Insert(item);
            Repository.Save();

            return insertedItem;
        }

        // POST api/T
        public virtual HttpResponseMessage Post(TDto dto)
        {
            try
            {
                var item = Map<TDto, TModel>(dto);

                item.ObjectOwner = KitosUser;
                item.LastChangedByUser = KitosUser;

                var savedItem = PostQuery(item);

                return Created(Map(savedItem), new Uri(Request.RequestUri + "/" + savedItem.Id));
            }
            catch (ConflictException e)
            {
                return Conflict(e.Message);
            }
            catch (SecurityException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                // check if inner message is a duplicate, if so return conflict
                if (e.InnerException?.InnerException != null)
                    if (e.InnerException.InnerException.Message.Contains("Duplicate entry"))
                        return Conflict(e.InnerException.InnerException.Message);

                return LogError(e);
            }
        }

        protected virtual TModel PutQuery(TModel item)
        {
            Repository.Update(item);
            Repository.Save();

            return item;
        }

        // PUT api/T
        public virtual HttpResponseMessage Put(int id, int organizationId, JObject obj)
        {
            return Patch(id, organizationId, obj);
        }

        protected virtual void DeleteQuery(TModel entity)
        {
            Repository.DeleteByKey(entity.Id);
            Repository.Save();
        }

        // DELETE api/T
        public virtual HttpResponseMessage Delete(int id, int organizationId)
        {
            try
            {
                var item = Repository.GetByKey(id);
                if (!HasWriteAccess(item, organizationId)) return Unauthorized();

                DeleteQuery(item);

                return Ok();
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }

        protected virtual TModel PatchQuery(TModel item, JObject obj)
        {
            if (obj != null)
            {
                var itemType = item.GetType();
                // get name of mapped property
                var map = AutoMapper.Mapper.FindTypeMapFor<TDto, TModel>().GetPropertyMaps();
                var nonNullMaps = map.Where(x => x.SourceMember != null).ToList();

                foreach (var valuePair in obj)
                {
                    var mapMember = nonNullMaps.SingleOrDefault(x => x.SourceMember.Name.Equals(valuePair.Key, StringComparison.InvariantCultureIgnoreCase));
                    if (mapMember == null)
                        continue; // abort if no map found

                    var destName = mapMember.DestinationProperty.Name;
                    var jToken = valuePair.Value;

                    if (destName == "LastChangedByUserId" || destName == "LastChanged")
                        continue; // don't allow writing to these. TODO This should really be done using in/out DTOs

                    var propRef = itemType.GetProperty(destName);
                    var t = propRef.PropertyType;

                    // we have to handle enums separately
                    if (t.IsEnum)
                    {
                        var value = valuePair.Value.Value<string>();
                        var enumValue = Enum.Parse(t, value, true);
                        propRef.SetValue(item, enumValue);
                    }
                    // parse null values properly
                    else if (t.IsEquivalentTo(typeof(int?)))
                    {
                        var value = valuePair.Value.Value<string>();

                        int intValue;
                        if (!String.IsNullOrEmpty(value) && Int32.TryParse(value, out intValue))
                        {
                            propRef.SetValue(item, intValue);
                        }
                        else
                        {
                            propRef.SetValue(item, null);
                        }
                    }
                    // BUG JSON.NET throws on Guid
                    // Bugreport https://json.codeplex.com/workitem/25599
                    else if (t.IsEquivalentTo(typeof(Guid)))
                    {
                        Guid guid;
                        Guid.TryParse(valuePair.Value.Value<string>(), out guid);
                        propRef.SetValue(item, guid);
                    }
                    else
                    {
                        try
                        {
                            // get reference to the generic method obj.Value<t>(parameter);
                            var genericMethod = jToken.GetType().GetMethod("Value").MakeGenericMethod(new Type[] { t });
                            // use reflection to call obj.Value<t>("keyName");
                            var value = genericMethod.Invoke(obj, new object[] { valuePair.Key });
                            // update the entity
                            propRef.SetValue(item, value);
                        }
                        catch
                        {
                            // ignore any errors with setting the value
                            // this should only happen when trying to set values,
                            // that aren't ment to be set via the API
                        }
                    }
                }

                item.LastChanged = DateTime.UtcNow;
                item.LastChangedByUser = KitosUser;
            }
            Repository.Update(item);
            Repository.Save();

            return item;
        }

        // PATCH api/T
        public virtual HttpResponseMessage Patch(int id, int organizationId, JObject obj)
        {
            try
            {
                var item = Repository.GetByKey(id);
                if (item == null) return NotFound();
                if (!HasWriteAccess(item, organizationId)) return Unauthorized();

                var result = PatchQuery(item, obj);
                return Ok(Map(result));
            }
            catch (Exception e)
            {
                // check if inner message is a duplicate, if so return conflict
                if (e.InnerException != null)
                    if (e.InnerException.InnerException != null)
                        if (e.InnerException.InnerException.Message.Contains("Duplicate entry"))
                            return Conflict(e.InnerException.InnerException.Message);

                return LogError(e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            Repository.Dispose();
            base.Dispose(disposing);
        }

        #region Write Access Checks functions

        /// <summary>
        /// Checks if a given user has write access to a given object.
        /// Override this method as needed.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="user">The user</param>
        /// <param name="organizationId"></param>
        /// <returns>True iff user has write access to obj</returns>
        protected virtual bool HasWriteAccess(TModel obj, User user, int organizationId)
        {
            return AuthenticationService.HasWriteAccess(user.Id, obj);
        }

        /// <summary>
        /// Checks if the current authenticated user has write access to a given object.
        /// </summary>
        /// <param name="objId">The id of object</param>
        /// <param name="organizationId"></param>
        /// <returns>True iff user has write access to the object with objId</returns>
        protected bool HasWriteAccess(int objId, int organizationId)
        {
            return HasWriteAccess(objId, KitosUser, organizationId);
        }

        /// <summary>
        /// Checks if a given user has write access to a given object.
        /// </summary>
        /// <param name="objId">The id of object</param>
        /// <param name="user">The user</param>
        /// <param name="organizationId"></param>
        /// <returns>True iff user has write access to the object with objId</returns>
        protected bool HasWriteAccess(int objId, User user, int organizationId)
        {
            var obj = Repository.GetByKey(objId);
            return HasWriteAccess(obj, user, organizationId);
        }

        /// <summary>
        /// Checks if the current authenticated user has write access to a given object.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="organizationId"></param>
        /// <returns>True iff user has write access to obj</returns>
        protected bool HasWriteAccess(TModel obj, int organizationId)
        {
            return HasWriteAccess(obj, KitosUser, organizationId);
        }

        #endregion

        #region Mapping functions

        //for easy access
        protected virtual TDto Map(TModel model)
        {
            return Map<TModel, TDto>(model);
        }

        //for easy access
        protected virtual TModel Map(TDto inputDto)
        {
            return Map<TDto, TModel>(inputDto);
        }

        //for easy access (list)
        protected virtual IEnumerable<TDto> Map(IEnumerable<TModel> models)
        {
            return Map<IEnumerable<TModel>, IEnumerable<TDto>>(models);
        }

        //for easy access (list)
        protected virtual IEnumerable<TModel> Map(IEnumerable<TDto> inputDtos)
        {
            return Map<IEnumerable<TDto>, IEnumerable<TModel>>(inputDtos);
        }

        #endregion

    }
}
