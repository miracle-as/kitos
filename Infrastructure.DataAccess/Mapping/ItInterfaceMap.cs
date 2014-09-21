using Core.DomainModel.ItSystem;

namespace Infrastructure.DataAccess.Mapping
{
    public class ItInterfaceMap : EntityMap<ItInterface>
    {
        public ItInterfaceMap()
        {
            // Properties
            // Table & Column Mappings
            this.ToTable("ItInterface");

            // Relationships
            this.HasMany(t => t.CanBeUsedBy)
                .WithRequired(t => t.ItInterface)
                .HasForeignKey(d => d.ItInterfaceId);

            this.HasOptional(t => t.Interface)
                .WithMany(d => d.References)
                .HasForeignKey(t => t.InterfaceId);

            this.HasOptional(t => t.InterfaceType)
                .WithMany(d => d.References)
                .HasForeignKey(t => t.InterfaceTypeId);

            this.HasOptional(t => t.Method)
                .WithMany(d => d.References)
                .HasForeignKey(t => t.MethodId);
        }
    }
}
