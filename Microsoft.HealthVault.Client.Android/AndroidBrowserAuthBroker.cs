// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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