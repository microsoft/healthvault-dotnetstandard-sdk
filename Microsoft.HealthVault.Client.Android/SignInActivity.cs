// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Android.App;
using Android.OS;
using Android.Webkit;
using Microsoft.HealthVault.Client.Core;

namespace Microsoft.HealthVault.Client.Platform.Android
{
    /// <summary>
    /// This activity manages the MSA sign in process for the HealthVault Xamarin Android SDK
    /// </summary>
    [Activity]
    public class SignInActivity : Activity
    {
        public const string StartUrl = "startUrl";
        public const string EndUrl = "endUrl";
        private WebView _webView;
        private IAndroidBrowserAuthBroker _authBroker;
        private bool _isComplete;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = ClientResources.SignInActivityTitle;

            SetContentView(Resource.Layout.SignInActivity);

            _authBroker = Ioc.Get<IAndroidBrowserAuthBroker>();
            var extras = Intent.Extras;
            string startUrl = extras.GetString(StartUrl);
            string endUrl = extras.GetString(EndUrl);
            CookieManager.Instance.RemoveAllCookie();

            _webView = FindViewById<WebView>(Resource.Id.SignInWebView);
            _webView.Settings.JavaScriptEnabled = true;
            _webView.SetWebViewClient(new SignInWebViewClient(this, endUrl));
            _webView.LoadUrl(startUrl);
        }

        private void onLoadCompleted(Uri uri)
        {
            if (!_isComplete)
            {
                _authBroker.OnLoginSucceeded(uri);
                _isComplete = true;
                Finish();
            }
        }

        public override void OnBackPressed()
        {
            if (_webView.CanGoBack())
            {
                // TODO: we may need to add some cases that don't react appropriately to the back button
                _webView.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnDestroy()
        {
            if (!_isComplete)
            {
                OnLoadCanceled();
            }

            base.OnDestroy();
        }

        private void OnLoadCanceled()
        {
            _isComplete = true;
            _authBroker.OnLoginFailed(new System.OperationCanceledException());
        }

        /// <summary>
        /// The client for examining URLs to identify when login was successful
        /// </summary>
        private class SignInWebViewClient : WebViewClient
        {
            private readonly SignInActivity _activity;
            private readonly string _endUrl;

            public SignInWebViewClient(SignInActivity activity, string endUrl)
            {
                _activity = activity;
                _endUrl = endUrl;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                if (url.Contains(_endUrl))
                {
                    _activity.onLoadCompleted(new Uri(url));
                }
            }
        }
    }
}