// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
        /// <summary>
        /// Gets a ThingBase by its unique identifier
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="thingId">The unique identifier of the thing</param>
        /// <returns>The thing requested</returns>
        Task<T> GetThingAsync<T>(Guid recordId, Guid thingId)
            where T : IThing;

        /// <summary>
        /// Gets a collection of Things that match a given query.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="query">An instance of <see cref="ThingQuery"/>.  Use this query to identify parameters for the search.</param>
        /// <returns>A collection of things returned from the query.</returns>
        Task<ThingCollection> GetThingsAsync(Guid recordId, ThingQuery query);

        /// <summary>
        /// Gets a collection of Things that match a given query.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="queries">A collection of <see cref="ThingQuery"/> objects.  Use these queries to identify parameters for the search.</param>
        /// <returns>A group of results for the queries. Each <see cref="ThingCollection"/> corresponds to a <see cref="ThingQuery"/> passed in.</returns>
        Task<IReadOnlyCollection<ThingCollection>> GetThingsAsync(Guid recordId, IEnumerable<ThingQuery> queries);

        /// <summary>
        /// Gets a collection of Things of the specific type.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="query">An instance of <see cref="ThingQuery"/>.  If you leave this null, it will return all things of the specified type.</param>
        /// <returns>ICollection of ThingBase</returns>
        Task<IReadOnlyCollection<T>> GetThingsAsync<T>(Guid recordId, ThingQuery query = null)
                        where T : IThing;

        /// <summary>
        /// Creates a new collection of things, and updates each item with a Key that has its thing id and version id.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="things">The collection of things to create.</param>
        /// <remarks>
        /// If a thing with a thing id is provided the object will be updated instead of failing with an exception.
        /// </remarks>
        /// <exception cref="Microsoft.HealthVault.Exceptions.HealthServiceException">
        /// An exception will be thrown if a thing id or version stamp is provided for the new object and
        /// the object doesn't exist.
        /// </exception>
        Task CreateNewThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing;

        /// <summary>
        /// Updates a collection of things that already exists.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="things">The collection of things to update.</param>
        Task UpdateThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing;

        /// <summary>
        /// Removes a collection of things.
        /// </summary>
        /// <param name="recordId">The health record's ID.</param>
        /// <param name="things">The collection of things to put.</param>
        Task RemoveThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing;
    }
}
