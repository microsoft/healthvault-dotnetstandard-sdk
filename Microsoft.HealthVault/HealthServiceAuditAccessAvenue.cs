// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// The avenue used to perform the operation.
    /// </summary>
    ///
    public enum HealthServiceAuditAccessAvenue
    {
        /// <summary>
        /// The access avenue returned from the server is not understood by
        /// this client.
        /// </summary>
        ///
        Unknown = 0,

        /// <summary>
        /// Online access avenue was used to perform the operation.
        /// </summary>
        ///
        Online = 1,

        /// <summary>
        /// Offline access avenue was used to perform the operation.
        /// </summary>
        ///
        Offline = 2
    }
}
