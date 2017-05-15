using System;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class Constants
    {
        public static readonly HealthVaultConfiguration Configuration = new HealthVaultConfiguration
        {
            MasterApplicationId = new Guid("0405874f-7b59-45ab-9410-8824dbef1f11"),
            DefaultHealthVaultShellUrl = new Uri("https://account.hvazads03.healthvault-test.com"),
            DefaultHealthVaultUrl = new Uri("https://platform.hvazads03.healthvault-test.com/platform"),
            RestHealthVaultUrl = new Uri("https://hvc-dev-khvwus01.westus2.cloudapp.azure.com/v3/")
        };
    }
}
