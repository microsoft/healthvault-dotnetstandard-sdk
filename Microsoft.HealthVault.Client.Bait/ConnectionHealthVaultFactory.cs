using System;

namespace Microsoft.HealthVault.Client
{
    public class ConnectionHealthVaultFactory : ConnectionHealthVaultFactoryBase
    {
        /// <summary>
        /// Gets the current IConnectionHealthVaultFactory instance.
        /// </summary>
        public static IConnectionHealthVaultFactory Current
        {
            get
            {
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
