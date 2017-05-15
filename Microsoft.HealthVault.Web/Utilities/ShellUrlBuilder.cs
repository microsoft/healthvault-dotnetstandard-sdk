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
        private HttpContextBase _context;
        private string _target;
        private IDictionary<string, object> _parameters;
        private WebHealthVaultConfiguration _webHealthVaultConfiguration;

        internal ShellUrlBuilder(
            HttpContextBase context,
            string target,
            IDictionary<string, object> parameters)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _parameters = parameters;

            _webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();
        }

        internal string Generate()
        {
            EnsureBaseParameters();

            StringBuilder targetUrl = GetShellUrl();
            StringBuilder query = CreateQuery();

            if (!string.IsNullOrEmpty(_target))
            {
                targetUrl.Append("redirect.aspx?target=");
                targetUrl.Append(_target);
                if (query.Length > 0)
                {
                    targetUrl.Append("&targetqs=");
                    targetUrl.Append(HttpUtility.UrlEncode(query.ToString()));
                }
            }

            return targetUrl.ToString();
        }

        public override string ToString()
        {
            return Generate();
        }

        private StringBuilder GetShellUrl()
        {
            string shellUrl = _webHealthVaultConfiguration.DefaultHealthVaultShellUrl.OriginalString;

            StringBuilder targetUrl = new StringBuilder(shellUrl);
            if (!shellUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                targetUrl.Append("/");
            }

            return targetUrl;
        }

        private void EnsureBaseParameters()
        {
            EnsureAppId();
            EnsureAppQs();
            EnsureRedirect();
            EnsureAib();
        }

        private StringBuilder CreateQuery()
        {
            var builder = new StringBuilder();
            if (_parameters == null)
            {
                return builder;
            }

            bool first = true;
            foreach (KeyValuePair<string, object> parameter in _parameters)
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

        private void EnsureAppId()
        {
            if (!_parameters.ContainsKey("appid"))
            {
                _parameters.Add("appid", _webHealthVaultConfiguration.MasterApplicationId);
            }
        }

        private void EnsureAppQs()
        {
            if (!_parameters.ContainsKey("actionqs"))
            {
                _parameters.Add("actionqs", _context.Request?.Url?.PathAndQuery);
            }
        }

        private void EnsureAib()
        {
            if (!_parameters.ContainsKey("aib")
                && _webHealthVaultConfiguration.MultiInstanceAware)
            {
                _parameters.Add("aib", "true");
            }
        }

        private void EnsureRedirect()
        {
            Uri redirectOverride = _webHealthVaultConfiguration.ActionUrlRedirectOverride;
            if (_parameters.ContainsKey("redirect") || redirectOverride == null)
            {
                return;
            }

            // absolute or scheme-relative urls are already ok
            if (redirectOverride.IsAbsoluteUri ||
                redirectOverride.OriginalString.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                _parameters.Add("redirect", redirectOverride.OriginalString);
            }
            else
            {
                // we have to make it absolute so we can redirect to it
                string redirect = redirectOverride.OriginalString;
                if (!redirect.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    if (_context.Request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        redirect = _context.Request.ApplicationPath + redirect;
                    }
                    else
                    {
                        redirect = _context.Request.ApplicationPath + "/" + redirect;
                    }
                }

                Uri absoluteRedirect = new Uri(_context.Request.Url, redirect);
                _parameters["redirect"] = absoluteRedirect.OriginalString;
            }
        }
    }
}
