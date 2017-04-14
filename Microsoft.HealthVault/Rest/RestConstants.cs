// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Rest
{
    /// <summary>
    /// Constants used for Rest calls to the REST HealthVault endpoint.
    /// </summary>
    internal static class RestConstants
    {
        public const string MSHSDKVersion = "MSH-NET/{0} ({1})";

        /// <summary>
        /// Correlation Id Custom Header Name
        /// </summary>
        public const string CorrelationIdHeaderName = "x-msh-correlation-id";

        /// <summary>
        /// The version header
        /// </summary>
        public const string VersionHeader = "x-ms-version";

        /// <summary>
        /// Content Type for Json
        /// </summary>
        public const string JsonContentType = "application/json";

        /// <summary>
        /// Offline Person ID entry in the Authorization Header
        /// </summary>
        public const string OfflinePersonId = "offline-person-id";
    }
}
