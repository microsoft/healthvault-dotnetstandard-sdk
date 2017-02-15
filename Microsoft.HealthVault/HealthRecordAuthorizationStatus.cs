// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// The record's current authorization status.
    /// </summary>
    /// <remarks>
    /// The members of HealthRecordAuthorizationStatus represent the current
    /// status of the application's authorization to the record.  Any status
    /// other than NoActionRequired, requires user intervention in HealthVault
    /// before the application may successfully access the record.
    /// </remarks>
    ///
    public enum HealthRecordAuthorizationStatus
    {
        /// <summary>
        /// An unknown state was returned from the server.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The record should be accessible to the application.
        /// </summary>
        NoActionRequired = 1,

        /// <summary>
        ///  The user must authorize this application.
        /// </summary>
        AuthorizationRequired = 2,

        /// <summary>
        /// It is not possible to reauthorize this application.
        /// </summary>
        ReauthorizationNotPossible = 3,

        /// <summary>
        /// The user must reauthorize this application.
        /// </summary>
        ReauthorizationRequired = 4,
    }
}
