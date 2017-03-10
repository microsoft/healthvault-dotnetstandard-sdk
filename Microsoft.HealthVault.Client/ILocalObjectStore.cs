using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal interface ILocalObjectStore
    {
        Task<T> ReadObjectAsync<T>(string key);

        Task WriteObjectAsync<T>(string key, T value);

        Task ClearObjectAsync(string key);

        Task<T> ReadEncryptedObjectAsync<T>(string key);

        Task WriteEncryptedObjectAsync<T>(string key, T value);
    }
}