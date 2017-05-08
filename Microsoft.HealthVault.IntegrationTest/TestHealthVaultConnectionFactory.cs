using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client;

namespace Microsoft.HealthVault.IntegrationTest
{
    public class HealthVaultConnectionFactory
    {
        private static readonly object InstanceLock = new object();
        private static IHealthVaultConnectionFactory current;

        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    if (current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        current = new HealthVaultConnectionFactoryInternal();
                    }

                    return current;
                }
            }
        }
    }
}
