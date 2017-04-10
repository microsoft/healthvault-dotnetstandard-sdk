// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Exceptions;

namespace Microsoft.HealthVault.Web.Connection
{
    internal class WebHealthVaultConnection : WebHealthVaultConnectionBase, IWebHealthVaultConnection
    {
        private readonly AsyncLock personInfoLock = new AsyncLock();

        private PersonInfo personInfo;

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

        /// <summary>
        /// Get PersonInfo for the authenticated connection.
        /// In case the request is not authenticated, then throws <see cref="UserNotFoundException"/>
        /// </summary>
        /// <returns>PersonInfo</returns>
        /// <exception cref="UserNotFoundException">When the request is not authenticated, the method will throw exception</exception>
        public override async Task<PersonInfo> GetPersonInfoAsync()
        {
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
                    this.personInfo = webConnectionInfo.PersonInfo;

                    // In case application settings/records are minimized due to size constraints in storing the webconnectionInfo object
                    // as a cookie, we will restore the application settings and authorized documents from the server.
                    if (webConnectionInfo.MinimizedPersonInfoApplicationSettings || webConnectionInfo.MinimizedPersonInfoRecords)
                    {
                        IPersonClient personClient = this.CreatePersonClient();
                        IReadOnlyCollection<PersonInfo> personInfoCollection = await personClient.GetAuthorizedPeopleAsync().ConfigureAwait(false);

                        // By default we pick the first authorized person for the app.
                        var personInfoFromServer = personInfoCollection.FirstOrDefault();

                        if (personInfoFromServer == null)
                        {
                            throw new UserNotFoundException("Authenticated user for the app cannot be found");
                        }

                        this.personInfo.ApplicationSettingsDocument = personInfoFromServer.ApplicationSettingsDocument;
                        this.personInfo.AuthorizedRecords = personInfoFromServer.AuthorizedRecords;
                    }
                }

                return this.personInfo; 
            }
        }

        public override string GetRestAuthSessionHeader(Guid? recordId)
        {
            throw new NotImplementedException();
        }
    }
}
