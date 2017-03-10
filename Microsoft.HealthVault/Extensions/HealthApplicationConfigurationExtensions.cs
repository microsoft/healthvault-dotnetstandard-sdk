using System;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.Extensions
{
    /// <summary>
    /// Extension methods for IConfiguration.
    /// </summary>
    internal static class HealthApplicationConfigurationExtensions
    {
        /// <summary>
        /// Gets the HealthVault client service URL for
        /// the configured default instance of the HealthVault web-service,
        /// from the application or web configuration file.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks>
        public static Uri GetHealthClientServiceUrl(this HealthVaultConfiguration configuration)
        {
            if (configuration.DefaultHealthVaultUrl != null)
            {
                return new Uri(configuration.DefaultHealthVaultUrl, "hvclientservice.ashx");
            }

            return null;
        }
    }
}
