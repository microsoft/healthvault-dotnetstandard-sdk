using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal class LocalObjectStore : ILocalObjectStore
    {
        private readonly ISecretStore secretStore;
        private readonly ILocalDataProtection dataProtection;

        public LocalObjectStore(ISecretStore secretStore, ILocalDataProtection dataProtection)
        {
            this.secretStore = secretStore;
            this.dataProtection = dataProtection;
        }

        public async Task<T> ReadAsync<T>(string key)
        {
            return default(T);
        }

        public async Task WriteAsync(string key, object value)
        {
            
        }

        public async Task DeleteAsync(string key)
        {
            
        }
    }
}
