// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Represents finalized Hash Message Authentication Code (HMAC) states
    /// that are sent via XML requests.
    /// </summary>
    ///
    /// <remarks>
    /// HealthVault verifies the hash against this object's digest.
    /// </remarks>
    ///
    public sealed class CryptoHmacFinalized : CryptoHashFinalized
    {
        #region ctor

        /// <summary>
        /// This class must be created with parameters.
        /// </summary>
        ///
        private CryptoHmacFinalized()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CryptoHmacFinalized"/>
        /// class with a known algorithm identifier and finalized
        /// Hash Message Authentication Code (HMAC) digest.
        /// </summary>
        ///
        /// <param name="algorithmName">
        /// A string representing the algorithm name.
        /// </param>
        ///
        /// <param name="digest">
        /// An array of bytes representing the HMAC digest.
        /// </param>
        ///
        public CryptoHmacFinalized(string algorithmName, byte[] digest)
            : base(algorithmName, digest)
        {
            this.DigestAlgorithmName = "hmac";
        }

        #endregion
    }
}
