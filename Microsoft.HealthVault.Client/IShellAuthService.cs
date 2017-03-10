using System;
using System.Threading.Tasks;

namespace Microsoft.HealthVault.Client
{
    internal interface IShellAuthService
    {
        Task<string> ProvisionApplicationAsync(Uri shellUrl, Guid masterAppId, string appCreationToken, string appInstanceId);

        Task AuthorizeAdditionalRecordsAsync(Uri shellUrl, Guid masterAppId);
    }
}