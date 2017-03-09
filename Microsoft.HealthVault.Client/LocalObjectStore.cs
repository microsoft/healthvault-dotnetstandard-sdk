using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal class LocalObjectStore : ILocalObjectStore
    {
        private readonly IFileStore fileStore;
        private readonly ILocalDataProtection dataProtection;

        public LocalObjectStore(IFileStore fileStore, ILocalDataProtection dataProtection)
        {
            this.fileStore = fileStore;
            this.dataProtection = dataProtection;
        }

        public async Task<T> ReadObjectAsync<T>(string key)
        {
            return default(T);
        }

        public async Task WriteObjectAsync<T>(string key, T value)
        {
            
        }

        public async Task ClearObjectAsync(string key)
        {
            
        }

        public async Task<T> ReadEncryptedObjectAsync<T>(string key)
        {
            return default(T);
        }

        public async Task WriteEncryptedObjectAsync<T>(string key, T value)
        {

        }
    }
}
