﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel.ItSystem
{
    public class Frequency : IOptionEntity<DataRowUsage>
    {
        public Frequency()
        {
            this.References = new List<DataRowUsage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuggestion { get; set; }
        public string Note { get; set; }
        public virtual ICollection<DataRowUsage> References { get; set; }
    }
}