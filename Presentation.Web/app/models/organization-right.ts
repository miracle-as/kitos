module Kitos.Models {
    /** Represents an Organization (such as a municipality, or a company).Holds local configuration and admin roles, as well as collections ofItSystems, ItProjects, etc that was created in this organization. */
    export interface IOrganizationRight extends IEntity {
        Name: string;
        Role: OrganizationRole;
    }
}
