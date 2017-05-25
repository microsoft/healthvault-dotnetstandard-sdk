// Copyright (c) Microsoft Corporation.  All rights reserved.
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.UnitTest.Samples;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.CloudRequestTests
{
    internal class TestHealthWebRequestClient : IHealthWebRequestClient
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
