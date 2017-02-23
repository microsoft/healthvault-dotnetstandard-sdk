// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the algorithm used to protect a package of data with a password.
    /// </summary>
    ///
    public enum PasswordProtectAlgorithm
    {
        /// <summary>
        /// The algorithm name returned from the server was not understood
        /// by this client.
        /// </summary>
        ///
        /// <remarks>
        /// This can happen if new algorithms are supported by the server but
        /// the client has not been updated.
        /// </remarks>
        ///
        Unknown = 0,

        /// <summary>
        /// No encryption was used to protect the package.
        /// </summary>
        ///
        None = 1,

        /// <summary>
        /// The package is encrypted using the HMAC-SHA1 pseudo-random and
        /// 3DES encryption functions.
        /// </summary>
        ///
        HmacSha13Des = 2,

        /// <summary>
        /// The package is encrypted using the HMAC-SHA256 and AES256
        /// encryption functions.
        /// </summary>
        ///
        HmacSha256Aes256 = 3
    }
}
