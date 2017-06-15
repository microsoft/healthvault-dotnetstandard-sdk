using Foundation;
using System;

using UIKit;
using WebKit;

namespace Microsoft.HealthVault.Client
{
    public partial class SignInViewController : UIViewController
    {
        private string _startUrlString;
        private ISignInNavigationHandler _navigationHandler;
        private const string s_webViewKey = "webView";
        private WKWebView _webView;

        public SignInViewController(ISignInNavigationHandler navigationHandler, string startUrlString) :
            base()
        {
            // Cancel and web view navigation is handeld by ISignInNavigationHandler
            _navigationHandler = navigationHandler;

            _startUrlString = startUrlString;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Add a cancel button to the navigation bar
            NavigationItem.SetLeftBarButtonItem(
                new UIBarButtonItem(UIBarButtonSystemItem.Cancel,
                (sender, args) =>
                {
                    _navigationHandler.SignInCancelled();
                }),
                false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LoadWebView(_startUrlString);
        }

        private void LoadWebView(string url)
        {
            if (_webView == null)
            {
                WKUserContentController contentController = new WKUserContentController();
                WKWebViewConfiguration configuration = new WKWebViewConfiguration();
                configuration.UserContentController = contentController;
                _webView = new WKWebView(View.Bounds, configuration);
                _webView.NavigationDelegate = _navigationHandler;

                // Add the web view to the view
                View.AddSubview(_webView);

                // Set up contraints
                _webView.TranslatesAutoresizingMaskIntoConstraints = false;
                var subviews = NSDictionary.FromObjectAndKey(_webView, new NSString(s_webViewKey));
                View.AddConstraints(NSLayoutConstraint.FromVisualFormat("|[" + s_webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(), subviews));
                View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[" + s_webViewKey + "]|", NSLayoutFormatOptions.DirectionLeadingToTrailing, new NSDictionary(), subviews));

                _webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
            }
        }
    }
}