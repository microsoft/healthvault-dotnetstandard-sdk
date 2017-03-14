// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The states of the thing to search for.
    /// </summary>
    ///
    [Flags]
    public enum ThingStates
    {
        /// <summary>
        /// things with state
        /// <see cref="ThingState.Active"/>
        /// are retrieved.
        /// </summary>
        ///
        Active = 0x1,

        /// <summary>
        /// things with state
        /// <see cref="ThingState.Deleted"/>
        /// are retrieved.
        /// </summary>
        ///
        Deleted = 0x2,

        /// <summary>
        /// things with state
        /// <see cref="ThingState.Active"/> are
        /// retrieved by default.
        /// </summary>
        ///
        Default = Active,

        /// <summary>
        /// things with any state will be retrieved.
        /// </summary>
        ///
        Any = Active | Deleted
    }
}
