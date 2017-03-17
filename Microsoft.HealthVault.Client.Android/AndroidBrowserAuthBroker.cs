using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    public class AndroidBrowserAuthBroker : IBrowserAuthBroker
    {
        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            throw new NotImplementedException();
        }
    }
}