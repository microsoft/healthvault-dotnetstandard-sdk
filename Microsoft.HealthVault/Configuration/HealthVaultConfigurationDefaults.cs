using System;

namespace Microsoft.HealthVault.Configuration
{
    /// <summary>
    /// Default vaules for HealthVault configuration
    /// </summary>
    public class HealthVaultConfigurationDefaults
    {
        /// <summary>
        /// The default number of internal retries.
        /// </summary>
        public const int RetryOnInternal500Count = 2;

        /// <summary>
        /// Default URL for Shell application
        /// </summary>
        public static readonly Uri ShellUrl = new Uri("https://account.healthvault.com");

        /// <summary>
        /// Default URL for HealthVault application
        /// </summary>
        public static readonly Uri HealthVaultRootUrl = new Uri("https://platform.healthvault.com/platform/");

        /// <summary>
        /// Default sleep duration in seconds.
        /// </summary>
        public static readonly TimeSpan RetryOnInternal500SleepDuration = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Base class for the app, web and soda configurations
        /// </summary>
        /// <summary>
        /// The default request time to live value.
        /// </summary>
        public static readonly TimeSpan RequestTimeToLiveDuration = TimeSpan.FromSeconds(30 * 60);

        /// <summary>
        /// The default request time out value.
        /// </summary>
        public static readonly TimeSpan RequestTimeoutDuration = TimeSpan.FromSeconds(30);
    }
}
