using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Transport
{
    internal interface IHealthWebRequestClient
    {
        /// <summary>
        /// Gets or sets the request compression method.
        /// </summary>
        ///
        string RequestCompressionMethod { get; set; }

        /// <summary>
        /// Sends a request asynchronously to the specified url.
        /// </summary>
        Task<HttpResponseMessage> SendAsync(Uri url, byte[] utf8EncodedXml, int utf8EncodedXmlLength, IDictionary<string, string> headers, CancellationToken token);
    }
}