using Microsoft.HealthVault.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace Microsoft.HealthVault.Client.Platform.Uwp
{
    /// <summary>
    /// The Uwp implementation of IBrowserAuthBroker
    /// </summary>
    public class UwpBrowserAuthBroker : IBrowserAuthBroker
    {
        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri stopUrlPrefix)
        {
            try
            {
                WebAuthenticationResult authResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, startUrl, stopUrlPrefix);

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
        }
    }
}
