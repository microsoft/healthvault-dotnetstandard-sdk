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
                    return current ?? (current = new ClientHealthVaultFactory());
                }
            }
        }

        protected override void EnsureIocInitialized()
        {
            ClientIoc.EnsureTypesRegistered();
        }
    }
}