// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Helper factory class to help cast any integer status code
    /// to an appropriate HealthServiceStatusCode enum value.
    /// </summary>
    ///
    internal static class HealthServiceStatusCodeManager
    {
        /// <summary>
        /// Helper factory method to help cast any integer status code
        /// to an appropriate HealthServiceStatusCode enum value.
        /// </summary>
        ///
        /// <param name="statusCodeId">
        /// the integer status code id
        /// to be converted  to a HealthServiceStatusCode enum value.
        /// </param>
        ///
        /// <returns>
        /// HealthVaultStatus code enum object appropriately initialized.
        /// </returns>
        ///
        internal static HealthServiceStatusCode GetStatusCode(int statusCodeId)
        {
            HealthServiceStatusCode statusCode = HealthServiceStatusCode.UnmappedError;

            // Update this when HealthServiceStatusCode enum gets new values
            if (statusCodeId < (int)HealthServiceStatusCode.Max)
            {
                statusCode = (HealthServiceStatusCode)statusCodeId;
            }

            return statusCode;
        }
    }
}
