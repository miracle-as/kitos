using Core.DomainModel;
using Core.DomainModel.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class ReportDTO
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryTypeId { get; set; }
        public virtual ReportCategoryType CategoryType { get; set; }
        public int OrganizationId { get; set; }
        public virtual OrganizationDTO Organization { get; set; }

        /// <summary>
        /// report definition saved as a json string
        /// </summary>
        public string Definition { get; set; }

        public AccessModifier AccessModifier { get; set; }
    }
}