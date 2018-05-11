﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class CreateUserDescriptorModel
    {
        public CreateUserModel User { get; set; }
        public int OrganizationId { get; set; }
        public bool SendMailOnCreation { get; set; }
    }
}