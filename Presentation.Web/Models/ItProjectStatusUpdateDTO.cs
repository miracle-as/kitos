using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class ItProjectStatusUpdateDTO
    {
        public int? AssociatedItProjectId { get; set; }
        /// <summary>
        /// Gets or sets the associated it project.
        /// </summary>
        /// <value>
        /// The associated it project.
        /// </value>
        public virtual ItProjectDTO AssociatedItProject { get; set; }
        public bool IsCombined { get; set; }
        public string Note { get; set; }
        public TrafficLight TimeStatus { get; set; }
        public TrafficLight QualityStatus { get; set; }
        public TrafficLight ResourcesStatus { get; set; }
        public TrafficLight CombinedStatus { get; set; }
        public DateTime Created { get; set; }
        public int OrganizationId { get; set; }
        public bool IsFinal { get; set; }
    }
}