﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class UserController : ApiController
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IUserService _userService;

        public UserController(IGenericRepository<User> repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }

        [Authorize]
        public HttpResponseMessage Post(UserDTO item)
        {
            try
            {
                var user = AutoMapper.Mapper.Map<UserDTO, User>(item);

                user = _userService.AddUser(user);

                var msg = Request.CreateResponse(HttpStatusCode.Created, AutoMapper.Mapper.Map<User,UserDTO>(user));
                msg.Headers.Location = new Uri(Request.RequestUri + "/" + user.Id);
                return msg;
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
        }
        
        [Authorize]
        public IEnumerable<UserDTO> Get()
        {
            return AutoMapper.Mapper.Map<IEnumerable<User>, List<UserDTO>>(_repository.Get());
        }
        
        [Authorize]
        public UserDTO Get(int id)
        {
            return AutoMapper.Mapper.Map<User,UserDTO>(_repository.GetByKey(id));
        }
    }

}
