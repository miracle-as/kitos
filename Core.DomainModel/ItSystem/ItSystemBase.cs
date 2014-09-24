﻿namespace Core.DomainModel.ItSystem
{
    public abstract class ItSystemBase : Entity
    {
        public string Name { get; set; }
        
        public string Description { get; set; }
        public string Url { get; set; }
        public AccessModifier AccessModifier { get; set; }

        public int OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the organization this instance was created under.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public virtual Organization Organization { get; set; }

        public int? BelongsToId { get; set; }
        /// <summary>
        /// Gets or sets the organization the system belongs to.
        /// </summary>
        /// <remarks>
        /// Belongs to is a OIO term - think "produced by".
        /// </remarks>
        /// <value>
        /// The organization the it system belongs to.
        /// </value>
        public virtual Organization BelongsTo { get; set; }
    }
}