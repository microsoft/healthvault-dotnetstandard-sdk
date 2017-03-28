namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory : HealthVaultConnectionFactoryBase
    {
        private static readonly object InstanceLock = new object();
        private static IHealthVaultConnectionFactory current;

        /// <summary>
        /// Gets the current IClientHealthVaultFactory instance.
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
                        current = new HealthVaultConnectionFactory();
                    }

                    return current;
                }
            }
        }
    }
}
