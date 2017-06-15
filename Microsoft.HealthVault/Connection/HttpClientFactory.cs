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
