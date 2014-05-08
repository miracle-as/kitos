﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class State : IEntity<int>, IHasOwner
    {
        public int Id { get; set; }

        /// <summary>
        /// Human readable ID ("brugervendt noegle" in OIO)
        /// </summary>
        public string HumanReadableId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Note { get; set; }

        public int TimeEstimate { get; set; }

        /// <summary>
        /// Which date, the state should be reached
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Trafic light status for the state
        /// </summary>
        public int Status { get; set; }

        public int? AssociatedUserId { get; set; }
        public virtual User AssociatedUser { get; set; }

        public int? AssociatedActivityId { get; set; }
        public virtual Activity AssociatedActivity { get; set; }

        public int ObjectOwnerId { get; set; }
        public virtual User ObjectOwner { get; set; }

        /// <summary>
        /// The state might be a milestone for an IT project
        /// </summary>
        public virtual ItProject.ItProject MilestoneForProject { get; set; }
        public int? MilestoneForProjectId { get; set; }
    }
}
