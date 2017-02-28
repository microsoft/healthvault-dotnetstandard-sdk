using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault thing client. Used to access things associated with a particular record.
    /// </summary>
    public interface IThingClient : IClient
    {
        HealthRecordInfo Record { get; set; }
    }
}
