using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Factory for creating client connections to HealthVault.
    /// </summary>
    public abstract class ClientHealthVaultFactoryBase : IClientHealthVaultFactory
    {
        private readonly AsyncLock connectionLock = new AsyncLock();

        private IClientHealthVaultConnection cachedConnection;

        private ClientHealthVaultConfiguration healthVaultConfiguration;

        private readonly ConnectionState connectionState = new ConnectionState();

        /// <summary>
        /// Sets the configuration used to create connections.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetConnectionAsync"/> has been called already.
        /// </exception>
        public void SetConfiguration(ClientHealthVaultConfiguration clientHealthVaultConfiguration)
        {
            this.connectionState.ThrowIfAlreadyCreatedConnection(nameof(this.SetConfiguration));
            this.healthVaultConfiguration = clientHealthVaultConfiguration;
        }

        /// <summary>
        /// Gets an <see cref="IClientHealthVaultConnection"/> used to connect to HealthVault.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If called before calling <see cref="SetConfiguration(ClientHealthVaultConfiguration)"/>.
        /// </exception>
        public async Task<IClientHealthVaultConnection> GetConnectionAsync()
        {
            this.connectionState.MarkConnectionCalled();

            using (await this.connectionLock.LockAsync().ConfigureAwait(false))
            {
                if (this.cachedConnection != null)
                {
                    return this.cachedConnection;
                }

                if (this.healthVaultConfiguration == null)
                {
                    throw new InvalidOperationException(Resources.CannotCallMethodBefore.FormatResource(
                        nameof(this.GetConnectionAsync),
                        nameof(this.SetConfiguration)));
                }

                var missingProperties = new List<string>();

                Guid masterApplicationId = this.healthVaultConfiguration.MasterApplicationId;
                if (masterApplicationId == Guid.Empty)
                {
                    missingProperties.Add(nameof(masterApplicationId));
                }

                if (missingProperties.Count > 0)
                {
                    string requiredPropertiesString = string.Join(", ", missingProperties);
                    throw new InvalidOperationException(Resources.MissingRequiredProperties.FormatResource(requiredPropertiesString));
                }

                this.healthVaultConfiguration.Lock();

                Ioc.Container.Configure(c => c.ExportInstance(this.healthVaultConfiguration).As<ClientHealthVaultConfiguration>());
                Ioc.Container.Configure(c => c.ExportInstance(this.healthVaultConfiguration).As<HealthVaultConfiguration>());

                ClientHealthVaultConnection newConnection = Ioc.Get<ClientHealthVaultConnection>();
                await newConnection.AuthenticateAsync().ConfigureAwait(false);

                this.cachedConnection = newConnection;
                return newConnection;
            }
        }
    }
}
