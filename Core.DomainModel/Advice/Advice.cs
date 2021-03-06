using System;
using System.Collections.Generic;
using Core.DomainModel.ItContract;
using Core.DomainModel.ItProject;
using Core.DomainModel.ItSystem;

namespace Core.DomainModel.Advice
{
    public enum ObjectType
    {
        itContract,
        itSystemUsage,
        itProject,
        itInterface
    }
    public enum Scheduling
    {
       Immediate,
       Hour,
       Day,
       Week,
       Month,
       Year

    }
    /// <summary>
    /// Contains info about Advices on a contract.
    /// </summary>
    public class Advice : Entity, IProjectModule, ISystemModule, IContractModule
    {
        public Advice() {
            AdviceSent = new List<AdviceSent.AdviceSent>();
            Reciepients = new List<AdviceUserRelation>();
        }

        public virtual ICollection<AdviceSent.AdviceSent> AdviceSent { get; set; }
        public virtual ICollection<AdviceUserRelation> Reciepients { get; set; }
        public int? RelationId { get; set; }
        public ObjectType? Type { get; set; }

        public Scheduling? Scheduling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the contract name.
        /// </summary>
        /// <value>
        /// The contract name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the advice alarm date.
        /// </summary>
        /// <remarks>
        /// Once the alarm expires an email should be sent to all users assigned to
        /// the <see cref="Core.DomainModel.ItContract"/> with the role defined in <see cref="Receiver"/>
        /// and <see cref="CarbonCopyReceiver"/>.
        /// </remarks>
        /// <value>
        /// The advice alarm date.
        /// </value>
        public DateTime? AlarmDate { get; set; }


        /// <summary>
        /// Gets or sets the stop date.
        /// </summary>
        /// <value>
        /// The stop date.
        /// </value>
        public DateTime? StopDate { get; set; }

        /// <summary>
        /// Gets or sets the sent date.
        /// </summary>
        /// <value>
        /// The sent date.
        /// </value>
        public DateTime? SentDate { get; set; }

        /// <summary>
        /// Gets or sets the receiver contract role identifier.
        /// </summary>
        /// <remarks>
        /// Contract role id of which to send email.
        /// </remarks>
        /// <value>
        /// The receiver identifier.
        /// </value>
        public int? ReceiverId { get; set; }
        /// <summary>
        /// Gets or sets the receiver contract role.
        /// </summary>
        /// <remarks>
        /// Contract role of which to send email.
        /// </remarks>
        /// <value>
        /// The receiver.
        /// </value>
        public IRoleEntity Receiver { get; set; }

        /// <summary>
        /// Gets or sets the carbon copy receiver contract role identifier.
        /// </summary>
        /// <remarks>
        /// Contract role of which to send email.
        /// </remarks>
        /// <value>
        /// The carbon copy receiver identifier.
        /// </value>
        public int? CarbonCopyReceiverId { get; set; }
        /// <summary>
        /// Gets or sets the carbon copy contract role receiver.
        /// </summary>
        /// <remarks>
        /// Contract role of which to send email.
        /// </remarks>
        /// <value>
        /// The carbon copy receiver.
        /// </value>
        public IRoleEntity CarbonCopyReceiver { get; set; }
        /// <summary>
        /// Gets or sets the body of the email.
        /// </summary>
        /// <value>
        /// The email body.
        /// </value>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the subject of the email.
        /// </summary>
        /// <value>
        /// The email subject.
        /// </value>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the job id.
        /// </summary>
        public string JobId { get; set; }

        /// <summary>
        /// Determines whether a user has write access to this instance.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        ///   <c>true</c> if user has write access, otherwise <c>false</c>.
        /// </returns>
        public override bool HasUserWriteAccess(User user)
        {
           /* if (Object != null && Object.HasUserWriteAccess(user))
                return true;
                */
            return base.HasUserWriteAccess(user);
        }

    }
}
