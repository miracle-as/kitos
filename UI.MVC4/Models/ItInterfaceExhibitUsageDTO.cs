﻿namespace UI.MVC4.Models
{
    public class ItInterfaceExhibitUsageDTO
    {
        public int ItSystemUsageId { get; set; }
        public int ItInterfaceExhibitId { get; set; }
        public int ItInterfaceExhibitItInterfaceId { get; set; }
        public string ItInterfaceExhibitItInterfaceName { get; set; }
        public int? ItContractId { get; set; }
        public bool IsWishedFor { get; set; }
        //public int ItInterfaceExhibitItSystemId { get; set; }
        public string ItInterfaceExhibitItSystemName { get; set; }
    }
}
