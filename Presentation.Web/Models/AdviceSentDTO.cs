using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class AdviceSentDTO
    {
        public DateTime AdviceSentDate { get; set; }
        public int? AdviceId { get; set; }
        public virtual AdviceDTO Advice { get; set; }
    }
}