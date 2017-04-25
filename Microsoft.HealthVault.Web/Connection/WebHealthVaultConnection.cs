// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Exceptions;

namespace Microsoft.HealthVault.Web.Connection
{
    internal class WebHealthVaultConnection : WebHealthVaultConnectionBase, IWebHealthVaultConnection
    {
        private readonly AsyncLock personInfoLock = new AsyncLock();
        private PersonInfo personInfo;

        public WebHealthVaultConnection(
            IServiceLocator serviceLocator,
            IHealthWebRequestClient healthWebRequestClient,
            HealthVaultConfiguration configuration,
            HealthServiceInstance healthServiceInstance = null,
            SessionCredential sessionCredential = null,
            string userAuthToken = null)
            : base(serviceLocator, healthWebRequestClient, configuration, healthServiceInstance, sessionCredential)
        {
            this.UserAuthToken = userAuthToken;

            Ioc.Container.Configure(c => c.ExportInstance(this).As<IConnectionInternal>());
        }

        public string UserAuthToken { get; set; }

        /// <summary>
        /// Get PersonInfo for the authenticated connection.
        /// In case the request is not authenticated, then throws <see cref="UserNotFoundException"/>
        /// </summary>
        /// <returns>PersonInfo</returns>
        /// <exception cref="UserNotFoundException">When the request is not authenticated, the method will throw exception</exception>
        public override async Task<PersonInfo> GetPersonInfoAsync()
        {
            if (this.personInfo != null)
            {
                return this.personInfo;
            }

            using (await this.personInfoLock.LockAsync())
            {
                if (this.personInfo == null)
                {
                    IPrincipal principal = HttpContext.Current.User;
                    HealthVaultIdentity user = principal?.Identity as HealthVaultIdentity;

                    if (user == null)
                    {
                        throw new UserNotFoundException("Request should be authorized to retrieve PersonInfo, use RequireSignIn attribute");
                    }

                    WebConnectionInfo webConnectionInfo = user.WebConnectionInfo;
                    var personInfoFromCookie = webConnectionInfo.PersonInfo;

                    XDocument applicationSettingsDocument = null;
                    IDictionary<Guid, HealthRecordInfo> authorizedRecords = null;

                    // In case application settings/records are minimized due to size constraints in storing the webconnectionInfo object
                    // as a cookie, we will restore the application settings and authorized documents from the server.
                    if (webConnectionInfo.MinimizedPersonInfoApplicationSettings || webConnectionInfo.MinimizedPersonInfoRecords)
                    {
                        IPersonClient personClient = this.CreatePersonClient();
                        var personInfoFromServer = await personClient.GetPersonInfoAsync();

                        applicationSettingsDocument = personInfoFromServer.ApplicationSettingsDocument;
                        authorizedRecords = personInfoFromServer.AuthorizedRecords;
                    }

                    this.personInfo = personInfoFromCookie;

                    if (applicationSettingsDocument != null)
                    {
                        this.personInfo.ApplicationSettingsDocument = applicationSettingsDocument;
                    }

                    if (authorizedRecords != null)
                    {
                        this.personInfo.AuthorizedRecords = authorizedRecords;
                    }
                }

                return this.personInfo; 
            }
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
