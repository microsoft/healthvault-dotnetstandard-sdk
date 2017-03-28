using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Transport
{
    internal interface IHealthWebRequest
    {
        /// <summary>
        /// Gets or sets the request compression method.
        /// </summary>
        ///
        string RequestCompressionMethod { get; set; }

        /// <summary>
        /// Gets the dictionary of headers that will be added to the web request.
        /// </summary>
        ///
        Dictionary<string, string> Headers { get; }

        Task<HttpResponseMessage> FetchAsync(Uri url, CancellationToken token);
    }
}