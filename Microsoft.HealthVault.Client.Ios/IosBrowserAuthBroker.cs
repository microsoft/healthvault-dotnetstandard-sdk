using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Exceptions;
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

            SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));
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
                    SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));

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
    }
}
