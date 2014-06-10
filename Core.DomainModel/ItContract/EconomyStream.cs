﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel.ItContract
{
    public class EconomyStream : Entity
    {
        /// <summary>
        /// The EconomyStream might be an extern payment for a contract
        /// </summary>
        public ItContract ExternPaymentFor { get; set; }
        public int? ExternPaymentForId { get; set; }

        /// <summary>
        /// The EconomyStream might be an intern payment for a contract
        /// </summary>
        public ItContract InternPaymentFor { get; set; }
        public int? InternPaymentForId { get; set; }

        public int? OrganizationUnitId { get; set; }
        public virtual OrganizationUnit OrganizationUnit { get; set; }

        /// <summary>
        /// The field "anskaffelse"
        /// </summary>
        public int Acquisition { get; set; }

        /// <summary>
        /// The field "drift/år"
        /// </summary>
        public int Operation { get; set; }

        public int Other { get; set; }

        /// <summary>
        /// The field "kontering"
        /// </summary>
        public string AccountingEntry { get; set; }

        /// <summary>
        /// Traffic light for audit
        /// </summary>
        public TrafficLight AuditStatus { get; set; }

        /// <summary>
        /// DateTime for audit
        /// </summary>
        public DateTime? AuditDate { get; set; }

        public string Note { get; set; }

        public override bool HasUserWriteAccess(User user)
        {
            if (ExternPaymentFor != null && ExternPaymentFor.HasUserWriteAccess(user)) return true;
            if (InternPaymentFor != null && InternPaymentFor.HasUserWriteAccess(user)) return true;

            return base.HasUserWriteAccess(user);
        }

    }
}