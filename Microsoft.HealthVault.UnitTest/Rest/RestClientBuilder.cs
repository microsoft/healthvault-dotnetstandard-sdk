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
        private readonly HealthVaultConfiguration healthVaultConfiguration;
        private readonly IConnectionInternal connection;
        private readonly IHealthWebRequestClient webClient;

        private RestClientBuilder()
        {
            this.healthVaultConfiguration = new HealthVaultConfiguration
            {
                RestHealthVaultUrl = new Uri("http://localhost")
            };
            this.connection = Substitute.For<IConnectionInternal>();
            this.webClient = Substitute.For<IHealthWebRequestClient>();
        }

        public static RestClientBuilder Create()
        {
            return new RestClientBuilder();
        }

        public RestClientBuilder WithResponseMessage(string body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            this.webClient.SendAsync(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>(), Arg.Any<bool>())
                .Returns(Task.FromResult(new HttpResponseMessage(statusCode) { Content = new StringContent(body) }));

            return this;
        }

        public RestClientBuilder WithErrorMessage(string message)
        {
            return this.WithResponseMessage($"{{ \"error\": {{ \"message\": \"{message}\" }} }}", HttpStatusCode.BadRequest);
        }

        public HealthVaultRestClient Build()
        {
            return new HealthVaultRestClient(this.healthVaultConfiguration, this.connection, this.webClient);
        }
    }
}