// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class TestSecretStore : ISecretStore
    {
        private const string AppInstanceId = "943d8da7-5c92-4845-a5f7-cea183a5e7c8";

        private const string AppSharedSecret = "LNrYbHdnu3NPv5o/4GJBRf2n8uHjCeJBYpTo0ZAVJPE=";

        private const string AppCreationToken = "AiAAAJGrS8ppiNxBphT+Cg4SdCOafsRjmEFoU78yO7VPICQVRy4GwgDx7rJrtc8eQuJcn+AAAADK7NzsExeGj6S8nyj+sQK40Pif2TvbXZmTBV3Tk5MeRQIH9TMdV7IV1fZRzApjJ7zKcs7zXFZgd6b/YTxfzb7IVstzqXMrcMqr8n3KverjP8cPi2ZwCbU08EZEBac/RI4g8G+ss7vH7uipz7k1ZWYjyhTc+JRt4nwRXppQipIpTjr5T0qXzCTUXpinF++xJBxhKFQR1PZy7HFQ6B3XNMtG7xEL3ACCB7dGcF7nPZ/yMJ2RgUQvBm6GbzS8VCr992LqcRmfK6o4+QEqKLopoDfOqMZ515O/wEh3E7Rc6AbJCSAAAADhwUisfSm2qDKA5mfHCvz/GBsy8LOp+/tXbLr5HBEjuCAAAADhwUisfSm2qDKA5mfHCvz/GBsy8LOp+/tXbLr5HBEjuA==";

        private const string SessionToken = "AiAAACaKMRRu6IpOt2QrkZpYA3l9iKqJcf+4g/sB+/3GCGvyN4+qALnxiBnufKpWRCEtO8AAAADeNMZt3Baizn3PYGZd5rZf+ffosSxuawvw7wZ/6zRGUtShoJMFHzLUFBM2In+bJGyQfLfVfgLsK3QXtG8FL2Kmrg+A+9HF59rb4WxHbFM0OFFQNeCN1j0nbDZg0WfAuZ8vFotJZq5ZCbz34HZGJbfewiMFXEb64kY4rtM+N5uuOxNxeLcenbXZNG1uCnCJRF6kOyn3fVQljreF6JtQ04jrBW/USuCqXzfMjJ1wWRIG7d/KE/ISR22UFDmkqn+LKP8gAAAAFLxz4ecK3d10vcfEsiLFUxo1eXrUFrTugXaZ7lgNPnogAAAAFLxz4ecK3d10vcfEsiLFUxo1eXrUFrTugXaZ7lgNPno=";

        private const string SessionSharedSecret = "S11LeH7yaU9xvTyLwYvqAYb8p80rJn/c02sqmnDVxeWWqJxXODMxIWlq2E4ljOfWgMg+MGOijJLX/aT5v71unA==";

        private const string PersonId = "ac921c3a-d231-4626-88a2-23c3e48563b1";

        private const string RecordId = "2d4e32e6-9511-42b3-8ac2-5f6524b305a2";

        public async Task WriteAsync(string key, string contents)
        {
        }

        public async Task<string> ReadAsync(string key)
        {
            switch (key)
            {
                case HealthVaultSodaConnection.ServiceInstanceKey:
                    return "{\"Id\":\"1\",\"Name\":\"US\",\"Description\":\"US instance\",\"HealthServiceUrl\":\"https://platform.hvazads03.healthvault-test.com/platform/wildcat.ashx\",\"ShellUrl\":\"https://account.hvazads03.healthvault-test.com/\"}";
                case HealthVaultSodaConnection.ApplicationCreationInfoKey:
                    return "{\"AppInstanceId\":\"" + AppInstanceId + "\",\"SharedSecret\":\"" + AppSharedSecret + "\",\"AppCreationToken\":\"" + AppCreationToken + "\"}";
                case HealthVaultSodaConnection.SessionCredentialKey:
                    return "{\"Token\":\"" + SessionToken + "\",\"SharedSecret\":\"" + SessionSharedSecret + "\"}";
                case HealthVaultSodaConnection.PersonInfoKey:
                    return "{\"PersonId\":\"" + PersonId + "\",\"Name\":\"HealthVault SDK Integration Test\",\"ApplicationSettingsDocument\":null,\"SelectedRecord\":{\"Id\":\"" + RecordId + "\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"},\"AuthorizedRecords\":{\"c4059111-0f99-4fa6-a899-38ce78c3fad0\":{\"Id\":\"c4059111-0f99-4fa6-a899-38ce78c3fad0\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"}},\"PreferredCulture\":\"en-US\",\"PreferredUICulture\":\"en-US\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"HasMoreRecords\":false,\"HasMoreApplicationSettings\":false}";
            }

            return string.Empty;
        }

        public async Task DeleteAsync(string key)
        {
        }
    }
}
