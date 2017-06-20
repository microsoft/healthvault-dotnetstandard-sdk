using System;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class HealthVaultConnectionFactory
    {
        private static readonly object InstanceLock = new object();
        private static IHealthVaultConnectionFactory s_current;

        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    if (s_current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        s_current = new HealthVaultConnectionFactoryInternal();
                    }

                    return s_current;
                }
            }
        }

        public static IThingTypeRegistrar ThingTypeRegistrar
        {
            get
            {
                ClientIoc.EnsureTypesRegistered();
                return Ioc.Get<IThingTypeRegistrar>();
            }
        }
    }
}
