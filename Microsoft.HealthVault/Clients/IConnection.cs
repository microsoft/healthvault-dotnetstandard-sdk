using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the root HealthVault client. This client is used to make requests that do not require authentication as well as to access
    /// area specific clients.
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Gets a client of a given type.
        /// </summary>
        /// <typeparam name="T">The type of the client to retrieve</typeparam>
        /// <returns>A client instance</returns>
        T GetClient<T>()
            where T : IClient;

        /// <summary>
        /// A client that can be used to access information about the platform.
        /// </summary>
        IPlatformClient PlatformClient { get; }

        /// <summary>
        /// A client that can be used to access information and records associated with the currently athenticated user.
        /// </summary>
        IPersonClient PersonClient { get; }

        /// <summary>
        /// A client that can be used to access vocabularies.
        /// </summary>
        IVocabularyClient VocabularyClient { get; }

        /// <summary>
        /// Gets a client that can be used to access things associated with a particular record.
        /// </summary>
        /// <param name="record">The record to associate the thing client with</param>
        /// <returns>An instance implementing IThingClient</returns>
        IThingClient GetThingClient(HealthRecordInfo record);

        /// <summary>
        /// Gets a client that can be used to access action plans associated with a particular record
        /// </summary>
        /// <param name="record">The record to associate the action plan client with</param>
        /// <returns>An instance implementing IActionPlanClient</returns>
        IActionPlanClient GetActionPlanClient(HealthRecordInfo record);
    }
}
