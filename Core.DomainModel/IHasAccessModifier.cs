﻿namespace Core.DomainModel
{
    public enum AccessModifier
    {
        /// <summary>
        /// Only users within the organization this instance was created in have read access.
        /// </summary>
        Local,
        /// <summary>
        /// Everyone have read access.
        /// </summary>
        Public
    }

    /// <summary>
    /// Implemented by types whose read access can be modified
    /// </summary>
    public interface IHasAccessModifier
    {
        AccessModifier AccessModifier { get; set; }
    }
}
