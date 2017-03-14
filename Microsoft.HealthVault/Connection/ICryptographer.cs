namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Not yet flushed
    /// </summary>
    internal interface ICryptographer
    {
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