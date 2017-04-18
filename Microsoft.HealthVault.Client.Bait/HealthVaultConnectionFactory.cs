using System;

namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory
    {
        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                // non-functional code change
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
