// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.HealthVault.Web.Configuration;

namespace Microsoft.HealthVault.Web.Utilities
{
    internal class ShellUrlBuilder
    {
        private Uri shellUri;
        private string applicationPath;
        private string target;
        private IDictionary<string, object> parameters;
        private WebHealthVaultConfiguration webHealthVaultConfiguration;

        public ShellUrlBuilder(
            Uri shellUri,
            string target,
            string applicationPath,
            IDictionary<string, object> parameters)
        {
            this.shellUri = shellUri ?? throw new ArgumentNullException(nameof(shellUri));
            this.target = target;
            this.applicationPath = applicationPath;
            this.parameters = parameters;

            this.webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();
        }

        public override string ToString()
        {
            return Generate();
        }

        internal string Generate()
        {
            EnsureBaseParameters();

            StringBuilder targetUrl = GetShellUrl();
            StringBuilder query = CreateQuery();

            if (!string.IsNullOrEmpty(this.target))
            {
                targetUrl.Append("redirect.aspx?target=");
                targetUrl.Append(target);
                if (query.Length > 0)
                {
                    targetUrl.Append("&targetqs=");
                    targetUrl.Append(HttpUtility.UrlEncode(query.ToString()));
                }
            }

            return targetUrl.ToString();
        }

        internal void EnsureBaseParameters()
        {
            EnsureAppId();
            EnsureAppQs();
            EnsureRedirect();
            EnsureAib();
        }

        internal void EnsureAppId()
        {
            if (!this.parameters.ContainsKey("appid"))
            {
                this.parameters.Add("appid", this.webHealthVaultConfiguration.MasterApplicationId);
            }
        }

        internal void EnsureAppQs()
        {
            if (!this.parameters.ContainsKey("actionqs"))
            {
                this.parameters.Add("actionqs", shellUri.PathAndQuery);
            }
        }

        internal void EnsureRedirect()
        {
            Uri redirectOverride = this.webHealthVaultConfiguration.ActionUrlRedirectOverride;
            if (this.parameters.ContainsKey("redirect") || redirectOverride == null)
            {
                return;
            }

            // absolute or scheme-relative urls are already ok
            if (redirectOverride.IsAbsoluteUri ||
                redirectOverride.OriginalString.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                this.parameters.Add("redirect", redirectOverride.OriginalString);
            }
            else
            {
                // we have to make it absolute so we can redirect to it
                string redirect = redirectOverride.OriginalString;
                if (!redirect.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    if (this.applicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        redirect = this.applicationPath + redirect;
                    }
                    else
                    {
                        redirect = this.applicationPath + "/" + redirect;
                    }
                }

                Uri absoluteRedirect = new Uri(this.shellUri, redirect);
                this.parameters["redirect"] = absoluteRedirect.OriginalString;
            }
        }

        internal void EnsureAib()
        {
            if (!this.parameters.ContainsKey("aib")
                && this.webHealthVaultConfiguration.MultiInstanceAware)
            {
                this.parameters.Add("aib", "true");
            }
        }

        internal StringBuilder GetShellUrl()
        {
            string shellUrl = this.webHealthVaultConfiguration.DefaultHealthVaultShellUrl.OriginalString;

            StringBuilder targetUrl = new StringBuilder(shellUrl);
            if (!shellUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                targetUrl.Append("/");
            }

            return targetUrl;
        }

        internal StringBuilder CreateQuery()
        {
            var builder = new StringBuilder();
            if (parameters == null)
            {
                return builder;
            }

            bool first = true;
            foreach (KeyValuePair<string, object> parameter in parameters)
            {
                if (!first)
                {
                    builder.Append("&");
                }

                first = false;
                builder.Append(parameter.Key);
                builder.Append("=");
                builder.Append(HttpUtility.UrlEncode(
                    Convert.ToString(parameter.Value, CultureInfo.InvariantCulture)));
            }

            return builder;
        }
    }
}
