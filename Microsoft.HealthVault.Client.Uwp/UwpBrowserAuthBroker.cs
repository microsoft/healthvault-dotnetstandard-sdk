// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Exceptions;
using Windows.Security.Authentication.Web;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// The Uwp implementation of IBrowserAuthBroker
    /// </summary>
    internal class UwpBrowserAuthBroker : IBrowserAuthBroker
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
