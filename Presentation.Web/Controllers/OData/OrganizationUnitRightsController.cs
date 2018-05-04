﻿using Core.DomainServices;
using Core.DomainModel.Organization;
using System.Web.OData;
using System.Web.OData.Routing;
using System.Web.Http;
using System.Linq;

namespace Presentation.Web.Controllers.OData
{
    using System;
    using System.Net;
    using Core.ApplicationServices;
    using Models;

    public class OrganizationUnitRightsController : BaseEntityController<OrganizationUnitRight, OrganizationUnitRightDTO>
    {
        private readonly IAuthenticationService _authService;
        public OrganizationUnitRightsController(IGenericRepository<OrganizationUnitRight> repository, IAuthenticationService authService)
            : base(repository, authService)
        {
            _authService = authService;
        }
        
        public override IHttpActionResult Delete(int key)
        {
            var entity = Repository.GetByKey(key);

            if (entity == null)
                return NotFound();

            if (!_authService.HasWriteAccess(UserId, entity) && !_authService.IsLocalAdmin(this.UserId))
                return Unauthorized();

            try
            {
                Repository.DeleteByKey(key);
                Repository.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        public override IHttpActionResult Patch(int key, Delta<OrganizationUnitRightDTO> delta)
        {
            var entity = Repository.GetByKey(key);

            // does the entity exist?
            if (entity == null)
                return NotFound();

            // check if user is allowed to write to the entity
            if (!_authService.HasWriteAccess(UserId, entity) && !_authService.IsLocalAdmin(this.UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            //Check if user is allowed to set accessmodifier to public
            //var accessModifier = (entity as IHasAccessModifier)?.AccessModifier;
            //if (accessModifier == AccessModifier.Public && !_authService.CanExecute(UserId, Feature.CanSetAccessModifierToPublic))
            //{
            //    return Unauthorized();
            //}

            // check model state
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var enetitydto = AutoMapper.Mapper.Map<OrganizationUnitRightDTO>(entity);
                // patch the entity
                delta.Patch(enetitydto);
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
    }
}
