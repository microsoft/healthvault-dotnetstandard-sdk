using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.Client
{
    internal class LocalObjectStore : ILocalObjectStore
    {
        private readonly ISecretStore secretStore;

        public LocalObjectStore(ISecretStore secretStore)
        {
            this.secretStore = secretStore;
        }

        public async Task<T> ReadAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            var json = await this.secretStore.ReadAsync(key).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public async Task WriteAsync(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                throw new ArgumentException(Resources.ObjectStoreParametersEmpty);
            }

            var serializedObj = JsonConvert.SerializeObject(value);
            await this.secretStore.WriteAsync(key, serializedObj).ConfigureAwait(false);
        }

        public async Task DeleteAsync(string key)
        {
            await this.secretStore.DeleteAsync(key).ConfigureAwait(false);
        }
    }
}
