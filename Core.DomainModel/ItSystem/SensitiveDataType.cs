﻿using System.Collections.Generic;

namespace Core.DomainModel.ItSystem
{
    public class SensitiveDataType : OptionHasChecked<ItSystemUsage.ItSystemUsage>, IOptionReference<ItSystemUsage.ItSystemUsage>
    {
        public virtual ICollection<ItSystemUsage.ItSystemUsage> References { get; set; } = new HashSet<ItSystemUsage.ItSystemUsage>();
    }
}
