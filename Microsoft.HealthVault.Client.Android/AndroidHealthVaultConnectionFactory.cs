
namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory
    {
        private static readonly object s_instanceLock = new object();
        private static IHealthVaultConnectionFactory s_current;

        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                lock (s_instanceLock)
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