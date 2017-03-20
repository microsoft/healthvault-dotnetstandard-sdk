using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// A client connection to HealthVault.
    /// </summary>
    public interface IClientHealthVaultConnection : IHealthVaultConnection
    {
        /// <summary>
        /// Prompt the user to authorize additional records for use in this app.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AuthorizeAdditionalRecordsAsync();
    }
}
