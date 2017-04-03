// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;

namespace Microsoft.HealthVault.Web.Connection
{
    internal class WebHealthVaultConnection : WebHealthVaultConnectionBase, IWebHealthVaultConnection
    {
        public WebHealthVaultConnection(IServiceLocator serviceLocator,
            HealthServiceInstance healthServiceInstance = null,
            SessionCredential sessionCredential = null,
            string userAuthToken = null)
            : base(serviceLocator, healthServiceInstance, sessionCredential)
        {
            this.UserAuthToken = userAuthToken;
        }

        public string UserAuthToken { get; internal set; }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            writer.WriteStartElement("auth-session");
            writer.WriteElementString("auth-token", this.SessionCredential.Token);

            if (!string.IsNullOrEmpty(UserAuthToken))
            {
                writer.WriteElementString("user-auth-token", this.UserAuthToken);
            }

            writer.WriteEndElement();
        }

        public override Task<PersonInfo> GetPersonInfoAsync()
        {
           IPrincipal principal = HttpContext.Current.User;
           HealthVaultIdentity user = principal?.Identity as HealthVaultIdentity;

            return Task.FromResult(user?.WebConnectionInfo?.PersonInfo);
        }

        public override string GetRestAuthSessionHeader(Guid? recordId)
        {
            throw new NotImplementedException();
        }
    }
}
