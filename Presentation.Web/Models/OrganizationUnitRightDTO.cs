using Core.DomainModel.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class OrganizationUnitRightDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ObjectId { get; set; }
        public virtual UserDTO User { get; set; }
        public virtual RoleDTO Role { get; set; }
        public virtual OrganizationUnitDTO Object { get; set; }
    }
}