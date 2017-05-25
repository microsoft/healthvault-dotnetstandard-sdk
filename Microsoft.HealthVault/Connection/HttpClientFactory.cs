// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Net;
using System.Net.Http;
using Microsoft.HealthVault.Services;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Generates HttpClient instances.
    /// </summary>
    /// <remarks>The HttpClient instance should be re-used across the app, but since we cannot set ConnectionLeaseTimeout in .NET Standard
    /// we must instead re-generate it every so often to pick up DNS changes. See http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html </remarks>
    internal class HttpClientFactory : IHttpClientFactory
    {
        private readonly object _factoryLock = new object();

        private readonly IMessageHandlerFactory _messageHandlerFactory;
        private readonly IDateTimeService _dateTimeService;

        private static readonly TimeSpan s_clientExpirationTime = TimeSpan.FromMinutes(5);

        private HttpClient _client;

        private DateTimeOffset? _lastCreated;

        public HttpClientFactory(IMessageHandlerFactory messageHandlerFactory, IDateTimeService dateTimeService)
        {
            _messageHandlerFactory = messageHandlerFactory;
            _dateTimeService = dateTimeService;
        }

        public HttpClient GetOrCreateClient()
        {
            DateTimeOffset now = _dateTimeService.UtcNow;
            if (!NeedsRefresh(now))
            {
                return _client;
            }

            lock (_factoryLock)
            {
                // Re-check inside lock
                if (NeedsRefresh(now))
                {
                    _client = Create();
                    _lastCreated = now;
                }

                return _client;
            }
        }

        private bool NeedsRefresh(DateTimeOffset now)
        {
            return _client == null || _lastCreated == null || now > _lastCreated.Value + s_clientExpirationTime;
        }

        private HttpClient Create()
        {
            HttpClientHandler handler = _messageHandlerFactory.Create();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return new HttpClient(handler);
        }
    }
}
