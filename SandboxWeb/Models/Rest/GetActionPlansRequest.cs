using System;
using System.Net.Http;
using Microsoft.HealthVault.Rest;

namespace SandboxWeb.Models.Rest
{
    public class GetActionPlansRequest : HealthVaultRestMessage<ActionPlansResponseModel>
    {
        public GetActionPlansRequest(Guid recordId) : base(new Uri("/v3/ActionPlans", UriKind.Relative), recordId, HttpMethod.Get)
        {
        }
    }
}