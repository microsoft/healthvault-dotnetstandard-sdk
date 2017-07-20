// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Foundation;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Security;
using UIKit;
using WebKit;

namespace Microsoft.HealthVault.Client
{
    internal class IosBrowserAuthBroker : NSObject, IBrowserAuthBroker, ISignInNavigationHandler
    {
        private TaskCompletionSource<Uri> _loginCompletionSource;
        private readonly AsyncLock _asyncLock = new AsyncLock();
        private readonly object _taskLockObject = new object();
        private SignInViewController _signInViewController;
        private bool _isTaskComplete;
        private string _endUrlString;
        private readonly HashSet<string> _browserLinks = new HashSet<string>
        {
            "https://account.healthvault.com/help.aspx?clcid=0x409",
            "https://healthvault.uservoice.com/forums/561754-healthvault-for-consumers?clcid=0x409&mobile=false",
            "https://go.microsoft.com/fwlink/?LinkId=521839&clcid=0x409",
            "https://go.microsoft.com/fwlink/?LinkID=530144&clcid=0x409",
            "https://account.healthvault.com/help.aspx?topicid=CodeofConduct&clcid=0x409",
            "https://msdn.microsoft.com/healthvault",
            "https://login.live.com/gls.srf?urlID=WinLiveTermsOfUse&mkt=EN-US&vv=1600",
            "https://login.live.com/gls.srf?urlID=MSNPrivacyStatement&mkt=EN-US&vv=1600",
        };

        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            using (await _asyncLock.LockAsync().ConfigureAwait(false))
            {
                try
                {
                    _endUrlString = endUrl.AbsoluteUri;
                    _isTaskComplete = false;
                    _loginCompletionSource = new TaskCompletionSource<Uri>();

                    BeginInvokeOnMainThread(() =>
                    {
                        _signInViewController = new SignInViewController(this, startUrl.AbsoluteUri);
                        UIViewController rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;

                        rootViewController.PresentViewController(new UINavigationController(_signInViewController), true, null);
                    });

                    Uri loginUri = await _loginCompletionSource.Task.ConfigureAwait(false);

                    return loginUri;
                }
                finally
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        _signInViewController.DismissViewController(false, null);
                    });
                }
            }
        }

        private void SetTaskResult(Uri url, Exception ex)
        {
            lock (_taskLockObject)
            {
                if (_isTaskComplete == false)
                {
                    _isTaskComplete = true;

                    if (ex != null)
                    {
                        _loginCompletionSource.SetException(ex);
                    }
                    else
                    {
                        _loginCompletionSource.SetResult(url);
                    }
                }
            }
        }

        public void SignInCancelled()
        {
            SetTaskResult(null, new OperationCanceledException());
        }

        private void NavigationFailedWithError(NSError error)
        {
            if (error.Domain == NSError.NSUrlErrorDomain && error.Code == (int)NSUrlError.Cancelled)
            {
                // Double tapping a link in a webview will cause the first navigation to be cancelled. Ignore the cancellation of the first request.
                return;
            }

            SetTaskResult(null, new HealthServiceException(ClientResources.LoginError.FormatResource(error.Code)));
        }

        [Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            if (navigationResponse.Response.GetType() == typeof(NSHttpUrlResponse))
            {
                NSHttpUrlResponse response = (NSHttpUrlResponse)navigationResponse.Response;
                if (response.StatusCode >= 400)
                {
                    // The navigation request resulted in an error.
                    SetTaskResult(null, new HealthServiceException(ClientResources.LoginErrorWithCode.FormatResource(response.StatusCode)));

                    return;
                }

                string url = response.Url.AbsoluteString;

                if (url.Contains(_endUrlString))
                {
                    SetTaskResult(new Uri(url), null);
                }
            }

            decisionHandler(WKNavigationResponsePolicy.Allow);
        }

        [Export("webView:didReceiveAuthenticationChallenge:completionHandler:")]
        public void DidReceiveAuthenticationChallenge(WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
        {
            SecTrustResult result = challenge.ProtectionSpace.ServerSecTrust.Evaluate();

            if (result == SecTrustResult.Unspecified || result == SecTrustResult.Proceed)
            {
                completionHandler(NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, null);
            }
            else
            {
                completionHandler(NSUrlSessionAuthChallengeDisposition.CancelAuthenticationChallenge, null);
            }
        }

        [Export("webView:didFailProvisionalNavigation:withError:")]
        public void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NavigationFailedWithError(error);
        }

        [Export("webView:didFailNavigation:withError:")]
        public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            NavigationFailedWithError(error);
        }

        [Export("webViewWebContentProcessDidTerminate:")]
        public void ContentProcessDidTerminate(WKWebView webView)
        {
            SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            if (_browserLinks.Contains(navigationAction.Request.Url.AbsoluteString))
            {
                UIApplication.SharedApplication.OpenUrl(navigationAction.Request.Url);

                decisionHandler(WKNavigationActionPolicy.Cancel);
                return;
            }

            decisionHandler(WKNavigationActionPolicy.Allow);
        }
    }
}
