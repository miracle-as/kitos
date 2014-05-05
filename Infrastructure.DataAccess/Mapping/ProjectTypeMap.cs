using System.Data.Entity.ModelConfiguration;
using Core.DomainModel.ItProject;

namespace Infrastructure.DataAccess.Mapping
{
    public class ProjectTypeMap : EntityTypeConfiguration<ItProjectType>
    {
        public ProjectTypeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ItProjectType");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
