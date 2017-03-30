using System;

namespace Microsoft.HealthVault.Client
{
    internal interface IAndroidBrowserAuthBroker : IBrowserAuthBroker
    {
        void OnLoginSucceeded(Uri uri);

        void OnLoginFailed(Exception ex);
    }
}