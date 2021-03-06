using Core.DomainModel.ItContract;
using Core.DomainModel.Advice;

namespace Infrastructure.DataAccess.Mapping
{
    public class AdviceMap : EntityMap<Advice>
    {
        public AdviceMap()
        {
            // Table & Column Mappings
            this.ToTable("Advice");
            this.HasMany(a => a.AdviceSent)
                .WithRequired(a => a.Advice)
                .WillCascadeOnDelete(true);
            this.HasMany(a=> a.Reciepients)
                .WithRequired(ar => ar.Advice)
                .WillCascadeOnDelete(true);
            
          
            

            /*this.HasMany(a => a.CarbonCopyRecievers)
                .WithRequired(a => a.Advice)
                .WillCascadeOnDelete(true);

            // Relationships
             this.HasRequired(t => t.ItContract)
                 .WithMany(t => t.Advices)
                 .HasForeignKey(d => d.ItContractId);
                 */
            /*  this.HasOptional(t => t.Receiver)
                  .WithMany(d => d.ReceiverFor)
                  .HasForeignKey(t => t.ReceiverId);

              this.HasOptional(t => t.CarbonCopyReceiver)
                  .WithMany(d => d.CarbonCopyReceiverFor)
                  .HasForeignKey(t => t.CarbonCopyReceiverId);
      */
        }
    }
}
