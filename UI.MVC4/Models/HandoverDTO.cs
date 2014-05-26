﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI.MVC4.Filters;

namespace UI.MVC4.Models
{
    public class HandoverDTO
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? MeetingDate { get; set; }
        public string Summary { get; set; }
        public virtual IEnumerable<UserDTO> Participants { get; set; }
    }
}