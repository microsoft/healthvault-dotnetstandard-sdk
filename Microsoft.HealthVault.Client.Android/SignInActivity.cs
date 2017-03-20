using System;
using Android.App;
using Android.OS;
using Android.Webkit;

namespace Microsoft.HealthVault.Client.Platform.Android
{
    /// <summary>
    /// This activity manages the MSA sign in process for the HealthVault Xamarin Android SDK
    /// </summary>
    [Activity(Label = "SignInActivity")]
    public class SignInActivity : Activity
    {
        public const string StartUrl = "startUrl";
        public const string EndUrl = "endUrl";
        private WebView webView;
        private IAndroidBrowserAuthBroker authBroker;
        private bool successful;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SignInActivity);

            this.authBroker = Ioc.Get<IAndroidBrowserAuthBroker>();
            var extras = Intent.Extras;
            string startUrl = extras.GetString(StartUrl);
            string endUrl = extras.GetString(EndUrl);
            CookieManager.Instance.RemoveAllCookie();

            this.webView = FindViewById<WebView>(Resource.Id.SignInWebView);
            this.webView.Settings.JavaScriptEnabled = true;
            this.webView.SetWebViewClient(new SignInWebViewClient(this, endUrl));
            this.webView.LoadUrl(startUrl);
        }

        private void onLoadCompleted(Uri uri)
        {
            this.authBroker.OnLoginSucceeded(uri);
            this.successful = true;
            this.Finish();
        }

        public override void OnBackPressed()
        {
            if (this.webView.CanGoBack())
            {
                // TODO: we may need to add some cases that don't react appropriately to the back button
                this.webView.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        protected override void OnDestroy()
        {
            if (!successful)
            {
                OnLoadCanceled();
            }

            base.OnDestroy();
        }

        private void OnLoadCanceled()
        {
            this.authBroker.OnLoginFailed(new System.OperationCanceledException());
        }

        /// <summary>
        /// The client for examining URLs to identify when login was successful
        /// </summary>
        private class SignInWebViewClient : WebViewClient
        {
            private readonly SignInActivity activity;
            private readonly string endUrl;

            public SignInWebViewClient(SignInActivity activity, string endUrl)
            {
                this.activity = activity;
                this.endUrl = endUrl;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                if (url.Contains(endUrl))
                {
                    this.activity.onLoadCompleted(new Uri(url));
                }
            }
        }
    }
}