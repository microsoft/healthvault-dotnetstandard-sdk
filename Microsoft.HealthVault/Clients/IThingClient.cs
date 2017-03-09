using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault thing client. Used to access things associated with a particular record.
    /// </summary>
    public interface IThingClient : IClient
    {
        HealthRecordInfo Record { get; set; }

        /// <summary>
        /// Gets a HealthRecordItem by its unique identifier
        /// </summary>
        /// <param name="thingId">The unique identifier of the thing</param>
        /// <returns>The thing requested</returns>
        Task<T> GetThingAsync<T>(Guid thingId)
            where T : IThing;

        /// <summary>
        /// Gets a collection of Things that match a given query. 
        /// </summary>
        /// <param name="query">An instance of <see cref="ThingQuery"/>.  Use this query to identify parameters for the search.</param>
        /// <returns>ICollection of HealthRecordItem</returns>
        Task<IReadOnlyCollection<HealthRecordItemCollection>> GetThingsAsync(ThingQuery query);

        /// <summary>
        /// Gets a collection of Things of the specific type. 
        /// </summary>
        /// <param name="query">An instance of <see cref="ThingQuery"/>.  If you leave this null, it will return all things of the specified type.</param>
        /// <returns>ICollection of HealthRecordItem</returns>
        Task<IReadOnlyCollection<T>> GetThingsAsync<T>(ThingQuery query = null)
                        where T : IThing;

        /// <summary>
        /// Creates a new collection of things.
        /// </summary>
        /// <param name="things">The collection of things to create.</param>
        Task CreateNewThingsAsync(ICollection<IThing> things);

        /// <summary>
        /// Updates a collection of things that already exists.
        /// </summary>
        /// <param name="things">The collection of things to update.</param>
        Task UpdateThingsAsync(ICollection<IThing> things);

        /// <summary>
        /// Removes a collection of things.
        /// </summary>
        /// <param name="things">The collection of things to put.</param>
        Task RemoveThings(ICollection<IThing> things);
    }
}
