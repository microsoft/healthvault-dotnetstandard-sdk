// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.AspNetCore.Connection
{
    internal class WebHealthVaultConnection : WebHealthVaultConnectionBase, IWebHealthVaultConnection
    {
        private readonly ClaimsIdentity identity;

        public WebHealthVaultConnection(ClaimsIdentity identity, HealthServiceInstance healthServiceInstance = null, SessionCredential sessionCredential = null, string userAuthToken = null)
            : base(healthServiceInstance, sessionCredential)
        {
            this.identity = identity;
            this.UserAuthToken = userAuthToken;

            Ioc.Container.Configure(c => c.ExportInstance(this).As<IConnectionInternal>());
        }

        public string UserAuthToken { get; set; }

        public override Task<PersonInfo> GetPersonInfoAsync()
        {
            return Task.FromResult(this.identity.GetConnectionInfo().PersonInfo);
        }

        public override string GetRestAuthSessionHeader()
        {
            return $"user-token={this.UserAuthToken}";
        }

        public override AuthSession GetAuthSessionHeader()
        {
            AuthSession authSession = new AuthSession
            {
                AuthToken = this.SessionCredential.Token,
                UserAuthToken = this.UserAuthToken
            };

            return authSession;
        }
    }
}
