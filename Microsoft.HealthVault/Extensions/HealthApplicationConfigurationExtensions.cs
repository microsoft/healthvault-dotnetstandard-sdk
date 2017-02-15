using System;

namespace Microsoft.HealthVault.Extensions
{
    /// <summary>
    /// Extension methods for IHealthApplicationConfiguration.
    /// </summary>
    internal static class HealthApplicationConfigurationExtensions
    {
        /// <summary>
        /// Gets the HealthVault method request URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value.
        /// </remarks>
        internal static Uri GetHealthVaultMethodUrl(this IHealthApplicationConfiguration configuration)
        {
            string newUri = configuration.HealthVaultUrl.AbsoluteUri;
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

        /// <summary>
        /// Gets the HealthVault type schema root URL for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value.
        /// </remarks>
        internal static Uri GetHealthVaultTypeSchemaUrl(this IHealthApplicationConfiguration configuration)
        {
            return new Uri(configuration.HealthVaultUrl, "type-xsd/");
        }

        /// <summary>
        /// Gets the URL to/from which BLOBs get streamed, for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks>
        internal static Uri GetBlobStreamUrl(this IHealthApplicationConfiguration configuration)
        {
            if (configuration.HealthVaultUrl != null)
            {
                return new Uri(configuration.HealthVaultUrl.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped) + "/streaming/wildcatblob.ashx");
            }

            return null;
        }

        /// <summary>
        /// Gets the HealthVault client service URL for
        /// the configured default instance of the HealthVault web-service,
        /// from the application or web configuration file.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks>
        internal static Uri GetHealthClientServiceUrl(this IHealthApplicationConfiguration configuration)
        {
            if (configuration.HealthVaultUrl != null)
            {
                return new Uri(configuration.HealthVaultUrl, "hvclientservice.ashx");
            }

            return null;
        }
    }
}
