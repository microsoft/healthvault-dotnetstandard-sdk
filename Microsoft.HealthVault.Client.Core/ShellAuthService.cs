// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using Microsoft.HealthVault.Client.Exceptions;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.Client
{
    internal class ShellAuthService : IShellAuthService
    {
        private const string InstanceQueryParamKey = "instanceid=";

        private readonly IBrowserAuthBroker _browserAuthBroker;
        private readonly HealthVaultConfiguration _clientHealthVaultConfiguration;

        public ShellAuthService(IBrowserAuthBroker browserAuthBroker, HealthVaultConfiguration clientHealthVaultConfiguration)
        {
            _browserAuthBroker = browserAuthBroker;
            _clientHealthVaultConfiguration = clientHealthVaultConfiguration;
        }

        private string MraString => _clientHealthVaultConfiguration.IsMultiRecordApp ? "true" : "false";

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

            string query = $"appid={masterAppId}&appCreationToken={Uri.EscapeDataString(appCreationToken)}&instanceName={Uri.EscapeDataString(appInstanceId)}&ismra={MraString}&mobile=true";
            if (_clientHealthVaultConfiguration.MultiInstanceAware)
            {
                query += "&aib=true";
            }

            Uri successUri = await AuthenticateInBrowserAsync(shellUrl, query).ConfigureAwait(false);
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

            string query = $"?appid={masterAppId}&ismra={MraString}";

            await AuthenticateInBrowserAsync(shellUrl, query).ConfigureAwait(false);
        }

        private async Task<Uri> AuthenticateInBrowserAsync(Uri shellUrl, string query)
        {
            UriBuilder provisionBuilder = GetShellUriBuilder(shellUrl);
            string fullQuery = $"target={HealthVaultConstants.ShellRedirectTargets.CreateApplication}&targetqs=" + Uri.EscapeDataString(query);
            provisionBuilder.Query = fullQuery;
            Uri provisionUIUrl = provisionBuilder.Uri;

            UriBuilder endUriBuilder = new UriBuilder(shellUrl);
            endUriBuilder.Path = "application/complete";
            Uri stopUrl = endUriBuilder.Uri;

            return await _browserAuthBroker.AuthenticateAsync(provisionUIUrl, stopUrl).ConfigureAwait(false);
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
