namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory
    {
        private static readonly object InstanceLock = new object();
        private static IHealthVaultConnectionFactory current;

        /// <summary>
        /// Gets the current IHealthVaultConnectionFactory instance.
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
