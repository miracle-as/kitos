﻿using System;
using Core.DomainModel;
using Newtonsoft.Json;
using Presentation.Web.Filters;

namespace Presentation.Web.Models
{
    public class GoalDTO
    {
        public int Id { get; set; }

        public int GoalStatusId { get; set; }

        /// <summary>
        /// Human readable ID ("brugervendt noegle" in OIO)
        /// </summary>
        public string HumanReadableId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Note { get; set; }

        public int GoalTypeId { get; set; }
        public string GoalTypeName { get; set; }

        public bool Measurable { get; set; }
        
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? SubGoalDate1 { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? SubGoalDate2 { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? SubGoalDate3 { get; set; }

        public string SubGoal1 { get; set; }
        public string SubGoal2 { get; set; }
        public string SubGoal3 { get; set; }

        public string SubGoalRea1 { get; set; }
        public string SubGoalRea2 { get; set; }
        public string SubGoalRea3 { get; set; }

        public TrafficLight Status { get; set; }
    }
}