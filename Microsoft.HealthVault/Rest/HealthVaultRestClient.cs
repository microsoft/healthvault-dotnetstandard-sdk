﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Transport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.HealthVault.Rest
{
    internal sealed class HealthVaultRestClient : IHealthVaultRestClient
    {
        private readonly HealthVaultConfiguration _configuration;
        private readonly IConnectionInternal _connection;
        private readonly IHealthWebRequestClient _client;
        private readonly JsonSerializer _serializer = new JsonSerializer();

        public HealthVaultRestClient(
            HealthVaultConfiguration configuration,
            IConnectionInternal connection,
            IHealthWebRequestClient client)
        {
            _configuration = configuration;
            _connection = connection;
            _client = client;
        }

        public Guid? CorrelationId { get; set; }

        public async Task AuthorizeRestRequestAsync(HttpRequestMessage message, Guid recordId)
        {
            if (!string.IsNullOrEmpty(_connection.SessionCredential?.Token))
            {
                if (_connection.SessionCredential.IsExpired())
                {
                    await _connection.RefreshSessionAsync(CancellationToken.None);
                }

                var parts = new List<string>
                {
                    $"app-token={_connection.SessionCredential.Token}"
                };

                string connectionHeader = _connection.GetRestAuthSessionHeader();
                if (!string.IsNullOrEmpty(connectionHeader))
                {
                    parts.Add(connectionHeader);
                }

                if (recordId != Guid.Empty)
                {
                    parts.Add($"record-id={recordId}");
                }

                message.Headers.Authorization = new AuthenticationHeaderValue("MSH-V1", string.Join(",", parts));
            }
        }

        public async Task<T> ExecuteAsync<T>(IHealthVaultRestMessage<T> request)
        {
            using (var httpRequestMessage = await CreateHttpRequestMessageAsync(request))
            using (var httpResponseMessage = await _client.SendAsync(httpRequestMessage, CancellationToken.None, false))
            using (var stream = await ProcessHttpResponseMessage(httpResponseMessage))
            using (var streamReader = new StreamReader(stream))
            using (var reader = new JsonTextReader(streamReader))
            {
                return _serializer.Deserialize<T>(reader);
            }
        }

        private async Task<HttpRequestMessage> CreateHttpRequestMessageAsync<T>(IHealthVaultRestMessage<T> request)
        {
            Uri requestPath;
            if (request.Path.IsAbsoluteUri == false)
            {
                var requestBuilder = new UriBuilder(_configuration.RestHealthVaultUrl)
                {
                    Path = request.Path.ToString()
                };
                requestPath = requestBuilder.Uri;
            }
            else
            {
                requestPath = request.Path;
            }

            var httpRequestMessage = new HttpRequestMessage(request.HttpMethod, requestPath);
            httpRequestMessage.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("gzip"));
            httpRequestMessage.Headers.AcceptEncoding.Add(StringWithQualityHeaderValue.Parse("deflate"));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(RestConstants.JsonContentType));
            httpRequestMessage.Headers.Add(RestConstants.VersionHeader, request.ApiVersion.ToString());
            await AuthorizeRestRequestAsync(httpRequestMessage, request.RecordId);

            // TODO: Fix useragent string
            httpRequestMessage.Headers.UserAgent.ParseAdd(string.Format(RestConstants.MSHSDKVersion, "Unknown", "Unknown"));

            if (CorrelationId != Guid.Empty)
            {
                httpRequestMessage.Headers.Add(RestConstants.CorrelationIdHeaderName, CorrelationId.ToString());
            }

            if (request.CustomHeaders != null)
            {
                foreach (var header in request.CustomHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (request is IHealthVaultRestMessageContent hasContent)
            {
                httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(hasContent.Content), Encoding.UTF8, RestConstants.JsonContentType);
            }

            return httpRequestMessage;
        }

        private async Task<Stream> ProcessHttpResponseMessage(HttpResponseMessage message)
        {
            if (!message.IsSuccessStatusCode)
            {
                string errorMessage;

                try
                {
                    var content = await message.Content.ReadAsStringAsync();
                    var model = JToken.Parse(content);
                    errorMessage = model["error"]["message"].ToString();
                }
                catch
                {
                    throw new HealthHttpException(Resources.HttpReturnedError, message.StatusCode);
                }

                throw new HealthHttpException(errorMessage, message.StatusCode);
            }

            return await message.Content.ReadAsStreamAsync();
        }
    }
}