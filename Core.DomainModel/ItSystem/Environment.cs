using System.Collections.Generic;

namespace Core.DomainModel.ItSystem
{
    public partial class Environment
    {
        public Environment()
        {
            this.Technologies = new List<Technology>();
        }

        public int Id { get; set; }
        public virtual ICollection<Technology> Technologies { get; set; }
    }
}
