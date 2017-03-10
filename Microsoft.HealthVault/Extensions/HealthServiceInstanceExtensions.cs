using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.PlatformInformation;

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
            string newUri = serviceInstance.HealthServiceUrl.AbsoluteUri;
            if (!newUri.EndsWith("/", StringComparison.Ordinal))
            {
                newUri = newUri + "/wildcat.ashx";
            }
            else
            {
                newUri = newUri + "wildcat.ashx";
            }

            return new Uri(newUri);
        }
    }
}
