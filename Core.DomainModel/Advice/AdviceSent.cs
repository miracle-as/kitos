﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel.AdviceSent
{
    
    public class AdviceSent : Entity
    {
        public DateTime AdviceSentDate {get; set;}
        public int? AdviceId { get; set; }
        public virtual Advice.Advice Advice { get; set; }
    }
}