using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Things;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault thing client. Used to access things associated with a particular record.
    /// </summary>
    public interface IThingClient : IClient
    {
        HealthRecordInfo Record { get; set; }

        /// <summary>
        /// Gets a Thing by it's unique identifier
        /// </summary>
        /// <param name="thingId">The unique identifier of the thing</param>
        /// <returns>A Task of HealthRecordItem</returns>
        Task<Thing> GetThingAsync(Guid thingId);

        /// <summary>
        /// Gets a collection of Things that match a given query. 
        /// </summary>
        /// <param name="query">An instance of <see cref="ThingQuery"/>.</param>
        /// <returns>ICollection of HealthRecordItem</returns>
        Task<ICollection<HealthRecordItem>> GetThingsAsync(ThingQuery query);

        /// <summary>
        /// Puts a collection of things.
        /// </summary>
        /// <param name="things">The collection of things to put.</param>
        /// <returns>A Task</returns>
        Task PutThingsAsync(ICollection<Thing> things);

        /// <summary>
        /// Overwrites a collection of things.
        /// </summary>
        /// <param name="things">The collection of things to put.</param>
        /// <returns>A Task</returns>
        Task OverwriteThings(ICollection<Thing> things);

        /// <summary>
        /// Removes a collection of things.
        /// </summary>
        /// <param name="things">The collection of things to put.</param>
        /// <returns>A Task</returns>
        Task RemoveThings(ICollection<Thing> things);
    }
}
