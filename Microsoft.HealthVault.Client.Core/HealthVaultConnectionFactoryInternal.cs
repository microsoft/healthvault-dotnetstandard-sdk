using System;
using System.Collections.Generic;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Factory for creating client connections to HealthVault.
    /// </summary>
    internal class HealthVaultConnectionFactoryInternal : IHealthVaultConnectionFactory
    {
        private readonly object connectionLock = new object();

        private HealthVaultSodaConnection cachedConnection;

        /// <summary>
        /// Gets an <see cref="IHealthVaultSodaConnection"/> used to connect to HealthVault.
        /// </summary>
        /// <param name="configuration">Configuration required for authenticating the connection</param>
        /// <returns>Connection object to be used by the Client classes</returns>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetOrCreateSodaConnection"/> has been called already with a different MasterApplicationId.
        /// </exception>
        public IHealthVaultSodaConnection GetOrCreateSodaConnection(HealthVaultConfiguration configuration)
        {
            lock (this.connectionLock)
            {
                if (this.cachedConnection != null)
                {
                    ValidateConfiguration(this.cachedConnection.Configuration, configuration);
                    return this.cachedConnection;
                }

                var missingProperties = new List<string>();

                Guid masterApplicationId = configuration.MasterApplicationId;
                if (masterApplicationId == Guid.Empty)
                {
                    missingProperties.Add(nameof(configuration.MasterApplicationId));
                }

                if (missingProperties.Count > 0)
                {
                    string requiredPropertiesString = string.Join(", ", missingProperties);
                    throw new InvalidOperationException(Resources.MissingRequiredProperties.FormatResource(requiredPropertiesString));
                }

                Ioc.Container.Configure(c => c.ExportInstance(configuration).As<HealthVaultConfiguration>());

                HealthVaultSodaConnection newConnection = Ioc.Get<HealthVaultSodaConnection>();

                this.cachedConnection = newConnection;
                return newConnection;
            }// 
        }

        private static void ValidateConfiguration(HealthVaultConfiguration currentConfiguration, HealthVaultConfiguration configuration)
        {
            if (currentConfiguration.MasterApplicationId != configuration.MasterApplicationId)
            {
                throw new InvalidOperationException(Resources.CannotAuthWithDifferentMasterApplicationId.FormatResource(
                    nameof(GetOrCreateSodaConnection),
                    currentConfiguration.MasterApplicationId,
                    configuration.MasterApplicationId));
            }
        }
    }
}
