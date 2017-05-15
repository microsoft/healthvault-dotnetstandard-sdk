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