using Foundation;
using System;

using UIKit;
using WebKit;

namespace Microsoft.HealthVault.Client
{
    public partial class SignInViewController : UIViewController
    {
        private string startUrlString;
        private ISignInNavigationHandler navigationHandler;
        private const string webViewKey = "webView";
        private WKWebView webView;

        private WKWebView WebView
        {
            get
            {
                if (webView == null)
                {
                    WKUserContentController contentController = new WKUserContentController();
                    WKWebViewConfiguration configuration = new WKWebViewConfiguration();
                    configuration.UserContentController = contentController;
                    webView = new WKWebView(this.View.Bounds, configuration);
                }

                return webView;
            }
        }

        public SignInViewController(ISignInNavigationHandler navigationHandler, string startUrlString) : base("SignInViewController", null)
        {
            // Cancel and web view navigation is handeld by ISignInNavigationHandler
            this.navigationHandler = navigationHandler;
            this.WebView.NavigationDelegate = this.navigationHandler;

            this.startUrlString = startUrlString;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.AddWebView();

            this.WebView.LoadRequest(new NSUrlRequest(new NSUrl(this.startUrlString)));
        }

        private void AddWebView()
        {
            // Add the web view to the view
            this.View.AddSubview(this.WebView);

            // Set up contraints
            var subviews = NSDictionary.FromObjectAndKey(this.WebView, new NSString(webViewKey));
            this.View.AddConstraints(NSLayoutConstraint.FromVisualFormat("|[" + webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing));
            this.View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[" + webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing));
        }

        private void CancelButtonPressed(UIBarButtonItem sender)
        {
            this.navigationHandler.SignInCancelled();
        }
    }
}