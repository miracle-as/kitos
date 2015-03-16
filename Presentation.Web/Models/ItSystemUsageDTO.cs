﻿using System.Collections.Generic;

namespace Presentation.Web.Models
{
    public class ItSystemUsageDTO
    {
        public int Id { get; set; }
        public bool IsStatusActive { get; set; }
        public string Note { get; set; }
        public string LocalSystemId { get; set; }
        public string Version { get; set; }
        public string EsdhRef { get; set; }
        public string CmdbRef { get; set; }
        public string DirectoryOrUrlRef { get; set; }
        public string LocalCallName { get; set; }

        public int? SensitiveDataTypeId { get; set; }
        public int? ArchiveTypeId { get; set; }
        
        public string ResponsibleOrgUnitName { get; set; }

        public int OrganizationId { get; set; }
        public OrganizationDTO Organization { get; set; }

        public bool MainContractIsActive { get; set; }
        
        public int ItSystemId { get; set; }
        public ItSystemDTO ItSystem { get; set; }
        
        public string ItSystemParentName { get; set; }

        public int? OverviewId { get; set; }
        public string OverviewItSystemName { get; set; }

        public IEnumerable<RightOutputDTO> Rights { get; set; }
        
        public IEnumerable<TaskRefDTO> TaskRefs { get; set; }

        public int InterfaceExhibitCount { get; set; }
        public int InterfaceUseCount { get; set; }
        public int ActiveInterfaceUseCount { get; set; }
        
        public IEnumerable<InterfaceUsageDTO> InterfaceUsages { get; set; }
        public IEnumerable<InterfaceExposureDTO> InterfaceExposures { get; set; }

        public IEnumerable<ItProjectDTO> ItProjects { get; set; }

        public int? MainContractId { get; set; }
        public IEnumerable<ItContractSystemDTO> Contracts { get; set; }

        public string ObjectOwnerName { get; set; }
        public string ObjectOwnerLastName { get; set; }
        public string ObjectOwnerFullName
        {
            get { return ObjectOwnerName + " " + ObjectOwnerLastName; }
        }
    }
}
