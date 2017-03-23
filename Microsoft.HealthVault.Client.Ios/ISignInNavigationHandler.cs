using System;
using WebKit;

namespace Microsoft.HealthVault.Client
{
    public interface ISignInNavigationHandler : IWKNavigationDelegate
    {
        void SignInCancelled();
    }
}
