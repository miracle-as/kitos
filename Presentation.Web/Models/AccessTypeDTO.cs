using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class AccessTypeDTO
    {
        public string Name { get; set; }
        public int ItSystemId { get; set; }
        public virtual ItSystemDTO ItSystem { get; set; }
        public virtual ICollection<ItSystemUsageDTO> ItSystemUsages { get; set; }
    }
}