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
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Factory class to create Web/Offline connections
    /// </summary>
    public class WebHealthVaultFactory
    {
        /// <summary>
        /// Creates an authenticated web connection when the reuest 
        /// has been authenticated using [RequireSignIn] attribute.
        /// In case the request has not been authenticated, an anonymous
        /// connection is created.
        /// </summary>
        /// <returns>IWebHealthVaultConnection</returns>
        /// <exception cref="NotSupportedException">
        ///     WebConnectionInfo is expected for authenticated connections
        /// </exception>
        public static async Task<IWebHealthVaultConnection> CreateWebConnectionAsync()
        {
            IPrincipal principal = HttpContext.Current.User;
            HealthVaultIdentity identity = principal?.Identity as HealthVaultIdentity;

            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();

            if (identity == null)
            {
                IWebHealthVaultConnection anonymousWebConnection = serviceLocator.GetInstance<IWebHealthVaultConnection>();
                return anonymousWebConnection;
            }

            var webConnectionInfo = identity.WebConnectionInfo;

            if (webConnectionInfo == null)
            {
                throw new NotSupportedException("WebConnectionInfo is expected for authenticated connections");
            }

            // Get ServiceInstance
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(webConnectionInfo.ServiceInstanceId);

            // Get AuthInformation
            SessionCredential sessionCredentialToken = webConnectionInfo.SessionCredential;
            string token = webConnectionInfo.UserAuthToken;

            IWebHealthVaultConnection webConnection = Ioc.Container.Locate<IWebHealthVaultConnection>(
                new { serviceLocator, serviceInstance, sessionCredentialToken, token });

            return webConnection;
        }

        /// <summary>
        /// Creates the offline connection.
        /// </summary>
        /// <param name="offlinePersonId">The offline person identifier.</param>
        /// <param name="instanceId">The instance identifier.</param>
        /// <param name="sessionCredential">The session credential.</param>
        /// <returns></returns>
        public static async Task<IOfflineHealthVaultConnection> CreateOfflineConnectionAsync(
            string offlinePersonId, 
            string instanceId = null, 
            SessionCredential sessionCredential = null)
        {
            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();

            // Get ServiceInstance
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            IOfflineHealthVaultConnection offlineHealthVaultConnection = Ioc.Container.Locate<IOfflineHealthVaultConnection>(new 
            {
                serviceLocator,
                serviceInstance,
                sessionCredential,
                offlinePersonId
            });

            return offlineHealthVaultConnection;
        }
    }
}
