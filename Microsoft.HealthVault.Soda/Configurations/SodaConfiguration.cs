using System;

namespace Microsoft.HealthVault.Configurations
{
    /// <summary>
    /// Class used for web configurations
    /// </summary>
    public class SodaConfiguration : ConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SodaConfiguration"/> class.
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public SodaConfiguration()
        {
        }

        public bool AllowInstanceBounce { get; set; } = true;
    }
}
