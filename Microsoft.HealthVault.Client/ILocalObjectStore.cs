using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Reads and writes simple objects to local storage.
    /// </summary>
    internal interface ILocalObjectStore
    {
        /// <summary>
        /// Reads an object from local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is stored under.</param>
        /// <returns>The stored object, or null if it was not found.</returns>
        Task<T> ReadObjectAsync<T>(string key);

        /// <summary>
        /// Writes an object to local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is to be stored under.</param>
        /// <param name="value">The value to store.</param>
        Task WriteObjectAsync<T>(string key, T value);

        Task ClearObjectAsync(string key);

        /// <summary>
        /// Reads and decrypts an object stored in local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is stored under.</param>
        /// <returns>The stored object, or null if it was not found.</returns>
        Task<T> ReadEncryptedObjectAsync<T>(string key);

        /// <summary>
        /// Writes an object to local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is to be stored under.</param>
        /// <param name="value">The value to store.</param>
        Task WriteEncryptedObjectAsync<T>(string key, T value);
    }
}