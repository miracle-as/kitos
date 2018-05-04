using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class OrganizationUnitDTO
    {
        public string LocalId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// EAN number of the department.
        /// </summary>
        public long? Ean { get; set; }

        public int? ParentId { get; set; }
        /// <summary>
        /// Parent department.
        /// </summary>
        public virtual OrganizationUnitDTO Parent { get; set; }
        public virtual ICollection<OrganizationUnitDTO> Children { get; set; }

        public int OrganizationId { get; set; }
        /// <summary>
        /// The organization which the unit belongs to.
        /// </summary>
        public virtual OrganizationDTO Organization { get; set; }

        /// <summary>
        /// The usage of task on this Organization Unit.
        /// Should be a subset of the TaskUsages of the parent department.
        /// </summary>
        public virtual ICollection<TaskUsageDTO> TaskUsages { get; set; }

        /// <summary>
        /// Local tasks that was created in this unit
        /// </summary>
        public virtual ICollection<TaskRefDTO> TaskRefs { get; set; }

        public virtual ICollection<TaskRefDTO> OwnedTasks { get; set; }
        /// <summary>
        /// Gets or sets the delegated system usages.
        /// TODO write better summary
        /// </summary>
        /// <value>
        /// The delegated system usages.
        /// </value>
        public virtual ICollection<ItSystemUsageDTO> DelegatedSystemUsages { get; set; }

        /// <summary>
        /// Gets or sets it system usages.
        /// TODO write better summary
        /// </summary>
        /// <value>
        /// It system usages.
        /// </value>
        public virtual ICollection<ItSystemUsageDTO> ItSystemUsages { get; set; } // TODO is this used anymore isn't Using used instead?

        /// <summary>
        /// Users which have set this as their default OrganizationUnit.
        /// </summary>
        /// <remarks>
        /// Goes through <seealso cref="OrganizationRight"/>.
        /// So to access the user you must call .User on the rights object.
        /// </remarks>
        public virtual ICollection<OrganizationRightDTO> DefaultUsers { get; set; }

        /// <summary>
        /// This Organization Unit is using these IT Systems (Via ItSystemUsage)
        /// </summary>
        public virtual ICollection<ItSystemUsageOrgUnitUsageDTO> Using { get; set; }

        /// <summary>
        /// This Organization Unit is using these IT projects
        /// </summary>
        public virtual ICollection<ItProjectOrgUnitUsageDTO> UsingItProjects { get; set; }

        /// <summary>
        /// This Organization Unit is responsible for these IT ItContracts
        /// </summary>
        public virtual ICollection<ItContractDTO> ResponsibleForItContracts { get; set; }

        /// <summary>
        /// The Organization Unit is listed in these economy streams
        /// </summary>
        public virtual ICollection<EconomyStreamDTO> EconomyStreams { get; set; }
    }
}