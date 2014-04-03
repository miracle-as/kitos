﻿using System;

namespace UI.MVC4.Models
{
    public class TaskRefDTO
    {
        public int Id { get; set; }
        public Guid Uuid { get; set; }
        public string Type { get; set; }
        public string TaskKey { get; set; }
        public string Description { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; } 
    }
}