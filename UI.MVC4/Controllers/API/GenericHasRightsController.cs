﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public abstract class GenericHasRightsController<TObject, TRight, TRole, TDto> : GenericApiController<TObject, TDto>
        where TObject : HasRightsEntity<TObject, TRight, TRole>
        where TRight : Entity, IRight<TObject, TRight, TRole>
        where TRole : IRoleEntity<TRight>
    {
        protected readonly IGenericRepository<TRight> RightRepository;

        protected GenericHasRightsController(IGenericRepository<TObject> repository, IGenericRepository<TRight> rightRepository ) : base(repository)
        {
            RightRepository = rightRepository;
        }

        /// <summary>
        /// Returns all rights for an object
        /// </summary>
        /// <param name="id">The id of the object</param>
        /// <returns>List of all rights</returns>
        protected IEnumerable<TRight> GetRightsQuery(int id)
        {
            return RightRepository.Get(right => right.ObjectId == id);
        }

        /// <summary>
        /// Get all rights for a given object
        /// </summary>
        /// <param name="id">The id of the object</param>
        /// <param name="rights">Routing qualifier</param>
        /// <returns>List of rights</returns>
        public virtual HttpResponseMessage GetRights(int id, bool? rights)
        {
            try
            {
                var theRights = GetRightsQuery(id);
                var dtos = Map<IEnumerable<TRight>, IEnumerable<RightOutputDTO>>(theRights);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        /// <summary>
        /// Post a new right to the object
        /// </summary>
        /// <param name="id">The id of the object</param>
        /// <param name="dto">DTO of right</param>
        /// <returns></returns>
        public HttpResponseMessage PostRight(int id, RightInputDTO dto)
        {
            try
            {
                if (!HasWriteAccess(id, KitosUser))
                    return Unauthorized();

                var right = AutoMapper.Mapper.Map<RightInputDTO, TRight>(dto);
                right.ObjectId = id;
                right.ObjectOwner = KitosUser;

                right = RightRepository.Insert(right);
                RightRepository.Save();

                //TODO: FIX navigation properties not loading properly!!!
                right.User = UserRepository.GetByKey(right.UserId);

                var outputDTO = AutoMapper.Mapper.Map<TRight, RightOutputDTO>(right);

                return Created(outputDTO);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        /// <summary>
        /// Delete a right from the object
        /// </summary>
        /// <param name="id">ID of object</param>
        /// <param name="rId">ID of role</param>
        /// <param name="uId">ID of user in role</param>
        /// <returns></returns>
        public HttpResponseMessage Delete(int id, [FromUri] int rId, [FromUri] int uId)
        {
            try
            {
                if (!HasWriteAccess(id, KitosUser))
                    return Unauthorized();

                var right = RightRepository.Get(r => r.ObjectId == id && r.RoleId == rId && r.UserId == uId).FirstOrDefault();

                if (right == null) return NotFound();

                RightRepository.DeleteByKey(right.Id);
                RightRepository.Save();

                return Ok();
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }
    }


    
}