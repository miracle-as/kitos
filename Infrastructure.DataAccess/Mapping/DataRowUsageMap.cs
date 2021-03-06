﻿using System.Data.Entity.ModelConfiguration;
using Core.DomainModel.ItSystemUsage;

namespace Infrastructure.DataAccess.Mapping
{
    public class DataRowUsageMap : EntityTypeConfiguration<DataRowUsage>
    {
        public DataRowUsageMap()
        {
            // Primary key
            this.HasKey(x => new {x.DataRowId, x.ItSystemUsageId, x.ItSystemId, x.ItInterfaceId});

            // Properties
            this.Property(x => x.ItSystemUsageId);
            this.Property(x => x.ItSystemId);
            this.Property(x => x.ItInterfaceId);

            // Table & Column Mappings
            this.ToTable("DataRowUsage");

            this.HasRequired(t => t.ItInterfaceUsage)
                .WithMany(t => t.DataRowUsages)
                .HasForeignKey(d => new {d.ItSystemUsageId, d.ItSystemId, d.ItInterfaceId})
                .WillCascadeOnDelete(true);

            this.HasRequired(t => t.DataRow)
                .WithMany(d => d.Usages)
                .HasForeignKey(t => t.DataRowId)
                .WillCascadeOnDelete(true);

            this.HasOptional(t => t.Frequency)
                .WithMany(d => d.References)
                .HasForeignKey(t => t.FrequencyId);
        }
    }
}
