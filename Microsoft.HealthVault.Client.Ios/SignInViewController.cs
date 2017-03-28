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

        public SignInViewController(ISignInNavigationHandler navigationHandler, string startUrlString) :
            base("SignInViewController", null)
        {
            // Cancel and web view navigation is handeld by ISignInNavigationHandler
            this.navigationHandler = navigationHandler;

            this.startUrlString = startUrlString;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            this.LoadWebView(this.startUrlString);
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        private void LoadWebView(string url)
        {
            if (this.webView == null)
            {
                WKUserContentController contentController = new WKUserContentController();
                WKWebViewConfiguration configuration = new WKWebViewConfiguration();
                configuration.UserContentController = contentController;
                this.webView = new WKWebView(this.View.Bounds, configuration);
                this.webView.NavigationDelegate = this.navigationHandler;

                // Add the web view to the view
                this.View.AddSubview(this.webView);

                // Set up contraints
                this.webView.TranslatesAutoresizingMaskIntoConstraints = false;
                var subviews = NSDictionary.FromObjectAndKey(this.webView, new NSString(webViewKey));
                this.View.AddConstraints(NSLayoutConstraint.FromVisualFormat("|[" + webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(), subviews));
                this.View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-44-[" + webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(), subviews));

                this.webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
            }
        }

        partial void CancelButtonPressed()
        {
            this.navigationHandler.SignInCancelled();
        }
    }
}