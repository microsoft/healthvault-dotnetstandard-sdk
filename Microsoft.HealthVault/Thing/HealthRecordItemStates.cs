// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// The states of the health record item to search for.
    /// </summary>
    ///
    [Flags]
    public enum HealthRecordItemStates
    {
        /// <summary>
        /// Health record items with state
        /// <see cref="HealthRecordItemState.Active"/>
        /// are retrieved.
        /// </summary>
        ///
        Active = 0x1,

        /// <summary>
        /// Health record items with state
        /// <see cref="HealthRecordItemState.Deleted"/>
        /// are retrieved.
        /// </summary>
        ///
        Deleted = 0x2,

        /// <summary>
        /// Health record items with state
        /// <see cref="HealthRecordItemState.Active"/> are
        /// retrieved by default.
        /// </summary>
        ///
        Default = Active,

        /// <summary>
        /// Health record items with any state will be retrieved.
        /// </summary>
        ///
        Any = Active | Deleted
    }
}
