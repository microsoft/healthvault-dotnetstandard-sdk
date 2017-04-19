using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.HealthVault.Clients;
using Microsoft.Rest;

namespace HealthVault.Client
{
    public class HealthVaultRestCredentials : ServiceClientCredentials
    {
        private readonly IHealthVaultRestClient client;
        private readonly Guid recordId;

        public HealthVaultRestCredentials(IHealthVaultRestClient client, Guid recordId)
        {
            this.client = client;
            this.recordId = recordId;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.client.AuthorizeRestRequest(request, this.recordId);
            return Task.FromResult(true);
        }
    }
}