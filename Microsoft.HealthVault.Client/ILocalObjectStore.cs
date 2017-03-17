using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Reads and writes simple objects to encrypted local storage.
    /// </summary>
    internal interface ILocalObjectStore
    {
        /// <summary>
        /// Reads an object from local storage.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key the object is stored under.</param>
        /// <returns>The stored object, or null if it was not found.</returns>
        Task<T> ReadAsync<T>(string key);

        /// <summary>
        /// Writes an object to local storage.
        /// </summary>
        /// <param name="key">The key the object is to be stored under.</param>
        /// <param name="value">The value to store.</param>
        Task WriteAsync(string key, object value);

        /// <summary>
        /// Deletes an object from local storage.
        /// </summary>
        /// <param name="key">The key the object is stored under.</param>
        Task DeleteAsync(string key);
    }
}