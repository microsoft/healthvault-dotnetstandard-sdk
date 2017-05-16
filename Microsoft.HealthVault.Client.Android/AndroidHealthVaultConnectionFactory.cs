namespace Microsoft.HealthVault.Client
{
    public class HealthVaultConnectionFactory
    {
        private static readonly object s_instanceLock = new object();
        private static IHealthVaultConnectionFactory _current;

        /// <summary>
        /// Gets the current factory instance.
        /// </summary>
        public static IHealthVaultConnectionFactory Current
        {
            get
            {
                lock (s_instanceLock)
                {
                    if (_current == null)
                    {
                        ClientIoc.EnsureTypesRegistered();
                        _current = new HealthVaultConnectionFactoryInternal();
                    }

                    return _current;
                }
            }
        }
    }
}