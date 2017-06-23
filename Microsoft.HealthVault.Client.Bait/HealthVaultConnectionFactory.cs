using System;
using Microsoft.HealthVault.Client.Core;
using Microsoft.HealthVault.Thing;

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

        /// <summary>
        /// Gets ThingTypeRegistrar to allow apps to register custom thing types and thing extensions
        /// </summary>
        public static IThingTypeRegistrar ThingTypeRegistrar => throw new NotImplementedException(ClientResources.BaitWithoutSwitchError);
    }
}
