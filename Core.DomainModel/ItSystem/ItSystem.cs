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
            
            this.Rights = new List<ItSystemRight>();
        }

        public int Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }

        public int? ParentId { get; set; }
        public int? ExposedById { get; set; }

        public int OrganizationId { get; set; }
        public int UserId { get; set; }

        public AccessModifier AccessModifier { get; set; }

        public string Description { get; set; }
        public string Url { get; set; }

        /* if this system is an interface, which system exposed it */
        public virtual ItSystem ExposedBy { get; set; }
        /* which interfaces does this system expose */
        public virtual ICollection<ItSystem> ExposedInterfaces { get; set; }
        
        /* if this system is an interface, which system can use it? */
        public virtual ICollection<ItSystem> CanBeUsedBy { get; set; }
        /* which interfaces can this system use */
        public virtual ICollection<ItSystem> CanUseInterfaces { get; set; }

        /* sub systems*/
        public virtual ICollection<ItSystem> Children { get; set; }
        /* super systems */
        public virtual ItSystem Parent { get; set; }

        public virtual Organization Organization { get; set; } //created inside which organization?
        public virtual User User { get; set; } //created by

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

        //TODO KLE!!
    }
}
