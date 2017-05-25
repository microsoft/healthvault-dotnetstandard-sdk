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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Rest;
using Microsoft.HealthVault.Transport;
using NSubstitute;

namespace Microsoft.HealthVault.UnitTest.Rest
{
    internal class RestClientBuilder
    {
        private readonly HealthVaultConfiguration _healthVaultConfiguration;
        private readonly IConnectionInternal _connection;
        private readonly IHealthWebRequestClient _webClient;

        private RestClientBuilder()
        {
            _healthVaultConfiguration = new HealthVaultConfiguration
            {
                RestHealthVaultUrl = new Uri("http://localhost")
            };
            _connection = Substitute.For<IConnectionInternal>();
            _webClient = Substitute.For<IHealthWebRequestClient>();
        }

        public static RestClientBuilder Create()
        {
            return new RestClientBuilder();
        }

        public RestClientBuilder WithResponseMessage(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _webClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>(), Arg.Any<bool>())
                .Returns(Task.FromResult(new HttpResponseMessage(statusCode) { Content = new StringContent(body) }));

            return this;
        }

        public RestClientBuilder WithErrorMessage(string message)
        {
            return WithResponseMessage($"{{ \"error\": {{ \"message\": \"{message}\" }} }}", HttpStatusCode.BadRequest);
        }

        public HealthVaultRestClient Build()
        {
            return new HealthVaultRestClient(_healthVaultConfiguration, _connection, _webClient);
        }
    }
}