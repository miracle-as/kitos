using System.Data.Entity.ModelConfiguration;
using Core.DomainModel.ItProject;

namespace Infrastructure.DataAccess.Mapping
{
    public class EconomyMap : EntityTypeConfiguration<Economy>
    {
        public EconomyMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("Economy");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ItProject_Id).HasColumnName("ItProject_Id");

            // Relationships
            this.HasRequired(t => t.ItProject)
                .WithMany(t => t.Economies)
                .HasForeignKey(d => d.ItProject_Id);

        }
    }
}