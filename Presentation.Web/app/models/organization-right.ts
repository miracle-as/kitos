module Kitos.Models {
    export interface IOrganizationRight {
        UserId?: number;
        User?: IUser;
        Name?: string;
        Role?: OrganizationRole;
        OrganizationId?: number;
        Organization?: IOrganization;
        DefaultOrgUnitId?: number;
        DefaultOrgUnit?: IOrganizationUnit;
    }
}
