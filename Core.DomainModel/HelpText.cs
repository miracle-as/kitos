﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public class HelpText : Entity
    {
        public string Title { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
    }
}
