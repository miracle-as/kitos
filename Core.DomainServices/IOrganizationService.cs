﻿using System.Collections.Generic;
using Core.DomainModel;

namespace Core.DomainServices
{
    public interface IOrganizationService
    {
        ICollection<Organization> GetByUser(User user);
        Organization CreateOrganization(string name, OrganizationType organizationType, User objectOwner);

        bool IsUserMember(User user, Organization organization);
    }
}