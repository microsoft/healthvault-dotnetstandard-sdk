using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.HealthVault.Exceptions;
using UIKit;
using WebKit;
using Security;

namespace Microsoft.HealthVault.Client
{
    internal class IosBrowserAuthBroker : NSObject, IBrowserAuthBroker, ISignInNavigationHandler
    {
        private TaskCompletionSource<Uri> loginCompletionSource;
        private readonly AsyncLock asyncLock = new AsyncLock();
        private readonly object taskLockObject = new object();
        private SignInViewController signInViewController;
        private bool isTaskComplete;
        private string endUrlString;

        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            using (await asyncLock.LockAsync().ConfigureAwait(false))
            {
                try
                {
                    this.endUrlString = endUrl.AbsoluteUri;
                    this.isTaskComplete = false;
                    this.loginCompletionSource = new TaskCompletionSource<Uri>();

                    BeginInvokeOnMainThread(() =>
                    {
                        signInViewController = new SignInViewController(this, startUrl.AbsoluteUri);
                        IUIApplicationDelegate appDelegate = UIApplication.SharedApplication.Delegate;
                        UIViewController rootViewController = appDelegate.GetWindow().RootViewController;

                        rootViewController.PresentViewController(signInViewController, true, null);
                    });

                    Uri loginUri = await this.loginCompletionSource.Task.ConfigureAwait(false);

                    return loginUri;
                }
                finally
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        signInViewController.DismissViewController(false, null);
                    });
                }
            } 
        }

        private void SetTaskResult(Uri url, Exception ex)
        {
            lock(taskLockObject)
            {
                if (this.isTaskComplete == false)
                {
                    this.isTaskComplete = true;

                    if (ex != null)
                    {
                        this.loginCompletionSource.SetException(ex);
                    }
                    else
                    {
                        this.loginCompletionSource.SetResult(url);
                    }
                }
            }
        }

        public void SignInCancelled()
        {
            this.SetTaskResult(null, new OperationCanceledException());
        }

        private void NavigationFailedWithError(NSError error)
        {
            if (error.Domain == NSError.NSUrlErrorDomain && error.Code == (int)NSUrlError.Cancelled)
            {
                // Double tapping a link in a webview will cause the first navigation to be cancelled. Ignore the cancellation of the first request.
                return;
            }

            this.SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));
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
                    this.SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));

                    return;
                }

                string url = response.Url.AbsoluteString;

                if (url.Contains(this.endUrlString))
                {
                    this.SetTaskResult(new Uri(url), null);
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
            this.NavigationFailedWithError(error);
        }

        [Export("webView:didFailNavigation:withError:")]
        public void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            this.NavigationFailedWithError(error);
        }

        [Export("webViewWebContentProcessDidTerminate:")]
        public void ContentProcessDidTerminate(WKWebView webView)
        {
            this.SetTaskResult(null, new HealthServiceException(ClientResources.LoginError));
        }

    }
}
