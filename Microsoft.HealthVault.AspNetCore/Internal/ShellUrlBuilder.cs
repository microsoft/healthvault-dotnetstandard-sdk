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
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal class ShellUrlBuilder
    {
        private HttpContext context;
        private string target;
        private IDictionary<string, object> parameters;
        private HealthVaultConfiguration webHealthVaultConfiguration;

        internal ShellUrlBuilder(
            HttpContext context,
            string target,
            IDictionary<string, object> parameters)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.target = target ?? throw new ArgumentNullException(nameof(target));
            this.parameters = parameters;

            this.webHealthVaultConfiguration = Ioc.Get<HealthVaultConfiguration>();
        }

        internal string Generate()
        {
            this.EnsureBaseParameters();

            StringBuilder targetUrl = this.GetShellUrl();
            StringBuilder query = this.CreateQuery();

            if (!string.IsNullOrEmpty(this.target))
            {
                targetUrl.Append("redirect.aspx?target=");
                targetUrl.Append(this.target);
                if (query.Length > 0)
                {
                    targetUrl.Append("&targetqs=");
                    targetUrl.Append(WebUtility.UrlEncode(query.ToString()));
                }
            }

            return targetUrl.ToString();
        }

        public override string ToString()
        {
            return this.Generate();
        }

        private StringBuilder GetShellUrl()
        {
            string shellUrl = this.webHealthVaultConfiguration.DefaultHealthVaultShellUrl.OriginalString;

            StringBuilder targetUrl = new StringBuilder(shellUrl);
            if (!shellUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                targetUrl.Append("/");
            }

            return targetUrl;
        }

        private void EnsureBaseParameters()
        {
            this.EnsureAppId();
            this.EnsureAppQs();
            this.EnsureRedirect();
            this.EnsureAib();
        }

        private StringBuilder CreateQuery()
        {
            var builder = new StringBuilder();
            if (this.parameters == null)
            {
                return builder;
            }

            bool first = true;
            foreach (KeyValuePair<string, object> parameter in this.parameters)
            {
                if (!first)
                {
                    builder.Append("&");
                }

                first = false;
                builder.Append(parameter.Key);
                builder.Append("=");
                builder.Append(WebUtility.UrlEncode(
                    Convert.ToString(parameter.Value, CultureInfo.InvariantCulture)));
            }

            return builder;
        }

        private void EnsureAppId()
        {
            if (!this.parameters.ContainsKey("appid"))
            {
                this.parameters.Add("appid", this.webHealthVaultConfiguration.MasterApplicationId);
            }
        }

        private void EnsureAppQs()
        {
            if (!this.parameters.ContainsKey("actionqs"))
            {
                this.parameters.Add("actionqs", this.context.Request?.Path);
            }
        }

        private void EnsureAib()
        {
            if (!this.parameters.ContainsKey("aib")
                && this.webHealthVaultConfiguration.MultiInstanceAware)
            {
                this.parameters.Add("aib", "true");
            }
        }

        private void EnsureRedirect()
        {
            if (this.parameters.ContainsKey("redirect"))
            {
                return;
            }
            throw new NotSupportedException();
        }
    }
}
