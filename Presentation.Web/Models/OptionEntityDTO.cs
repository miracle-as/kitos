using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
        public abstract class OptionEntityDTO<TReference>
        {
            public string Name { get; set; }
            public bool IsLocallyAvailable { get; set; }
            public bool IsObligatory { get; set; }
            public bool IsSuggestion { get; set; }
            public string Description { get; set; }
            public bool IsEnabled { get; set; }
            public int Priority { get; set; }
        }
}