using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault.Connection
{
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
        /// <param name="text">The text.</param>
        /// <returns>CryptoHmac</returns>
        CryptoHmac Hmac(string keyMaterial, string text);

        /// <summary>
        /// Hashes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>CryptoHash</returns>
        CryptoHash Hash(string text);

        /// <summary>
        /// Generates the info hash section for HealthVault service
        /// web requests given the specified data beginning at the specified
        /// index.
        /// </summary>
        ///
        /// <param name="buffer">
        /// An array of bytes representing the UTF8 encoded data.
        /// </param>
        ///
        /// <param name="index">
        /// An integer representing the starting location in the byte array.
        /// </param>
        ///
        /// <param name="count">
        /// An integer representing the count of bytes.
        /// </param>
        ///
        /// <returns>
        /// A string representing the info hash.
        /// </returns>
        string CreateInfoHash(byte[] buffer, int index, int count);
    }
}