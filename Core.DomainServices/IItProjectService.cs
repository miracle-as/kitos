﻿using System.Collections.Generic;
using Core.DomainModel;
using Core.DomainModel.ItProject;

namespace Core.DomainServices
{
    public interface IItProjectService
    {
        ItProjectType ProgramType { get; }

        IEnumerable<ItProject> GetAll(Organization organization, string nameSearch);
        IEnumerable<ItProject> GetProjects(Organization organization, string nameSearch);
        IEnumerable<ItProject> GetPrograms(Organization organization, string nameSearch);

        /// <summary>
        /// Adds an IT project. It creates default phases and saves the project.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        ItProject AddProject(ItProject project);

        /// <summary>
        /// Clones and saves an IT project.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newOwner"></param>
        /// <param name="newOrgId"></param>
        /// <returns></returns>
        ItProject CloneProject(ItProject original, User newOwner, int newOrgId);
    }
}