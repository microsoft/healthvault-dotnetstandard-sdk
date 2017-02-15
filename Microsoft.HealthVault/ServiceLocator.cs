using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
{
    public class ServiceLocator : IServiceLocator
    {
        private static readonly object instanceLock = new object();

        private static ServiceLocator _current;

        private ICryptoService _cryptoService;

        /// <summary>
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public static ServiceLocator Current
        {
            get
            {
                lock (instanceLock)
                {
                    return _current ?? (_current = new ServiceLocator());
                }
            }

            internal set
            {
                lock (instanceLock)
                {
                    _current = value;
                }
            }
        }

        private ServiceLocator() { }

        public ICryptoService CryptoService => _cryptoService ?? (_cryptoService = new CryptoService(new CryptoConfiguration()));
    }
}
