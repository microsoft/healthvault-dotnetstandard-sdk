using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private readonly object factoryLock = new object();

        private readonly IMessageHandlerFactory messageHandlerFactory;
        private readonly IDateTimeService dateTimeService;

        private static readonly TimeSpan ClientExpirationTime = TimeSpan.FromMinutes(5);

        private HttpClient client;

        private DateTimeOffset? lastCreated;

        public HttpClientFactory(IMessageHandlerFactory messageHandlerFactory, IDateTimeService dateTimeService)
        {
            this.messageHandlerFactory = messageHandlerFactory;
            this.dateTimeService = dateTimeService;
        }

        public HttpClient GetOrCreateClient()
        {
            DateTimeOffset now = this.dateTimeService.UtcNow;
            if (!this.NeedsRefresh(now))
            {
                return this.client;
            }

            lock (this.factoryLock)
            {
                // Re-check inside lock
                if (this.NeedsRefresh(now))
                {
                    this.client = this.Create();
                    this.lastCreated = now;
                }

                return this.client;
            }
        }

        private bool NeedsRefresh(DateTimeOffset now)
        {
            return this.client == null || this.lastCreated == null || now > this.lastCreated.Value + ClientExpirationTime;
        }

        private HttpClient Create()
        {
            HttpClientHandler handler = this.messageHandlerFactory.Create();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return new HttpClient(handler);
        }
    }
}
