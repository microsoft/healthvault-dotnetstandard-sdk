using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal interface IAndroidBrowserAuthBroker : IBrowserAuthBroker
    {
        Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl);
        void OnLoginSucceeded(Uri uri);
        void OnLoginFailed(Exception ex);
    }
}