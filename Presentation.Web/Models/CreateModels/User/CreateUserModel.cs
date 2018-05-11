using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class CreateUserModel
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsGlobalAdmin { get; set; }
    }
}