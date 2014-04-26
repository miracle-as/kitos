using System.Collections.Generic;

namespace Core.DomainModel.ItSystem
{
    public class ItSystem : IEntity<int>, IHasRights<ItSystemRight>, IHasAccessModifier
    {
        public ItSystem()
        {
            this.ExposedInterfaces = new List<ItSystem>();
            this.CanUseInterfaces = new List<ItSystem>();
            this.Children = new List<ItSystem>();
            this.TaskRefs = new List<TaskRef>();
            this.Rights = new List<ItSystemRight>();
        }

        public int Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }

        public int? ExposedById { get; set; }

        public int OrganizationId { get; set; }
        public int UserId { get; set; }

        public AccessModifier AccessModifier { get; set; }

        public string Description { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// If this system is an interface, which system exposed it
        /// </summary>
        public virtual ItSystem ExposedBy { get; set; }
        
        /// <summary>
        /// Which interfaces does this system expose
        /// </summary>
        public virtual ICollection<ItSystem> ExposedInterfaces { get; set; }

        /// <summary>
        /// If this system is an interface, which system can use it?
        /// </summary>
        public virtual ICollection<ItSystem> CanBeUsedBy { get; set; }
        
        /// <summary>
        /// Which interfaces can this system use
        /// </summary>
        public virtual ICollection<ItSystem> CanUseInterfaces { get; set; }

        /// <summary>
        /// Sub system
        /// </summary>
        public virtual ICollection<ItSystem> Children { get; set; }
        
        public int? ParentId { get; set; }
        /// <summary>
        /// Super systems
        /// </summary>
        public virtual ItSystem Parent { get; set; }

        /// <summary>
        /// Created inside which organization?
        /// </summary>
        public virtual Organization Organization { get; set; } //
        
        /// <summary>
        /// Created by
        /// </summary>
        public virtual User User { get; set; }

        public virtual ICollection<ItSystemRight> Rights { get; set; }

        /// <summary>
        /// Usages (binding between system and org)
        /// </summary>
        /// <value>
        /// The usages.
        /// </value>
        public virtual ICollection<ItSystemUsage> Usages { get; set; }

        /// <summary>
        /// an usage can specify an alternative parent system
        /// </summary>
        /// <value>
        /// The local parent usages.
        /// </value>
        public virtual ICollection<ItSystemUsage> LocalParentUsages { get; set; }

        public virtual AppType AppType { get; set; }
        public virtual BusinessType BusinessType { get; set; }

        public virtual ICollection<Wish> Wishes { get; set; }

        public virtual ICollection<TaskRef> TaskRefs { get; set; }
    }
}
