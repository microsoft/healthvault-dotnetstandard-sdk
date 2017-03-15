using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Utilities;

namespace Microsoft.HealthVault.Extensions
{
    internal static class HealthServiceInstanceExtensions
    {
        /// <summary>
        /// Gets the HealthVault method request URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value.
        /// </remarks>
        public static Uri GetHealthVaultMethodUrl(this HealthServiceInstance serviceInstance)
        {
            return UrlUtilities.GetFullPlatformUrl(serviceInstance.HealthServiceUrl);
        }
    }
}
