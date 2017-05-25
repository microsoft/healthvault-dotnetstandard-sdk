// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
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
            WebHealthVaultFactory factory = new WebHealthVaultFactory();
            IWebHealthVaultConnection webHealthVaultConnection = await factory.CreateWebConnectionInternalAsync();

            return webHealthVaultConnection;
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
            WebHealthVaultFactory factory = new WebHealthVaultFactory();
            IOfflineHealthVaultConnection webHealthVaultConnection = await factory.CreateOfflineConnectionInternalAsync(
                offlinePersonId,
                instanceId,
                sessionCredential);

            return webHealthVaultConnection;
        }

        // Enables unit test
        internal async Task<IWebHealthVaultConnection> CreateWebConnectionInternalAsync()
        {
            IHealthVaultIdentityProvider healthVaultIdentityProvider = Ioc.Container.Locate<IHealthVaultIdentityProvider>();
            HealthVaultIdentity identity = healthVaultIdentityProvider.TryGetIdentity();

            IServiceLocator serviceLocator = new ServiceLocator();

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
            IServiceInstanceProvider serviceInstanceProvider = Ioc.Container.Locate<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(webConnectionInfo.ServiceInstanceId);

            // Get AuthInformation
            SessionCredential sessionCredentialToken = webConnectionInfo.SessionCredential;
            string token = webConnectionInfo.UserAuthToken;

            IWebHealthVaultConnection webConnection = Ioc.Container.Locate<IWebHealthVaultConnection>(extraData: new { serviceLocator = serviceLocator });

            WebHealthVaultConnection connection = webConnection as WebHealthVaultConnection;
            connection.UserAuthToken = token;
            connection.ServiceInstance = serviceInstance;
            connection.SessionCredential = sessionCredentialToken;

            return webConnection;
        }

        // Enables unit test
        internal async Task<IOfflineHealthVaultConnection> CreateOfflineConnectionInternalAsync(
            string offlinePersonId,
            string instanceId = null,
            SessionCredential sessionCredential = null)
        {
            Guid parsedOfflinePersonId;
            if (!Guid.TryParse(offlinePersonId, out parsedOfflinePersonId))
            {
                throw new ArgumentException("Unable to parse offline person id to Guid", nameof(offlinePersonId));
            }

            IServiceLocator serviceLocator = new ServiceLocator();

            HealthServiceInstance serviceInstance = null;
            if (!string.IsNullOrEmpty(instanceId))
            {
                // Get ServiceInstance
                IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
                serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);
            }

            IOfflineHealthVaultConnection offlineHealthVaultConnection = Ioc.Container.Locate<IOfflineHealthVaultConnection>(
                extraData: new { serviceLocator = serviceLocator });

            OfflineHealthVaultConnection connection = offlineHealthVaultConnection as OfflineHealthVaultConnection;
            connection.SessionCredential = sessionCredential;
            connection.OfflinePersonId = parsedOfflinePersonId;

            // By default, service instance is "US", so do not override in case the instance id is not set
            if (serviceInstance != null)
            {
                connection.ServiceInstance = serviceInstance;
            }

            return offlineHealthVaultConnection;
        }
    }
}
