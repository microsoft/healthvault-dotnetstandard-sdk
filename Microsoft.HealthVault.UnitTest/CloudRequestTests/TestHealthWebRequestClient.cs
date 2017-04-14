using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.CloudRequestTests
{
    class TestHealthWebRequestClient : IHealthWebRequestClient
    {
        public string RequestCompressionMethod { get; set; }

        public HttpResponseMessage message = Substitute.For<HttpResponseMessage>();

        public Dictionary<string, string> Headers { get; }

        public HttpClient CreateHttpClient()
        {
            return new HttpClient();
        }

        public Task<HttpResponseMessage> SendAsync(Uri url, byte[] utf8EncodedXml, int utf8EncodedXmlLength, IDictionary<string, string> headers, CancellationToken token)
        {
            return SendAsync(url, token);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, CancellationToken token, bool throwExceptionOnFailure = true)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> SendAsync(Uri url, CancellationToken token)
        {
            var content = Substitute.For<HttpContent>();
            content.ReadAsStreamAsync().Returns(new MemoryStream(Encoding.UTF8.GetBytes(SampleUtils.GetSampleContent("ThingSampleBloodPressure.xml"))));
            message.Content = content;
            await Task.FromResult(true);
            return message;
        }
    }
}
