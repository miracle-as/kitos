using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentation.Web.Models
{
    public class TokenDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }

    }
}