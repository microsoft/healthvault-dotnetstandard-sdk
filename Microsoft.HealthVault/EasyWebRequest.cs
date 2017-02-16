using Microsoft.HealthVault.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.HealthVault
{
    internal class EasyWebRequest
    {
        protected byte[] xmlRequest; // utf8Encoded
        protected int xmlRequestLength;

        internal EasyWebRequest()
        {
        }

        internal EasyWebRequest(byte[] utf8EncodedXml, int length)
        {
            this.xmlRequest = utf8EncodedXml;
            this.xmlRequestLength = length;
        }

        /// <summary>
        /// Sets the proxy to use with this instance of
        /// EasyWebRequest. To disable proxy usage, set this property to null.
        /// </summary>
        /// 
        internal IWebProxy WebProxy
        {
            get { return this.webProxy; }
            set { this.webProxy = value; }
        }
        private IWebProxy webProxy;

        internal bool EnableRequestCompression { get; set; } = true;

        /// <summary>
        /// Gets the dictionary of headers that will be added to the web request.
        /// </summary>
        ///
        internal Dictionary<string, string> Headers
        {
            get { return this.headers; }
        }
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        internal async Task<HttpResponseMessage> FetchAsync(Uri url, CancellationToken token)
        {
            HttpMethod method;
            if (this.xmlRequest == null)
            {
                method = HttpMethod.Get;
            }
            else
            {
                method = HttpMethod.Post;
            }

            HttpRequestMessage message = new HttpRequestMessage(method, url);
            foreach (KeyValuePair<string, string> headerPair in this.Headers)
            {
                message.Headers.Add(headerPair.Key, headerPair.Value);
            }

            HttpContent content = new ByteArrayContent(this.xmlRequest, 0, this.xmlRequestLength);
            if (this.EnableRequestCompression)
            {
                content  = new CompressedContent(content, "gzip");
            }

            message.Content = content;

            // TODO: Investigate singleton for HttpClient?
            using (HttpClient client = this.CreateHttpClient())
            {
                int retryCount = HealthApplicationConfiguration.Current.RetryOnInternal500Count;
                do
                {
                    HttpResponseMessage response = await client.SendAsync(message, token).ConfigureAwait(false);
                    if (response.StatusCode == HttpStatusCode.InternalServerError && retryCount > 0)
                    {
                        // If we have a 500 and have retries left, retry.
                        await Task.Delay(
                            TimeSpan.FromSeconds(HealthApplicationConfiguration.Current.RetryOnInternal500SleepSeconds),
                            token).ConfigureAwait(false);
                    }
                    else
                    {
                        // If we have a non-500 error or have run out of retries, throw.
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HealthHttpException("Http status code returned error", response.StatusCode);
                        }

                        // If we have a successful response, return it.
                        return response;
                    }

                    retryCount--;
                } while (retryCount >= 0);
            }

            // We should never get here but we need to make the compiler happy.
            throw new Exception("Unexpectedly got to the end of FetchAsync.");
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            if (this.WebProxy != null)
            {
                handler.Proxy = this.WebProxy;
            }

            return new HttpClient(handler);
        }
    }
}
