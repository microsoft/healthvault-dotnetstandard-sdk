// ********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
// ********************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;

namespace Microsoft.HealthVault.Web.Mvc
{
    internal class ShellUrlBuilder
    {
        private readonly HttpContextBase _context;
        private readonly string _target;
        private readonly IDictionary<string, object> _params;

        internal ShellUrlBuilder(
            HttpContextBase context,
            string target,
            IDictionary<string, object> parameters)
        {   
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            _context = context;
            _target = target;
            _params = parameters;
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

        private static StringBuilder GetShellUrl()
        {
            string shellUrl = HealthVault.Config.HealthVaultShellUrl.OriginalString;
            StringBuilder targetUrl = new StringBuilder(shellUrl);
            if (!shellUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                targetUrl.Append("/");
            }

            return targetUrl;
        }

        private void EnsureBaseParameters()
        {
            EnsureAppID();
            EnsureAppQS();
            EnsureRedirect();
            EnsureAib();
        }

        private StringBuilder CreateQuery()
        {
            var builder = new StringBuilder();
            if (_params == null)
            {
                return builder;
            }

            bool first = true;
            foreach (KeyValuePair<string, object> parameter in _params)
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

        private void EnsureAppID()
        {
            if (!_params.ContainsKey("appid"))
            {
                _params.Add("appid", HealthVault.Config.ApplicationId);
            }
        }

        private void EnsureAppQS()
        {
            if (!_params.ContainsKey("actionqs"))
            {
                _params.Add("actionqs", _context.Request.Url.PathAndQuery);
            }
        }

        private void EnsureAib()
        {
            if (!_params.ContainsKey("aib")
                && HealthWebApplicationConfiguration.Current.MultiInstanceAware)
            {
                _params.Add("aib", "true");
            }
        }

        private void EnsureRedirect()
        {
            Uri redirectOverride = HealthVault.Config.ActionUrlRedirectOverride;
            if (_params.ContainsKey("redirect") || redirectOverride == null)
            {
                return;
            }

            // absolute or scheme-relative urls are already ok
            if (redirectOverride.IsAbsoluteUri || 
                redirectOverride.OriginalString.StartsWith("//", StringComparison.OrdinalIgnoreCase))
            {
                _params.Add("redirect", redirectOverride.OriginalString);
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
                _params["redirect"] = absoluteRedirect.OriginalString;
            }
        }
    }
}
