using Microsoft.HealthVault.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Extensions;

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

        private IClientHealthVaultConnection cachedConnection;

        private ClientConfiguration configuration;

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

        /// <summary>
        /// Sets the configuration used to create connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetConnectionAsync"/> has been called already.
        /// </exception>
        public void SetConfiguration(ClientConfiguration clientConfiguration)
        {
            this.ThrowIfAlreadyCreatedConnection(nameof(this.SetConfiguration));
            this.configuration = clientConfiguration;
        }

        /// <summary>
        /// Gets an <see cref="IClientHealthVaultConnection"/> used to connect to HealthVault.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If called before calling <see cref="SetConfiguration(ClientConfiguration)"/>.
        /// </exception>
        public async Task<IClientHealthVaultConnection> GetConnectionAsync()
        {
            this.GetConnectionCalled = true;

            using (await this.connectionLock.LockAsync().ConfigureAwait(false))
            {
                if (this.cachedConnection != null)
                {
                    return this.cachedConnection;
                }

                if (this.configuration == null)
                {
                    throw new InvalidOperationException(Resources.CannotCallMethodBefore.FormatResource(
                        nameof(this.GetConnectionAsync),
                        nameof(this.SetConfiguration)));
                }

                var missingProperties = new List<string>();

                if (this.configuration.MasterApplicationId == Guid.Empty)
                {
                    missingProperties.Add(nameof(this.configuration.MasterApplicationId));
                }

                if (missingProperties.Count > 0)
                {
                    string requiredPropertiesString = string.Join(", ", missingProperties);
                    throw new InvalidOperationException(Resources.MissingRequiredProperties.FormatResource(requiredPropertiesString));
                }

                this.configuration.Lock();

                Ioc.Container.Configure(c => c.ExportInstance(this.configuration).As<ClientConfiguration>());
                Ioc.Container.Configure(c => c.ExportInstance(this.configuration).As<HealthVaultConfiguration>());

                ClientHealthVaultConnection newConnection = Ioc.Get<ClientHealthVaultConnection>();
                await newConnection.AuthenticateAsync().ConfigureAwait(false);

                this.cachedConnection = newConnection;
                return newConnection;
            }
        }
    }
}
