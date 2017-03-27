namespace Microsoft.HealthVault.Client
{
    public class ConnectionHealthVaultFactory : ConnectionHealthVaultFactoryBase
    {
        private static readonly object InstanceLock = new object();
        private static IConnectionHealthVaultFactory current;

        /// <summary>
        /// Gets the current IConnectionHealthVaultFactory instance.
        /// </summary>
        public static IConnectionHealthVaultFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    if (current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        current = new ConnectionHealthVaultFactory();
                    }

                    return current;
                }
            }
        }
    }
}