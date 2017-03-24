using System;

namespace Microsoft.HealthVault.Client
{
    public class ClientHealthVaultFactory : ClientHealthVaultFactoryBase
    {
        /// <summary>
        /// Gets the current IClientHealthVaultFactory instance.
        /// </summary>
        public static IClientHealthVaultFactory Current
        {
            get
            {
                throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
            }
        }
    }
}
