using System;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Sample
    /// </summary>
    /// <seealso cref="IConnection" />
    public abstract class ApplicationConnectionV2 : IConnection
    {
        /// <summary>
        /// The HealthVault web-service instance.
        /// </summary>
        public HealthServiceInstance ServiceInstance { get; set; }

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        /// <value>
        /// The application configuration.
        /// </value>
        public IConfiguration ApplicationConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the session credential.
        /// </summary>
        /// <value>
        /// The session credential.
        /// </value>
        public ISessionCredential SessionCredential { get; set; }

        /// <inheritdoc cref="IConnection"/>
        public T GetClient<T>()
            where T : IClient
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IConnection"/>
        public IPlatformClient PlatformClient { get; }

        /// <inheritdoc cref="IConnection"/>
        IPersonClient IConnection.PersonClient { get; }

        /// <inheritdoc cref="IConnection"/>
        public IVocabularyClient VocabularyClient { get; }

        /// <inheritdoc cref="IConnection"/>
        public IThingClient GetThingClient(HealthRecordInfo record)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IConnection"/>
        public IActionPlanClient GetActionPlanClient(HealthRecordInfo record)
        {
            throw new NotImplementedException();
        }

        // All clients will have a pointer to connection
        // they execute with executeasync method of Connection.
        public IPersonClient PersonClient { get; set; }

        internal Task<HealthServiceResponseData> ExecuteAsync(string methodName, int methodVersion)
        {
            // HealthServiceRequest request = new HealthServiceRequest(this, methodName, methodVersion);
            // await request.ExecuteAsync();
            // Modify the code in ExecuteAsync and use BuildRequestXml to take into account 
            // the different connections

            return Task.FromResult(new HealthServiceResponseData());
        }

        /// <summary>
        /// Constructs the authentication session Header.
        /// </summary>
        /// <param name="xpath">The xpath.</param>
        /// <returns>AUTH-SESSION XML</returns>
        /// <remarks>SODA VS OFFLINE VS WEB have different auth session headers</remarks>
        internal abstract string ConstructAuthSessionHeader(XPathNavigator xpath);

        /// <summary>
        /// Authenticates the connection.
        /// </summary>
        /// <remarks>
        /// This should depend on application platform - SODA vs WEB
        /// </remarks>
        public abstract Task AuthenticateAsync();
    }
}
