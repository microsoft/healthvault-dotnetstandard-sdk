using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Client
{
    internal class LocalObjectStore : ILocalObjectStore
    {
        private readonly ISecretStore _secretStore;

        public LocalObjectStore(ISecretStore secretStore)
        {
            _secretStore = secretStore;
        }

        public async Task<T> ReadAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            var json = await _secretStore.ReadAsync(key).ConfigureAwait(false);
            if (json == null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task WriteAsync(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            var serializedObj = JsonConvert.SerializeObject(value);
            await _secretStore.WriteAsync(key, serializedObj).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string key)
        {
            await _secretStore.DeleteAsync(key).ConfigureAwait(false);
        }
    }
}
