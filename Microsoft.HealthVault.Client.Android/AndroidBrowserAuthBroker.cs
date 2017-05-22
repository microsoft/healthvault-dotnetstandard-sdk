using System;
using System.Threading.Tasks;
using Android.Content;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Client.Platform.Android;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Client
{
    internal class AndroidBrowserAuthBroker : IAndroidBrowserAuthBroker
    {
        private TaskCompletionSource<Uri> _loginCompletionSource;
        private Exception _loginException;
        private static readonly AsyncLock s_asyncLock = new AsyncLock();

        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            // Wait here for any future threads until the current one is finished
            using (await s_asyncLock.LockAsync().ConfigureAwait(false))
            {
                Intent intent = new Intent(Android.App.Application.Context, typeof(SignInActivity));
                intent.AddFlags(ActivityFlags.NewTask);
                intent.AddFlags(ActivityFlags.ExcludeFromRecents);
                intent.PutExtra(SignInActivity.StartUrl, startUrl.AbsoluteUri);
                intent.PutExtra(SignInActivity.EndUrl, endUrl.AbsoluteUri);
                Android.App.Application.Context.StartActivity(intent);

                _loginCompletionSource = new TaskCompletionSource<Uri>();

                Uri loginUri = await _loginCompletionSource.Task.ConfigureAwait(false);

                if (loginUri == null)
                {
                    throw _loginException ?? new HealthServiceException(ClientResources.LoginError);
                }
                return loginUri;
            }
        }

        public void OnLoginSucceeded(Uri uri)
        {
            _loginCompletionSource.SetResult(uri);
        }

        public void OnLoginFailed(Exception ex)
        {
            _loginException = ex;
            _loginCompletionSource.SetResult(null);
        }
    }
}