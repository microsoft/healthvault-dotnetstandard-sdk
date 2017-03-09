using System;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Clients
{
    public class ActionPlanClient : IActionPlanClient
    {
        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; }

        public HealthRecordInfo Record { get; set; }
    }
}
