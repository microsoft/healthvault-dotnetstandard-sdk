// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Enumeration of the application record authorization action codes.
    /// </summary>
    /// 
    public enum ApplicationRecordAuthorizationAction
    {
        /// <summary>
        /// The server returned a value that is not understood by this client.
        /// </summary>
        /// 
        Unknown = 0,

        /// <summary>
        /// The application has never been authorized.
        /// </summary>
        /// 
        AuthorizationRequired = 1,

        /// <summary>
        /// The application both requires and can do re-authorization.
        /// </summary>
        /// 
        ReauthorizationRequired = 2,

        /// <summary>
        /// The application requires but cannot do re-authorization.
        /// </summary>
        /// 
        ReauthorizationNotPossible = 3,

        /// <summary>
        /// There are no actions required.
        /// </summary>
        /// 
        NoActionRequired = 4,

        /// <summary>
        /// The record location isn't supported by the application.
        /// </summary>
        RecordLocationNotSupported = 5,
    } 
}

