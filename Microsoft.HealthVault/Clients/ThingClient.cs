using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// <inheritdoc cref="IThingClient"/>
    /// </summary>
    public class ThingClient : IThingClient
    {
        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; }

        public HealthRecordInfo Record { get; set; }

        public Task<Thing> GetThingAsync(Guid thingId)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<HealthRecordItem>> GetThingsAsync(ThingQuery query)
        {
            throw new NotImplementedException();
        }

        public Task PutThingsAsync(ICollection<Thing> things)
        {
            throw new NotImplementedException();
        }

        public Task OverwriteThings(ICollection<Thing> things)
        {
            throw new NotImplementedException();
        }

        public Task RemoveThings(ICollection<Thing> things)
        {
            throw new NotImplementedException();
        }
    }
}
