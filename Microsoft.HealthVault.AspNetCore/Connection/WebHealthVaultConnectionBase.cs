// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.PlatformInformation;

namespace Microsoft.HealthVault.AspNetCore.Connection
{
    /// <summary>
    /// Provides common functionality for both Web and Offline Connection types
    /// </summary>
    /// <seealso cref="HealthVaultConnectionBase" />
    internal abstract class WebHealthVaultConnectionBase : HealthVaultConnectionBase
    {
        protected readonly HealthVaultConfiguration webHealthVaultConfiguration;

        protected WebHealthVaultConnectionBase(
            HealthServiceInstance healthServiceInstance = null,
            SessionCredential sessionCredential = null)
            : base(Ioc.Get<IServiceLocator>())
        {
            this.webHealthVaultConfiguration = this.ServiceLocator.GetInstance<HealthVaultConfiguration>();

            this.ServiceInstance = healthServiceInstance ?? new HealthServiceInstance(
                "1",
                "Default",
                "Default HealthVault instance",
                UrlUtilities.GetFullPlatformUrl(this.webHealthVaultConfiguration.DefaultHealthVaultUrl),
                this.webHealthVaultConfiguration.DefaultHealthVaultShellUrl);

            this.SessionCredential = sessionCredential;
        }

        public override Guid? ApplicationId => this.webHealthVaultConfiguration.MasterApplicationId;

        public override async Task AuthenticateAsync()
        {
            await this.RefreshSessionCredentialAsync(CancellationToken.None).ConfigureAwait(false);
        }

        protected override ISessionCredentialClient CreateSessionCredentialClient()
        {
            return this.ServiceLocator.GetInstance<IWebSessionCredentialClient>();
        }
    }
}
