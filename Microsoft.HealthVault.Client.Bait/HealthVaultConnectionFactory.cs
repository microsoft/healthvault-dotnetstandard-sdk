using Microsoft.HealthVault.Client.Core;
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
                // a third non-functional code change
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
