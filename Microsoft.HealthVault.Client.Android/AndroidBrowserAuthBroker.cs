using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Client.Platform.Android;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Client
{

    internal class AndroidBrowserAuthBroker : IAndroidBrowserAuthBroker
    {
        private readonly TaskCompletionSource<Uri> loginCompletionSource = new TaskCompletionSource<Uri>();
        private Exception loginException;
        static readonly SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);

        public async Task<Uri> AuthenticateAsync(Uri startUrl, Uri endUrl)
        {
            // Wait here for any future threads until the current one is finished
            await semaphore.WaitAsync();

            Intent intent = new Intent(Android.App.Application.Context, typeof(SignInActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.ExcludeFromRecents);
            intent.PutExtra(SignInActivity.StartUrl, startUrl.AbsolutePath);
            intent.PutExtra(SignInActivity.EndUrl, endUrl.AbsolutePath);
            Android.App.Application.Context.StartActivity(intent);

            var task = this.loginCompletionSource.Task;

            Uri loginUri = await task;

            semaphore.Release();

            if (loginUri == null)
            {
                throw loginException ?? new HealthServiceException(ClientResources.LoginError);
            }
            return loginUri;
        }

        public void OnLoginSucceeded(Uri uri)
        {
            this.loginCompletionSource.SetResult(uri);
        }

        public void OnLoginFailed(Exception ex)
        {
            this.loginException = ex;
            this.loginCompletionSource.SetResult(null);
        }
    }
}