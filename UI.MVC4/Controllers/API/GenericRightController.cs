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
    public abstract class GenericRightController<TRight, TObject, TRole> : BaseApiController
        where TObject : Entity
        where TRole : Entity, IRoleEntity
        where TRight : Entity, IRight<TObject, TRole>
    {
        protected readonly IGenericRepository<TRight> RightRepository;
        private readonly IGenericRepository<TObject> _objectRepository;

        protected GenericRightController(IGenericRepository<TRight> rightRepository, IGenericRepository<TObject> objectRepository)
        {
            RightRepository = rightRepository;
            _objectRepository = objectRepository;
        }

        protected virtual IEnumerable<TRight> GetAll(int oId)
        {
            return RightRepository.Get(right => right.ObjectId == oId);
        } 

        protected virtual bool HasWriteAccess(TObject theObject, User user)
        {
            if (theObject.ObjectOwnerId == user.Id) return true; 

            var rights = RightRepository.Get(right => right.ObjectId == theObject.Id && right.UserId == user.Id).ToList();
            return rights.Any(right => right.Role.HasWriteAccess);
        }

        private bool HasWriteAccess(int objId, int userId)
        {
            var user = UserRepository.GetByKey(userId);
            var theObject = _objectRepository.GetByKey(objId);
            return HasWriteAccess(theObject, user);
        }

        public HttpResponseMessage Get(int id)
        {
            try
            {
                var rights = GetAll(id);
                var dtos = AutoMapper.Mapper.Map<IEnumerable<TRight>, IEnumerable<RightOutputDTO>>(rights);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetHasWriteAccess(int id, bool? hasWriteAccess)
        {
            try
            {
                return Ok(HasWriteAccess(id, KitosUser.Id));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage GetHasWriteAccess(bool? hasWriteAccess, int oId, int uId)
        {
            try
            {
                return Ok(HasWriteAccess(oId, uId));
            }
            catch (Exception e)
            {
                return Error(e);
            }
        }

        public HttpResponseMessage Post(RightInputDTO inputDTO)
        {
            try
            {
                if (!HasWriteAccess(inputDTO.ObjectId, KitosUser.Id))
                    return Unauthorized();

                var right = AutoMapper.Mapper.Map<RightInputDTO, TRight>(inputDTO);

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

        public HttpResponseMessage Delete([FromUri] int oId, [FromUri] int rId, [FromUri] int uId)
        {
            try
            {
                RightRepository.DeleteByKey(oId, rId, uId);
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
