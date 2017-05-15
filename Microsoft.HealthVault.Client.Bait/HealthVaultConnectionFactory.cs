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
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
