// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.HealthVault.AspNetCore.Connection;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore
{
    /// <summary>
    /// Factory class to create Web/Offline connections
    /// </summary>
    public static class WebHealthVaultFactory
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
        public static async Task<IWebHealthVaultConnection> CreateWebConnectionAsync(this HttpContext context)
        {
            IPrincipal principal = context.User;
            var identity = principal?.Identity as ClaimsIdentity;

            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();
            var webConnectionInfo = identity.GetConnectionInfo();

            // Get ServiceInstance
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(webConnectionInfo.ServiceInstanceId);

            // Get AuthInformation
            SessionCredential sessionCredentialToken = webConnectionInfo.SessionCredential;
            string token = webConnectionInfo.UserAuthToken;
            
            IWebHealthVaultConnection webConnection = new WebHealthVaultConnection(identity, serviceInstance, sessionCredentialToken, token);

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

            IOfflineHealthVaultConnection offlineHealthVaultConnection = new OfflineHealthVaultConnection( 
                serviceInstance,
                sessionCredential,
                offlinePersonId);

            return offlineHealthVaultConnection;
        }
    }
}
