using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
{
    public class ServiceLocator : IServiceLocator
    {
        private static readonly object InstanceLock = new object();

        private static ServiceLocator current;

        private ICryptoService cryptoService;

        /// <summary>
        /// Gets the current configuration object for the app-domain.
        /// </summary>
        public static ServiceLocator Current
        {
            get
            {
                lock (InstanceLock)
                {
                    return current ?? (current = new ServiceLocator());
                }
            }

            internal set
            {
                lock (InstanceLock)
                {
                    current = value;
                }
            }
        }

        private ServiceLocator()
        {
        }

        public ICryptoService CryptoService => _cryptoService ?? (_cryptoService = new CryptoService(new BaseCryptoConfiguration()));
    }
}
