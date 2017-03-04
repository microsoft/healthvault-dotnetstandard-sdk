using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Represents a connection for an application to the HealthVault service
    /// for operations
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// The HealthVault web-service instance.
        /// </summary>
        HealthServiceInstance ServiceInstance { get; }

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        IConfiguration ApplicationConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the session credential.
        /// </summary>
        /// <value>
        /// The session credential.
        /// </value>
        ISessionCredential SessionCredential { get; set; }

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

        /// <summary>
        /// Authenticates the connection.
        /// </summary>
        /// <remarks> This should depend on application platform - SODA vs WEB </remarks>
        Task AuthenticateAsync();
    }
}