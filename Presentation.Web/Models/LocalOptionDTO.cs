using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class LocalOptionDTO
    {
        public string Description { get; set; }

        public virtual OrganizationDTO Organization { get; set; }

        public int OrganizationId { get; set; }

        public int OptionId { get; set; }

        public bool IsActive { get; set; }

    }
}