using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class StubBrowserAuthBroker : IBrowserAuthBroker
    {
        public Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix)
        {
            return Task.FromResult(new Uri("http://contoso.com"));
        }
    }
}
