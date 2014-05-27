using System;

namespace Core.DomainModel.ItContract
{
    public class PaymentMilestone : IEntity<int>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Expected { get; set; }
        public DateTime? Approved { get; set; }
        
        public int ItContractId { get; set; }
        public virtual ItContract ItContract { get; set; }
    }
}