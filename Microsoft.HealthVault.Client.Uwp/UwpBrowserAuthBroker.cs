using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Exceptions;
using Windows.Security.Authentication.Web;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// The Uwp implementation of IBrowserAuthBroker
    /// </summary>
    public class UwpBrowserAuthBroker : IBrowserAuthBroker
    {
        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix)
        {
            return await DispatcherUtilities.RunOnUIThreadAsync(async () =>
            {
                try
                {
                    WebAuthenticationResult authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUrl, stopUrlPrefix)
                        .AsTask()
                        .ConfigureAwait(false);

                    switch (authResult.ResponseStatus)
                    {
                        case WebAuthenticationStatus.Success:
                            return new Uri(authResult.ResponseData);

                        case WebAuthenticationStatus.UserCancel:
                            throw new OperationCanceledException();

                        case WebAuthenticationStatus.ErrorHttp:
                            throw new BrowserAuthException((int)authResult.ResponseErrorDetail);

                        default:
                            throw new BrowserAuthException(null);
                    }
                }
                catch (Exception)
                {
                    // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                    throw new BrowserAuthException(null);
                }
            }).ConfigureAwait(false);
        }
    }
}
