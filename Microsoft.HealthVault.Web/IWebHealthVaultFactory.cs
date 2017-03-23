using Microsoft.HealthVault.Connection;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// An interface for the web HealthVault factory.
    /// </summary>
    public interface IWebHealthVaultFactory
    {
        /// <summary>
        /// Gets a connection for an online we connection
        /// </summary>
        /// <returns></returns>
        Task<IHealthVaultConnection> GetWebApplicationConnectionAsync();
    }
}
