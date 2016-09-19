﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using Core.DomainModel.ItProject;
using Core.DomainServices;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.API
{
    public class ItProjectRightController : GenericRightsController<ItProject, ItProjectRight, ItProjectRole>
    {
        public ItProjectRightController(IGenericRepository<ItProjectRight> rightRepository, IGenericRepository<ItProject> objectRepository) : base(rightRepository, objectRepository)
        {
        }

        /// <summary>
        /// Returns all ItProjectRight for a specific user
        /// </summary>
        /// <param name="userId">Id of the user</param>
        /// <returns>List of rights</returns>
        public HttpResponseMessage GetRightsForUser(int userId)
        {
            try
            {
                var theRights = new List<ItProjectRight>();
                theRights.AddRange(RightRepository.Get(r => r.UserId == userId));

                var dtos = AutoMapper.Mapper.Map<ICollection<ItProjectRight>, ICollection<RightOutputDTO>>(theRights);

                return Ok(dtos);
            }
            catch (Exception e)
            {
                return LogError(e);
            }
        }
    }
}
