namespace Microsoft.HealthVault.Client
{
    public class ClientHealthVaultFactory : ClientHealthVaultFactoryBase
    {
        private static readonly object InstanceLock = new object();
        private static IClientHealthVaultFactory current;

        /// <summary>
        /// Gets the current IClientHealthVaultFactory instance.
        /// </summary>
        public static IClientHealthVaultFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    if (current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        current = new ClientHealthVaultFactory();
                    }

                    return current;
                }
            }
        }
    }
}