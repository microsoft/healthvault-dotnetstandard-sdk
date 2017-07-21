// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
                _webView.AllowsBackForwardNavigationGestures = true;

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