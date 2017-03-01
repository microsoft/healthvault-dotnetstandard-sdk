using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Things;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Application;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// The default implementation of IPlatformClient
    /// </summary>
    internal class PlatformClient : IPlatformClient
    {
        public HealthServiceConnection Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; set; }

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

        public Task<IDictionary<Guid, HealthRecordItemTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds, 
            HealthRecordItemTypeSections sections, 
            IList<string> imageTypes, 
            DateTime? lastClientRefreshDate)
        {
            return HealthVaultPlatformInformation.Current.GetHealthRecordItemTypeDefinitionAsync(typeIds, sections, imageTypes, lastClientRefreshDate, this.Connection);
        }
    }
}
