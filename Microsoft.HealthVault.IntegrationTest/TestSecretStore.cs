using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class TestSecretStore : ISecretStore
    {
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
                    return "{\"AppInstanceId\":\"affaf9d1-c051-49a7-ac71-ca43db03d111\",\"SharedSecret\":\"nDwCaVSGqUlUYR9/c5hSTaswaZ7KR3E2y8czrd/llmU=\",\"AppCreationToken\":\"AiAAANaTbifA8mlKr0dGpOSpW7xyIj/RTx1Gf3U8m+AzycJ9VIfssiZfGILnxiRjzcFpQeAAAACN1KARiN15/5iXFfHerNHwEAEp/Y1VVsuXztTLKN9oK/JXu8J7TgJyIJjq/PdJiVuNprdW6fIDOpwtLAw2PUgaViH0uuxglGLFs6E7HoBD6TvpT0Yz3EPbEkcKZnL31GXzEtHGktONS9DaoXHBw811PtJe27+APVt80BPZYtUnRDhk26EG6F9IQcQ10L4VN0OZFzxduuF4a/axlQPHbIoMBMiq5owKD177PwZfKFEoHNYCebaSmpfrT0bCbtmzeEkjQ3CrRaDPdqKwXy2VfHkERE9uUgaeSTiEPsTv3X9tbSAAAAAa2F4VKj0cAS8UllqxEROqqUzS06SROjZjGiUDEsbR9SAAAAAa2F4VKj0cAS8UllqxEROqqUzS06SROjZjGiUDEsbR9Q==\"}";
                case HealthVaultSodaConnection.SessionCredentialKey:
                    return "{\"Token\":\"AiAAAMpPpOOlETRLu3ykAZg6BOQBXu23pclWRKZe9pF/0uiaOaWZeZn5zH0tqw2/FKgeFsAAAADFdfookgLAcOzVfS2awcqi/wzoMORmBPwkEpKJLaGzXeOXJX/cpEA1/jpBi6G++jWl6qJ2b6j/nEy4tNXhfqhT4vFRAf+hqQoEqG0Lrek6/VZ3kr2CWdHn2VCfyRQUQj/MtqG2uaGpGT+b2FAxVNkIaH6ypFQWa95iB4m6BZDSNLneJueHmrBjrFY3Akhgcc9tOQLz6FHoJd5kSKYQrm/lqR93hboAuu5t2lvuZNf3nnNG28BoxbgIxWRC/5x5nGggAAAAEJoaP58IIIAh35+Xg4em+laIKMwMtNgD+EJ+10mz/KogAAAAEJoaP58IIIAh35+Xg4em+laIKMwMtNgD+EJ+10mz/Ko=\",\"SharedSecret\":\"vY1QvPM4wawdddS38sUAZqgTwfQDJyoDkgz5W6tOSAHO9bLN7iM7mbQbXhwIj0aF+4HQkIGXVjc5KIK2GjepWg==\"}";
                case HealthVaultSodaConnection.PersonInfoKey:
                    return "{\"PersonId\":\"cf24b744-011a-46c9-87ed-72300d93159f\",\"Name\":\"HealthVault SDK Integration Test\",\"ApplicationSettingsDocument\":null,\"SelectedRecord\":{\"Id\":\"c4059111-0f99-4fa6-a899-38ce78c3fad0\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"},\"AuthorizedRecords\":{\"c4059111-0f99-4fa6-a899-38ce78c3fad0\":{\"Id\":\"c4059111-0f99-4fa6-a899-38ce78c3fad0\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"}},\"PreferredCulture\":\"en-US\",\"PreferredUICulture\":\"en-US\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"HasMoreRecords\":false,\"HasMoreApplicationSettings\":false}";
            }

            return string.Empty;
        }

        public async Task DeleteAsync(string key)
        {
        }
    }
}
