using System.Net.Http;

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Generates HttpClient instances.
    /// </summary>
    /// <remarks>The HttpClient instance should be re-used across the app, but since we cannot set ConnectionLeaseTimeout in .NET Standard
    /// we must instead re-generate it every so often to pick up DNS changes. See http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html </remarks>
    internal interface IHttpClientFactory
    {
        HttpClient GetOrCreateClient();
    }
}