// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents the state of the <see cref="ThingBase"/>.
    /// </summary>
    ///
    public enum ThingState
    {
        /// <summary>
        /// The record item state returned from the server is not understood
        /// by this client.
        /// </summary>
        ///
        Unknown = 0,

        /// <summary>
        /// The thing is active.
        /// </summary>
        ///
        /// <remarks>
        /// Active things are retrieved by default and can be
        /// updated.
        /// </remarks>
        ///
        Active = 1,

        /// <summary>
        /// The thing is deleted.
        /// </summary>
        ///
        /// <remarks>
        /// Deleted things are retrieved when specified in
        /// <see cref="ThingQuery.States"/>.
        /// Deleted things are useful to view for auditing
        /// purposes and cannot be updated.
        /// </remarks>
        ///
        Deleted = 2
    }
}
