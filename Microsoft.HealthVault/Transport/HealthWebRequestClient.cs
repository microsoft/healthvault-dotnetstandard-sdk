// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Transport
{
    internal class HealthWebRequestClient : IHealthWebRequestClient
    {
        private readonly HealthVaultConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;

        public HealthWebRequestClient(HealthVaultConfiguration config, IHttpClientFactory httpClientFactory)
        {
            this.configuration = config;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Gets or sets the request compression method.
        /// </summary>
        ///
        public string RequestCompressionMethod
        {
            get { return this.requestCompressionMethod; }

            set
            {
                this.requestCompressionMethod = value;

                if (string.IsNullOrEmpty(this.requestCompressionMethod))
                {
                    this.requestCompressionMethod = null;
                }
                else
                {
                    if (!string.Equals(
                            this.requestCompressionMethod,
                            "gzip",
                            StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(
                            this.requestCompressionMethod,
                            "deflate",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        throw new HealthServiceException(Resources.InvalidRequestCompressionMethod);
                    }
                }
            }
        }

        private string requestCompressionMethod = "gzip";

        public async Task<HttpResponseMessage> SendAsync(Uri url, byte[] utf8EncodedXml, int utf8EncodedXmlLength, IDictionary<string, string> headers, CancellationToken token)
        {
            HttpMethod method;
            if (utf8EncodedXml == null)
            {
                method = HttpMethod.Get;
            }
            else
            {
                method = HttpMethod.Post;
            }

            HttpRequestMessage message = new HttpRequestMessage(method, url);
            foreach (KeyValuePair<string, string> headerPair in headers)
            {
                message.Headers.Add(headerPair.Key, headerPair.Value);
            }

            HttpContent content = new ByteArrayContent(utf8EncodedXml, 0, utf8EncodedXmlLength);
            if (!string.IsNullOrEmpty(this.RequestCompressionMethod))
            {
                content = new CompressedContent(content, this.RequestCompressionMethod);
            }

            message.Content = content;

            HttpClient client = this.httpClientFactory.GetOrCreateClient();
            int retryCount = this.configuration.RetryOnInternal500Count;
            do
            {
                HttpResponseMessage response = await client.SendAsync(message, token).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.InternalServerError && retryCount > 0)
                {
                    // If we have a 500 and have retries left, retry.
                    await Task.Delay(
                        this.configuration.RetryOnInternal500SleepDuration,
                        token).ConfigureAwait(false);
                }
                else
                {
                    // If we have a non-500 error or have run out of retries, throw.
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HealthHttpException(Resources.HttpReturnedError, response.StatusCode);
                    }

                    // If we have a successful response, return it.
                    return response;
                }

                retryCount--;
            }
            while (retryCount >= 0);

            // We should never get here but we need to make the compiler happy.
            throw new Exception(Resources.UnexpectedError);
        }
    }
}
