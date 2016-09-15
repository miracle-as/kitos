﻿module Kitos.Models {
    export interface IUser extends IEntity {
        Name?: string;
        LastName?: string;
        PhoneNumber?: string;
        Email?: string;
        IsGlobalAdmin?: boolean;
        Uuid?: any;
        LastAdvisDate?: Date;
        /** The admin rights of the user */
        OrganizationRights?: IOrganizationRight[];
        /** Passwords reset request issued for the user */
        PasswordResetRequests?: IPasswordResetRequest[];
        /** Wishes created by this user */
        Wishes?: ItSystem.IWish[];
        /** Gets or sets the  or  associated with this user */
        ItProjectStatuses?: ItProject.IItProjectStatus[];
        /** Risks associated with this user */
        ResponsibleForRisks?: ItProject.IRisk[];
        /** Communications associated with this user */
        ResponsibleForCommunications?: ItProject.ICommunication[];
        /** Handovers associated with this user */
        HandoverParticipants?: ItProject.IHandover[];
        /** The contracts that the user has been marked as contract signer for */
        SignerForContracts?: ItContract.IItContract[];
    }
}
