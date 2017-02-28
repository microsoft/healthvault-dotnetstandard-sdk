using Microsoft.HealthVault.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// The base interface for HealthVault clients
    /// </summary>
    public interface IClient
    {
        // TODO: This connection will become an instance of IConnection once that work is done.

        /// <summary>
        /// The HealthVault connection object
        /// </summary>
        HealthServiceConnection Connection { get; set; }

        /// <summary>
        /// An optional identifier that can be used to correlate a request. 
        /// </summary>
        Guid CorrelationId { get; set; }

        /// <summary>
        /// The unique identifier of the last completed response.
        /// </summary>
        Guid LastResponseId { get; }
    }
}
