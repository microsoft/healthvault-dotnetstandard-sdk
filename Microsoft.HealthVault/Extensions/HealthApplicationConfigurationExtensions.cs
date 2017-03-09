using System;
using Microsoft.HealthVault.Configurations;

namespace Microsoft.HealthVault.Extensions
{
    /// <summary>
    /// Extension methods for IConfiguration.
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
        public static Uri GetHealthVaultMethodUrl(this IConfiguration configuration)
        {
            string newUri = configuration.DefaultHealthVaultUrl.AbsoluteUri;
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
        public static Uri GetHealthVaultTypeSchemaUrl(this IConfiguration configuration)
        {
            return new Uri(configuration.DefaultHealthVaultUrl, "type-xsd/");
        }

        /// <summary>
        /// Gets the URL to/from which BLOBs get streamed, for
        /// the configured default instance of the HealthVault web-service.
        /// </summary>
        /// <remarks>
        /// This property is based on the "HealthVaultUrl" configuration
        /// value with the path modified to the appropriate handler.
        /// </remarks>
        public static Uri GetBlobStreamUrl(this IConfiguration configuration)
        {
            if (configuration.DefaultHealthVaultUrl != null)
            {
                return new Uri(configuration.DefaultHealthVaultUrl.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped) + "/streaming/wildcatblob.ashx");
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
        public static Uri GetHealthClientServiceUrl(this IConfiguration configuration)
        {
            if (configuration.DefaultHealthVaultUrl != null)
            {
                return new Uri(configuration.DefaultHealthVaultUrl, "hvclientservice.ashx");
            }

            return null;
        }
    }
}
