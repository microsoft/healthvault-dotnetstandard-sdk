// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Connection;

namespace Microsoft.HealthVault.Web.Providers
{
    internal class ServiceInstanceProvider : IServiceInstanceProvider
    {
        private readonly AsyncLock seriviceInstanceLock;
        private readonly IServiceLocator serviceLocator;

        private HealthServiceInstance cachedServiceInstance;

        public ServiceInstanceProvider(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            this.seriviceInstanceLock = new AsyncLock();
        }
 
        public async Task<HealthServiceInstance> GetHealthServiceInstanceAsync(string serviceInstanceId)
        {
            using (await this.seriviceInstanceLock.LockAsync().ConfigureAwait(false))
            {
                if (this.cachedServiceInstance == null)
                {
                    ServiceInfo serviceInfo = await this.GetFromServiceAsync().ConfigureAwait(false);

                    if (!serviceInfo.ServiceInstances.TryGetValue(serviceInstanceId, out cachedServiceInstance))
                    {
                        throw new HealthServiceException(HealthServiceStatusCode.Failed);
                    }
                }

                return this.cachedServiceInstance;
            }
        }

        private async Task<ServiceInfo> GetFromServiceAsync()
        {
            
            IWebHealthVaultConnection webHealthVaultConnection = new WebHealthVaultConnection(serviceLocator);
            IPlatformClient platformClient = webHealthVaultConnection.CreatePlatformClient();

            ServiceInfo serviceInfo = await platformClient.GetServiceDefinitionAsync(ServiceInfoSections.Topology).ConfigureAwait(false);
            return serviceInfo;
        }
    }
}
