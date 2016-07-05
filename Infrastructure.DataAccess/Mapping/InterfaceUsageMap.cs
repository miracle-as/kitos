﻿using System.Data.Entity.ModelConfiguration;
using Core.DomainModel.ItSystemUsage;

namespace Infrastructure.DataAccess.Mapping
{
    class InterfaceUsageMap : EntityTypeConfiguration<InterfaceUsage>
    {
        public InterfaceUsageMap()
        {
            // Primary key
            this.HasKey(x => new { x.ItSystemUsageId, x.ItSystemId, x.ItInterfaceId });

            // Table & Column Mappings
            this.ToTable("InfUsage"); // MySQL: rename else it'll cause name length issues

            this.HasRequired(t => t.ItSystemUsage)
                .WithMany(d => d.InterfaceUsages)
                .HasForeignKey(t => t.ItSystemUsageId)
                .WillCascadeOnDelete(false);

            this.HasRequired(t => t.ItInterfaceUse)
                .WithMany(t => t.InterfaceUsages)
                .HasForeignKey(d => new {d.ItSystemId, d.ItInterfaceId})
                .WillCascadeOnDelete(false);

            this.HasOptional(t => t.ItContract)
                .WithMany(d => d.AssociatedInterfaceUsages)
                .HasForeignKey(t => t.ItContractId)
                .WillCascadeOnDelete(true);

            this.HasOptional(t => t.Infrastructure)
                .WithMany()
                .HasForeignKey(t => t.InfrastructureId);
        }
    }
}
