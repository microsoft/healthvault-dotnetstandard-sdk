using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Exceptions;
using Microsoft.HealthVault.Soda;

namespace Microsoft.HealthVault.Client
{
    internal class ShellAuthService : IShellAuthService
    {
        private const string InstanceQueryParamKey = "instanceid=";

        private readonly IBrowserAuthBroker browserAuthBroker;
        private readonly ClientConfiguration clientConfiguration;

        public ShellAuthService(IBrowserAuthBroker browserAuthBroker, ClientConfiguration clientConfiguration)
        {
            this.browserAuthBroker = browserAuthBroker;
            this.clientConfiguration = clientConfiguration;
        }

        public async Task<string> ProvisionApplicationAsync(Uri shellUrl, Guid masterAppId, string appCreationToken, string appInstanceId)
        {
            if (shellUrl == null)
            {
                throw new ArgumentNullException(nameof(shellUrl));
            }

            if (appCreationToken == null)
            {
                throw new ArgumentNullException(nameof(appCreationToken));
            }

            if (appInstanceId == null)
            {
                throw new ArgumentNullException(nameof(appInstanceId));
            }

            string query = $"?appid={masterAppId}&appCreationToken={appCreationToken}&instanceName={appInstanceId}&ismra=true";
            if (this.clientConfiguration.AllowInstanceBounce)
            {
                query += "&aib=true";
            }

            UriBuilder builder = GetShellUriBuilder(shellUrl);
            builder.Query = query;

            Uri provisionUIUrl = builder.Uri;

            Uri successUri = await this.browserAuthBroker.AuthenticateAsync(
                provisionUIUrl,
                url => { return url.ToString().Contains("application/complete"); }).ConfigureAwait(false);

            string environmentInstanceId = ParseEnvironmentInstanceIdFromUri(successUri.ToString());
            if (environmentInstanceId == null)
            {
                throw new ShellAuthException($"Could not find instance ID in success URL {successUri}");
            }

            return environmentInstanceId;
        }

        public async Task AuthorizeAdditionalRecordsAsync(Uri shellUrl, Guid masterAppId)
        {
            if (shellUrl == null)
            {
                throw new ArgumentNullException(nameof(shellUrl));
            }

            string query = $"?appid={masterAppId}&ismra=true";

            UriBuilder builder = GetShellUriBuilder(shellUrl);
            builder.Query = query;

            Uri provisionUIUrl = builder.Uri;

            await this.browserAuthBroker.AuthenticateAsync(
                provisionUIUrl,
                url => { return url.ToString().Contains("application/complete"); }).ConfigureAwait(false);
        }

        private static UriBuilder GetShellUriBuilder(Uri shellUrl)
        {
            UriBuilder builder = new UriBuilder(shellUrl);
            builder.Path = "/redirect.aspx";

            return builder;
        }

        private static string ParseEnvironmentInstanceIdFromUri(string uri)
        {
            int instanceStartIndex = uri.IndexOf(InstanceQueryParamKey, StringComparison.OrdinalIgnoreCase);

            if (instanceStartIndex >= 0)
            {
                string instanceSubstring = uri.Substring(instanceStartIndex + InstanceQueryParamKey.Length);
                int instanceEndIndex = instanceSubstring.IndexOf("&", StringComparison.OrdinalIgnoreCase);
                if (instanceEndIndex > 0)
                {
                    return instanceSubstring.Substring(0, instanceEndIndex);
                }
                else
                {
                    return instanceSubstring;
                }
            }

            return null;
        }
    }
}
