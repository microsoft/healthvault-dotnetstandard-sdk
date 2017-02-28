using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Represents an individual request to a HealthVault service.
    /// The class wraps up the XML generation and web request/response.
    /// </summary>
    public interface IHealthServiceRequest
    {
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        /// <value>
        /// The correlation identifier.
        /// </value>
        Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the culture code.
        /// </summary>
        /// <value>
        /// The culture code.
        /// </value>
        string CultureCode { get; set; }

        /// <summary>
        /// Gets or sets the impersonated person identifier.
        /// </summary>
        /// <value>
        /// The impersonated person identifier.
        /// </value>
        Guid ImpersonatedPersonId { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        string MethodName { get; set; }

        /// <summary>
        /// Gets or sets the method version.
        /// </summary>
        /// <value>
        /// The method version.
        /// </value>
        int? MethodVersion { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        string Parameters { get; set; }

        /// <summary>
        /// Gets or sets the record identifier.
        /// </summary>
        /// <value>
        /// The record identifier.
        /// </value>
        Guid RecordId { get; set; }

        /// <summary>
        /// Gets or sets the request compression method.
        /// </summary>
        /// <value>
        /// The request compression method.
        /// </value>
        string RequestCompressionMethod { get; set; }

        /// <summary>
        /// Gets or sets the respose identifier.
        /// </summary>
        /// <value>
        /// The respose identifier.
        /// </value>
        Guid ResposeId { get; set; }

        /// <summary>
        /// Gets or sets the timeout seconds.
        /// </summary>
        /// <value>
        /// The timeout seconds.
        /// </value>
        int TimeoutSeconds { get; set; }

        /// <summary>
        /// Executes the asynchronous.
        /// </summary>
        /// <returns>HealthServiceResponseData</returns>
        Task<HealthServiceResponseData> ExecuteAsync();
    }
}