using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class LocalOptionEntityDTO
    {
        public int? ObjectOwnerId { get; set; }
        public int? LastChangedByUserId { get; set; }
        public string Description { get; set; }

        public virtual OrganizationDTO Organization { get; set; }

        public int OrganizationId { get; set; }

        public virtual OptionDTO Option { get; set; }

        public int OptionId { get; set; }

        public bool IsActive { get; set; }
    }
}