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
                    return "{\"Id\":\"1\",\"Name\":\"US\",\"Description\":\"US instance\",\"HealthServiceUrl\":\"https://platform.healthvault-ppe.com/platform/wildcat.ashx\",\"ShellUrl\":\"https://account.healthvault-ppe.com/\"}";
                case HealthVaultSodaConnection.ApplicationCreationInfoKey:
                    return "{\"AppInstanceId\":\"5826bd6d-83c3-47c0-a175-45021efa375c\",\"SharedSecret\":\"hcswCV9yt8emWbkhhDGwVf9fn5gLMVi59mlJcdMlRIc=\",\"AppCreationToken\":\"AiAAAInaPSXoVPxDhpEZ5hjfVSKEO/yul8XNEbuFLGN1MQNBNUJgnQXoAJg6Sfcs19mCh+AAAAB65CiTZpUOzz17g52KdDl3hpC+ee4YAKNa3KhuBa0O274G8IsDgsN8MSN3ERSOU3swmK2pVXmTuMeK4Vg+WzpFoRJTb8IH/L0pbXpdkucLs/TBubQgV+TYNqPcaRLEoT/TUWaU8HaKxcCwbYkpKCHa1H+pXf3QQOT/N0G8liQ+jkV9sTO0kmUtz1il+Kn+GE+Gx5AqOBDsv5bopT/Gy5z2mOR+BD4MyPU9gQvT/qShlRWP0EKPulhPYyrw6vOR1kwEgoU9ZBtF+nYSj05pxEoRLrtJW370/9R+Jb3RLj5gQSAAAACyAdz0mWrls9tlvQG1RvNcy5ShEe0jdIQjb4uq+m74iiAAAACyAdz0mWrls9tlvQG1RvNcy5ShEe0jdIQjb4uq+m74ig==\"}";
                case HealthVaultSodaConnection.SessionCredentialKey:
                    return "{\"Token\":\"AiAAAML47WGYEfRCjJD4xSaFJiBhsF8rrZxJVgQ+lR11SBP9nOu+5hiw4d2+bYeAUV6eIsAAAABU1UNVEv6ybtQXYgathUG9A5HREp1leHiKz8/j7056vHxJirzGzTXDJO4ERw+PPmRxhfff2gj+nwVqjZewSBEqldj3Rn+2Y+RH8/B3HSIZgY4atOO0pk6qZ6aD5xUhBuUHh0AMjAGI6nO13APNlLccbWm/R7bs+v5Pd/kCA0bSgZHjkwc87eXeSTKFTZNcK2e6hKjQzjjyZZOQY+THuox5WtWfa0QLgGWC8BWZn2Qw8eja/gfvn+2zkWyEPhXCA5YgAAAAHPL6pAxyoeJ39/RCNhWxaPgPSFHVPnEf650+rtekN4kgAAAAHPL6pAxyoeJ39/RCNhWxaPgPSFHVPnEf650+rtekN4k=\",\"SharedSecret\":\"XuGrduL48cndUwsOnnnK1EF8QIQtBouHI9Klqogl22boPk1SWVmvZ/959CPIWxqkzhAX6W5IHZ2qE82C+9BmMg==\"}";
                case HealthVaultSodaConnection.PersonInfoKey:
                    return "{\"PersonId\":\"656bfbce-983d-4454-a643-188854215772\",\"Name\":\"HealthVault SDK Integration Test\",\"ApplicationSettingsDocument\":null,\"SelectedRecord\":{\"Id\":\"ea5b4b21-b2de-4da6-bf5a-3e551cd0c54e\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"},\"AuthorizedRecords\":{\"ea5b4b21-b2de-4da6-bf5a-3e551cd0c54e\":{\"Id\":\"ea5b4b21-b2de-4da6-bf5a-3e551cd0c54e\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"IsCustodian\":true,\"DateAuthorizationExpires\":\"9999-12-31T23:59:59.999Z\",\"HasAuthorizationExpired\":false,\"Name\":\"HealthVault SDK Integration Test\",\"RelationshipType\":1,\"RelationshipName\":\"Self\",\"DisplayName\":\"HealthVault SDK\",\"State\":1,\"DateCreated\":\"2017-04-26T15:52:59.783Z\",\"DateUpdated\":\"2017-04-26T15:53:11.517Z\",\"QuotaInBytes\":4294967296,\"QuotaUsedInBytes\":2751,\"LatestOperationSequenceNumber\":6,\"HealthRecordAuthorizationStatus\":1,\"ApplicationSpecificRecordId\":\"782456\",\"RecordAppAuthCreatedDate\":\"2017-04-26T15:53:11.517Z\"}},\"PreferredCulture\":\"en-US\",\"PreferredUICulture\":\"en-US\",\"Location\":{\"Country\":\"US\",\"StateProvince\":null},\"HasMoreRecords\":false,\"HasMoreApplicationSettings\":false}";
            }

            return string.Empty;
        }

        public async Task DeleteAsync(string key)
        {
        }
    }
}
