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
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Exceptions;

namespace Microsoft.HealthVault.Web.Connection
{
    internal class WebHealthVaultConnection : WebHealthVaultConnectionBase, IWebHealthVaultConnection
    {
        private readonly AsyncLock _personInfoLock = new AsyncLock();
        private PersonInfo _personInfo;

        public WebHealthVaultConnection(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
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
            if (_personInfo != null)
            {
                return _personInfo;
            }

            using (await _personInfoLock.LockAsync())
            {
                if (_personInfo == null)
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
                        IPersonClient personClient = CreatePersonClient();
                        var personInfoFromServer = await personClient.GetPersonInfoAsync();

                        applicationSettingsDocument = personInfoFromServer.ApplicationSettingsDocument;
                        authorizedRecords = personInfoFromServer.AuthorizedRecords;
                    }

                    _personInfo = personInfoFromCookie;

                    if (applicationSettingsDocument != null)
                    {
                        _personInfo.ApplicationSettingsDocument = applicationSettingsDocument;
                    }

                    if (authorizedRecords != null)
                    {
                        _personInfo.AuthorizedRecords = authorizedRecords;
                    }
                }

                return _personInfo;
            }
        }

        protected override string GetPlatformSpecificRestAuthHeaderPortion()
        {
            return $"user-token={UserAuthToken}";
        }

        public override AuthSession GetAuthSessionHeader()
        {
            AuthSession authSession = new AuthSession
            {
                AuthToken = SessionCredential.Token,
                UserAuthToken = UserAuthToken
            };

            return authSession;
        }
    }
}
