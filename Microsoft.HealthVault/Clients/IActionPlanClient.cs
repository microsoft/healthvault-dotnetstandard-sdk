using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Clients
{
    public interface IActionPlanClient : IClient
    {
        HealthRecordInfo Record { get; set; }
    }
}
