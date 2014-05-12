﻿using System.Collections.Generic;

namespace Core.DomainModel.ItSystem
{
    public class ArchiveType : IOptionEntity<ItSystemUsage>
    {
        public ArchiveType()
        {
            this.References = new List<ItSystemUsage>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuggestion { get; set; }
        public string Note { get; set; }
        public virtual ICollection<ItSystemUsage> References { get; set; }
    }
}