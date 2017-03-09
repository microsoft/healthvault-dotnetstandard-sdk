using Microsoft.HealthVault.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// An interface for the web HealthVault factory.
    /// </summary>
    public interface IWebHealthVaultFactory : IHealthVaultFactoryBase
    {
        /// <summary>
        /// Gets a connection for an online we connection
        /// </summary>
        /// <returns></returns>
        Task<IConnection> GetWebApplicationConnectionAsync();
    }
}
