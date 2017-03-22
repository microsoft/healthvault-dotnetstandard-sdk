// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// The default implementation of IPlatformClient
    /// </summary>
    internal class PlatformClient : IPlatformClient
    {
        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Task<HealthServiceInstance> SelectInstanceAsync(Location preferredLocation)
        {
            return HealthVaultPlatformInformation.Current.SelectInstanceAsync(this.Connection, preferredLocation);
        }

        public Task<ServiceInfo> GetServiceDefinitionAsync()
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(this.Connection);
        }

        public Task<ServiceInfo> GetServiceDefinitionAsync(ServiceInfoSections responseSections)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(this.Connection, responseSections);
        }

        public Task<ServiceInfo> GetServiceDefinitionAsync(DateTime lastUpdatedTime)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(this.Connection, lastUpdatedTime);
        }

        public Task<ServiceInfo> GetServiceDefinitionAsync(ServiceInfoSections responseSections, DateTime lastUpdatedTime)
        {
            return HealthVaultPlatformInformation.Current.GetServiceDefinitionAsync(this.Connection, responseSections, lastUpdatedTime);
        }

        public Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            DateTime? lastClientRefreshDate)
        {
            return HealthVaultPlatformInformation.Current.GetHealthRecordItemTypeDefinitionAsync(typeIds, sections, imageTypes, lastClientRefreshDate, this.Connection);
        }

        public async Task<ApplicationCreationInfo> NewApplicationCreationInfoAsync()
        {
            HealthServiceResponseData responseData = await this.Connection.ExecuteAsync(HealthVaultMethods.NewApplicationCreationInfo, 1).ConfigureAwait(false);
            return ApplicationCreationInfo.Create(responseData.InfoNavigator);
        }

        public IEnumerable<Task<PersonInfo>> GetAuthorizedPeople()
        {
            return this.GetAuthorizedPeople(new GetAuthorizedPeopleSettings());
        }

        public IEnumerable<Task<PersonInfo>> GetAuthorizedPeople(GetAuthorizedPeopleSettings settings)
        {
            return HealthVaultPlatform.GetAuthorizedPeopleAsync(this.Connection, settings);
        }
    }
}
