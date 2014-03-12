﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using Core.DomainModel;
using Core.DomainServices;
using UI.MVC4.Models;

namespace UI.MVC4.Controllers.API
{
    public class GlobalAdminController : ApiController
    {
        private readonly IUserService _userService;

        public GlobalAdminController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "GlobalAdmin")]
        public HttpResponseMessage Post(UserDTO item)
        {
            try
            {
                var user = AutoMapper.Mapper.Map<UserDTO, User>(item);

                //TODO: Not hardcoding this
                user.Role_Id = 1;
                user.Municipality_Id = 1;

                user = _userService.AddUser(user);

                var msg = Request.CreateResponse(HttpStatusCode.Created, AutoMapper.Mapper.Map<User,UserDTO>(user));
                msg.Headers.Location = new Uri(Request.RequestUri + "/" + user.Id);
                return msg;
            }
            catch (SmtpException)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }
        }
    }
}
