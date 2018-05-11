using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.ApplicationServices;
using Core.DomainServices;
using Core.DomainModel.Organization;
using Presentation.Web.Models;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationRightsController : BaseEntityController<OrganizationRight, OrganizationRightDTO>
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;

        public OrganizationRightsController(IGenericRepository<OrganizationRight> repository, IUserService userService, IAuthenticationService authService)
            : base(repository, authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // GET /Organizations(1)/Rights
        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/Rights")]
        public IHttpActionResult GetRights(int orgKey)
        {
            var result = Repository.AsQueryable().Where(x => x.OrganizationId == orgKey);
            return Ok(result);
        }

        // POST /Organizations(1)/Rights
        [HttpPost]
        [ODataRoute("Organizations({orgKey})/Rights")]
        public IHttpActionResult PostRights(int orgKey, OrganizationRightDTO entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var mappedEntity = AutoMapper.Mapper.Map<OrganizationRight>(entity);

            mappedEntity.OrganizationId = orgKey;
            mappedEntity.ObjectOwnerId = UserId;
            mappedEntity.LastChangedByUserId = UserId;

            if (!_authService.HasWriteAccess(UserId, mappedEntity) && !_authService.IsLocalAdmin(this.UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            try
            {
                mappedEntity = Repository.Insert(mappedEntity);
                Repository.Save();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Created(mappedEntity);
        }

        // DELETE /Organizations(1)/Rights(1)
        [HttpDelete]
        [ODataRoute("Organizations({orgKey})/Rights({key})")]
        public IHttpActionResult DeleteRights(int orgKey, int key)
        {
            var entity = Repository.AsQueryable().SingleOrDefault(m => m.OrganizationId == orgKey && m.Id == key);
            if (entity == null)
                return NotFound();

            if (!_authService.HasWriteAccess(UserId, entity) && !_authService.IsLocalAdmin(this.UserId))
                return StatusCode(HttpStatusCode.Forbidden);

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
       
        public override IHttpActionResult Patch(int key, Delta<OrganizationRightDTO> delta)
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
                var enetitydto = AutoMapper.Mapper.Map<OrganizationRightDTO>(entity);
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
