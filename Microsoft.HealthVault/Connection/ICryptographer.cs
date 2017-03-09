using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault.Connection
{
    // TODO: We need to clean up this interface.

    /// <summary>
    /// Not yet flushed
    /// </summary>
    internal interface ICryptographer
    {
        /// <summary>
        /// Gets the hash algorithm.
        /// </summary>
        /// <value>
        /// The hash algorithm.
        /// </value>
        string HashAlgorithm { get; }

        /// <summary>
        /// Gets the hmac algorithm.
        /// </summary>
        /// <value>
        /// The hmac algorithm.
        /// </value>
        string HmacAlgorithm { get; }

        /// <summary>
        /// Hmacs the specified key material.
        /// </summary>
        /// <param name="keyMaterial">The key material.</param>
        /// <param name="data">The data.</param>
        /// <returns>CryptoHmac</returns>
        CryptoData Hmac(string keyMaterial, byte[] data);

        /// <summary>
        /// Hashes the specified text.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>CryptoHash</returns>
        CryptoData Hash(byte[] data);
    }
}