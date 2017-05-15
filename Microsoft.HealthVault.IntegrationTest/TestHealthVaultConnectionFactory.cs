using System;
using Microsoft.HealthVault.Client;

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
    }
}
