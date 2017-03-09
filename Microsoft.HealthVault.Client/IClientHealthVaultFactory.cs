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
    public interface IClientHealthVaultFactory : IHealthVaultFactoryBase
    {
        /// <summary>
        /// Sets the client configuration to use.
        /// </summary>
        /// <param name="clientConfiguration">The configuration to use.</param>
        /// <exception cref="InvalidOperationException">Thrown when called after calling <see cref="GetConnectionAsync"/>.</exception>
        /// <remarks>This can only be set before calling <see cref="GetConnectionAsync"/>. After calling it,
        /// this property cannot be set and no settings on the object can be changed.</remarks>
        void SetConfiguration(ClientConfiguration clientConfiguration);

        /// <summary>
        /// Gets a connection to access HealthVault.
        /// </summary>
        /// <returns>A connection to access HealthVault.</returns>
        /// <exception cref="InvalidOperationException">Thrown when called before calling <see cref="SetConfiguration"/> with required values.</exception>
        /// <remarks>This will perform any authentication needed to create the connection, including
        /// opening a web browser to prompt for credentials/consent.</remarks>
        Task<IHealthVaultConnection> GetConnectionAsync();
    }
}
