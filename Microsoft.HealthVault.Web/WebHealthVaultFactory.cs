using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web
{
    public class WebHealthVaultFactory
    {
        private static readonly object InstanceLock = new object();
        private static WebHealthVaultFactory current;

        private readonly AsyncLock connectionLock = new AsyncLock();
        private WebConfiguration configuration;
        private IHealthVaultConnection cachedWebApplicationConnection;

        private readonly ConnectionState connectionState = new ConnectionState();

        /// <summary>
        /// Gets the current WebHealthVaultFactory instance.
        /// </summary>
        public static WebHealthVaultFactory Current
        {
            get
            {
                lock (InstanceLock)
                {
                    return current ?? (current = new WebHealthVaultFactory());
                }
            }
        }

        private WebHealthVaultFactory()
        {
        }

        /// <summary>
        /// Sets the configuration used to create connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetWebApplicationConnectionAsync"/> has been called already.
        /// </exception>
        public void SetConfiguration(WebConfiguration configuration)
        {
            this.connectionState.ThrowIfAlreadyCreatedConnection(nameof(this.SetConfiguration));
            this.configuration = configuration;
        }

        public async Task<IHealthVaultConnection> GetWebApplicationConnectionAsync()
        {
            this.connectionState.MarkConnectionCalled();
            using (await (this.connectionLock.LockAsync().ConfigureAwait(false)))
            {
                if (this.cachedWebApplicationConnection != null)
                {
                    return this.cachedWebApplicationConnection;
                }

                WebIoc.EnsureTypesRegistered();

                var config = this.configuration ?? WebConfigurationFactory.CreateConfiguration();
                Ioc.Container.Configure(c => c.ExportInstance(() => config));
                Ioc.Container.Configure(c => c.ExportInstance<HealthVaultConfiguration>(() => config));
                config.Lock();

                IHealthVaultConnection connection = Ioc.Get<IConnectionInternal>();
                await connection.AuthenticateAsync().ConfigureAwait(false);
                this.cachedWebApplicationConnection = connection;
                return connection;
            }
        }
    }
}
