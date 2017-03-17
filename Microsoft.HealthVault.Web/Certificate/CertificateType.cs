// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Web.Certificate
{
    /// <summary>
    /// Supported certificate types.
    /// </summary>
    ///
    internal enum CertificateType
    {
        /// <summary>
        /// The ThingBase is not signed.
        /// </summary>
        ///
        None = 0,

        /// <summary>
        /// Unable to determine the type of the certificate used to sign the ThingBase.
        /// </summary>
        ///
        Unknown = 1,

        /// <summary>
        /// Matches <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/>.
        /// </summary>
        ///
        X509Certificate = 2
    }
}
