using Grace.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Factory for creating client connections to HealthVault.
    /// </summary>
    public class ClientHealthVaultFactory : HealthVaultFactoryBase, IClientHealthVaultFactory
    {
        private static readonly object InstanceLock = new object();

        private static IClientHealthVaultFactory current;

        private readonly AsyncLock connectionLock = new AsyncLock();

        private IHealthVaultConnection cachedConnection;

        private ClientConfiguration configuration;

        private volatile bool getConnectionCalled;

        internal ClientHealthVaultFactory()
        {
        }

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

        public void SetConfiguration(ClientConfiguration clientConfiguration)
        {
            if (this.getConnectionCalled)
            {
                throw new InvalidOperationException("Cannot set configuration after calling GetConnectionAsync.");
            }

            this.configuration = clientConfiguration;
        }

        public async Task<IHealthVaultConnection> GetConnectionAsync()
        {
            this.getConnectionCalled = true;

            using (await this.connectionLock.LockAsync().ConfigureAwait(false))
            {
                if (this.cachedConnection != null)
                {
                    return this.cachedConnection;
                }

                if (this.configuration == null)
                {
                    throw new InvalidOperationException("Cannot call GetConnectionAsync before calling SetConfiguration.");
                }

                if (this.configuration.DefaultHealthVaultUrl == null || this.configuration.DefaultHealthVaultShellUrl == null)
                {
                    var requiredParameters = new List<string>
                    {
                        nameof(this.configuration.DefaultHealthVaultUrl),
                        nameof(this.configuration.DefaultHealthVaultShellUrl)
                    };

                    string requiredParametersString = string.Join(", ", requiredParameters);
                    throw new InvalidOperationException("Missing one or more required parameters on configuration. Required parameters: " + requiredParametersString);
                }

                this.configuration.Lock();

                Ioc.Container.Configure(c => c.ExportInstance(this.configuration).As<ClientConfiguration>().Lifestyle.Singleton());

                ClientHealthVaultConnection newConnection = Ioc.Get<ClientHealthVaultConnection>();
                await newConnection.AuthenticateAsync().ConfigureAwait(false);

                this.cachedConnection = newConnection;
                return newConnection;
            }
        }
    }
}
