using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client.Platform.Android
{
    /// <summary>
    /// An interface to retrieve encryption keys.
    /// </summary>
    internal interface IEncryptionKeyService
    {
        /// <summary>
        /// Gets the encryption key with the given. Returns null if the key does not exist.
        /// </summary>
        /// <param name="keyName">The name of the key</param>
        /// <returns></returns>
        Task<byte[]> GetEncryptionKeyAsync(string keyName);

        /// <summary>
        /// Creates an encryption key with the specified name and stores it in the key store
        /// </summary>
        /// <param name="keyName">The name of the key</param>
        /// <returns></returns>
        Task<byte[]> CreateAndStoreEncryptionKeyAsync(string keyName);

        /// <summary>
        /// Gets the encryption key with the provided name or creats and stores it if it does not exist. 
        /// </summary>
        /// <param name="keyName">The name of the key</param>
        /// <returns></returns>
        Task<byte[]> GetOrMakeEncryptionKeyAsync(string keyName);
    }
}