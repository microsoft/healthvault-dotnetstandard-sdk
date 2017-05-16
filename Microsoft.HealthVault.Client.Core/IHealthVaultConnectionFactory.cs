using System;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Factory for creating client connections to HealthVault.
    /// </summary>
    public interface IHealthVaultConnectionFactory
    {
        /// <summary>
        /// Gets an <see cref="IHealthVaultSodaConnection"/> used to connect to HealthVault.
        /// </summary>
        /// <param name="configuration">Configuration required for authenticating the connection</param>
        /// <returns>Connection object to be used by the Client classes</returns>
        /// <exception cref="InvalidOperationException">
        /// If <see cref="GetOrCreateSodaConnection"/> has been called already with a different MasterApplicationId.
        /// </exception>
        IHealthVaultSodaConnection GetOrCreateSodaConnection(HealthVaultConfiguration configuration);
    }
}
