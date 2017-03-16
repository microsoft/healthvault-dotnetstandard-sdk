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
        internal const string CorrelationIdContextKey = "WC_CorrelationId";

        // Formatting

        #region Format constants
        internal const string HmacFormat = "{0}&{1}&{2}&{3}&{4}&{5}";
        internal const string AuthorizationHeaderElement = "{0}={1}";
        internal const string MSHSDKVersion = "MSH-NET/{0} ({1})";
        internal const string MSHV1HeaderFormat = "MSH-V1 {0}";
        internal const string V1HMACSHA256Format = "V1-HMACSHA256 {0}";
        #endregion

        // Authorization header elements

        #region Authorization Header Constants

        /// <summary>
        /// Record Id entry in the Authorization Header
        /// </summary>
        public const string RecordId = "record-id";

        /// <summary>
        /// Offline Person ID entry in the Authorization Header
        /// </summary>
        public const string OfflinePersonId = "offline-person-id";

        /// <summary>
        /// App Token entry in the Authorization Header
        /// </summary>
        public const string AppToken = "app-token";

        /// <summary>
        /// User Token entry in the Authorization Header
        /// </summary>
        public const string UserToken = "user-token";

        #endregion

        // Headers

        #region Header constants
        
        /// <summary>
        /// Hmac Custom Header Name
        /// </summary>
        public const string HmacHeaderName = "x-msh-hmac";

        /// <summary>
        /// Sha256 Custom Header Name
        /// </summary>
        public const string Sha256HeaderName = "x-msh-sha256";

        /// <summary>
        /// Correlation Id Custom Header Name
        /// </summary>
        public const string CorrelationIdHeaderName = "x-msh-correlation-id";

        /// <summary>
        /// standard authorization header Name
        /// </summary>
        public const string AuthorizationHeaderName = "Authorization";
        #endregion

        /// <summary>
        /// Content Type for Json
        /// </summary>
        public const string JsonContentType = "application/json; charset=utf-8";

        /// <summary>
        /// MSHHV default root
        /// </summary>
        public const string DefaultMshhvRoot = "https://api.microsofthealth.net";
    }
}
