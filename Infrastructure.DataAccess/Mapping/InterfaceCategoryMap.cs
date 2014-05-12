﻿using System.Data.Entity.ModelConfiguration;
using Core.DomainModel.ItSystem;

namespace Infrastructure.DataAccess.Mapping
{
    public class InterfaceCategoryMap : EntityTypeConfiguration<InterfaceCategory>
    {
        public InterfaceCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("InterfaceCategory");
            this.Property(t => t.Id).HasColumnName("Id");
        }
    }
}