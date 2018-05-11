using System;
using Core.ApplicationServices;
using Core.DomainModel.Organization;
using Core.DomainServices;
using System.Net;
using System.Security;
using System.Threading;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;
using Core.DomainModel;
using Presentation.Web.Models;
using System.Linq;
using Swashbuckle.Swagger.Annotations;

namespace Presentation.Web.Controllers.OData
{
    public class OrganizationsController : BaseEntityController<Organization, OrganizationDTO>
    {
        private readonly IOrganizationService _organizationService;
        private readonly IOrganizationRoleService _organizationRoleService;
        private readonly IAuthenticationService _authService;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<OrganizationUnit> _orgUnitRepository;
        private readonly IGenericRepository<OrganizationUnitRight> _orgUnitRightrepository;

        public OrganizationsController(IGenericRepository<Organization> repository, IOrganizationService organizationService, 
            IOrganizationRoleService organizationRoleService, 
            IAuthenticationService authService, IGenericRepository<User> userRepository,
            IGenericRepository<OrganizationUnit> orgUnitRepository,
            IGenericRepository<OrganizationUnitRight> orgUnitRightrepository)
            : base(repository, authService)
        {
            _organizationService = organizationService;
            _organizationRoleService = organizationRoleService;
            _authService = authService;
            _userRepository = userRepository;
            _orgUnitRepository = orgUnitRepository;
            _orgUnitRightrepository = orgUnitRightrepository;
        }
        
        public override IHttpActionResult Post(OrganizationDTO organizationDTO)
        {
            var organization = Map(organizationDTO);

            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != organization.Id && !_authService.HasReadAccessOutsideContext(UserId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var user = _userRepository.GetByKey(UserId);

            CheckOrgTypeRights(organization);

            _organizationService.SetupDefaultOrganization(organization, user);

            var result = base.Post(organizationDTO).ExecuteAsync(new CancellationToken());

            if (result.Result.IsSuccessStatusCode)
            {
                if (organization.TypeId == 2)
                {
                    _organizationRoleService.MakeLocalAdmin(user, organization, user);
                    _organizationRoleService.MakeUser(user, organization, user);
                }
            }
            else
            {
                return StatusCode(result.Result.StatusCode);
            }

            return Created(organization);
        }

        public override IHttpActionResult Patch(int key, Delta<OrganizationDTO> delta)
        {
            var organization = Map(delta.GetInstance());

            CheckOrgTypeRights(organization);
            return base.Patch(key, delta);
        }

        //Organizations({orgKey})/namespace.RemoveUser
        //Has to be post in order for action to work. 
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, "Returns a Ok if user has been removed")]
        [SwaggerResponse(HttpStatusCode.BadRequest, "If an error in the request exists")]
        [SwaggerResponse(HttpStatusCode.NotFound, "If the organization Doesent exists.")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, "If the user dosent have write access")]
        public IHttpActionResult RemoveUser([FromODataUri] int key, ODataActionParameters parameters)
        {
        
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = Repository.GetByKey(key);
            if (entity == null)
                return NotFound();

            if (!_authService.HasWriteAccess(UserId, entity))
                return Unauthorized();

            var userId = 0;
           if (parameters.ContainsKey("userId"))
            {
                userId = (int)parameters["userId"];
                // TODO check if user is allowed to remove users from this organization
            }

            _organizationService.RemoveUser(key, userId);
            return Ok(true);
        }

        //GET /Organizations(1)/namespace.GetUsers
        [EnableQuery]
        public IHttpActionResult GetUsers([FromODataUri] int key)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != key && !_authService.HasReadAccessOutsideContext(UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            var result = _userRepository.AsQueryable().Where(m => m.OrganizationRights.Any(r => r.OrganizationId == key));
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgKey})/LastChangedByUser")]
        public virtual IHttpActionResult GetLastChangedByUser([FromODataUri] int Id)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != Id && !_authService.HasReadAccessOutsideContext(UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            var result = Repository.GetByKey(Id).LastChangedByUser;
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/ObjectOwner")]
        public virtual IHttpActionResult GetObjectOwner([FromODataUri] int orgKey)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != orgKey && !_authService.HasReadAccessOutsideContext(UserId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = Repository.GetByKey(orgKey).ObjectOwner;
            return Ok(result);
        }
        
        [EnableQuery]
        [ODataRoute("Organizations({orgId})/Type")]
        public virtual IHttpActionResult GetType([FromODataUri] int orgKey)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != orgKey && !_authService.HasReadAccessOutsideContext(UserId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = Repository.GetByKey(orgKey).Type;
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/DefaultOrganizationForUsers")]
        public IHttpActionResult GetDefaultOrganizationForUsers([FromODataUri] int orgKey)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != orgKey && !_authService.HasReadAccessOutsideContext(UserId))
                return StatusCode(HttpStatusCode.Forbidden);

            var result = _userRepository.AsQueryable().Where(m => m.DefaultOrganizationId == orgKey);
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/OrganizationUnits")]
        public IHttpActionResult GetOrganizationUnits([FromODataUri] int orgKey)
        {
            var loggedIntoOrgId = _authService.GetCurrentOrganizationId(UserId);
            if (loggedIntoOrgId != orgKey && !_authService.HasReadAccessOutsideContext(UserId))
            {
                return StatusCode(HttpStatusCode.Forbidden);
            }

            var result = _orgUnitRepository.AsQueryable().Where(m => m.OrganizationId == orgKey);
            return Ok(result);
        }

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/OrganizationUnits({unitKey})")]
        public IHttpActionResult GetOrganizationUnit([FromODataUri] int orgKey, int unitKey)
        {
            var entity = _orgUnitRepository.AsQueryable().SingleOrDefault(m => m.OrganizationId == orgKey && m.Id == unitKey);
            if (entity == null)
                return NotFound();

            if (_authService.HasReadAccess(UserId, entity))
                return Ok(entity);

            return StatusCode(HttpStatusCode.Forbidden);
        }

        /* /*

         */

        /*//Organizations({orgKey})/namespace.GetDefaultOrganizationForUsers
        

        //REPORTS FRA REPORTSCONTROLLER
        // GET /Organizations(1)/namespace.Reports
        

        // GET /Organizations(1)/namespace.OrganizationUnit(1)
        

        //FROM ORGUNITRIGHTS repository skal ændres
        */

        [EnableQuery]
        [ODataRoute("Organizations({orgId})/OrganizationUnits({orgUnitId})/Rights")]
        public IHttpActionResult GetByOrganizationUnit([FromODataUri] int orgId, int orgUnitId)
        {
            // TODO figure out how to check auth
            var result = _orgUnitRightrepository.AsQueryable().Where(x => x.Object.OrganizationId == orgId && x.ObjectId == orgUnitId);
            return Ok(result);
        }
        
        //PRIVATE FUNCTIONS herfra
        private void CheckOrgTypeRights(Organization organization)
        {
            if (organization.TypeId > 0)
            {
                var typeKey = (OrganizationTypeKeys) organization.TypeId;
                switch (typeKey)
                {
                    case OrganizationTypeKeys.Kommune:
                        if (!_authService.CanExecute(UserId, Feature.CanSetOrganizationTypeKommune))
                            throw new SecurityException();
                        break;
                    case OrganizationTypeKeys.Interessefællesskab:
                        if (!_authService.CanExecute(UserId, Feature.CanSetOrganizationTypeInteressefællesskab))
                            throw new SecurityException();
                        break;
                    case OrganizationTypeKeys.Virksomhed:
                        if (!_authService.CanExecute(UserId, Feature.CanSetOrganizationTypeVirksomhed))
                            throw new SecurityException();
                        break;
                    case OrganizationTypeKeys.AndenOffentligMyndighed:
                        if (!_authService.CanExecute(UserId, Feature.CanSetOrganizationTypeAndenOffentligMyndighed))
                            throw new SecurityException();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
