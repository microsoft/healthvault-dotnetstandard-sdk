// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
