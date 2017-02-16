// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Represents the state of the <see cref="HealthRecordItem"/>.
    /// </summary>
    ///
    public enum HealthRecordItemState
    {
        /// <summary>
        /// The record item state returned from the server is not understood
        /// by this client.
        /// </summary>
        ///
        Unknown = 0,

        /// <summary>
        /// The health record item is active.
        /// </summary>
        ///
        /// <remarks>
        /// Active health record items are retrieved by default and can be
        /// updated.
        /// </remarks>
        ///
        Active = 1,

        /// <summary>
        /// The health record item is deleted.
        /// </summary>
        ///
        /// <remarks>
        /// Deleted health record items are retrieved when specified in
        /// <see cref="HealthRecordFilter.States"/>.
        /// Deleted health record items are useful to view for auditing
        /// purposes and cannot be updated.
        /// </remarks>
        ///
        Deleted = 2,
    }
}
