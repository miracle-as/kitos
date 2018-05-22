using Core.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class AttachedOptionDTO
    {
        public int ObjectId { get; set; }
        public EntityType ObjectType { get; set; }
        public int OptionId { get; set; }
        public OptionType OptionType { get; set; }
    }
}