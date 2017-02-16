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

using Microsoft.HealthVault.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// A simple controller that handles users coming back from HealthVault Shell
    /// and redirects them.
    /// </summary>
    public class RedirectController : Controller
    {
        /// <summary>
        /// Handles the basic request to /Redirect
        /// </summary>
        /// <param name="target">The target of the redirect</param>
        /// <param name="actionqs">A query string that was originally passed to HealthVault Shell.
        /// If it is a url, it will be redirected to.</param>
        /// <returns>A redirect to either the actionqs (if it is a url) or the application homepage</returns>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "actionqs",
            Justification = "actionqs is the query parameter, shortened for url brevity")]
        [SuppressMessage(
            "Microsoft.Security.Web.Configuration",
            "CA3147:MarkVerbHandlersWithValidateAntiforgeryToken",
            Justification = "We are doing cross posting so we are validating via referrer and encrypted credential")]
        public virtual ActionResult Index(string target, string actionqs)
        {
            string targetLocation = GetTargetLocation(target, actionqs);
            if (!string.IsNullOrEmpty(targetLocation))
            {
                return Redirect(targetLocation);
            }

            if (IsValidUrl(actionqs))
            {
                return Redirect(actionqs);
            }

            return Redirect(Request.ApplicationPath);
        }

        private bool IsValidUrl(string actionqs)
        {
            // relative urls are ok, except those that are scheme relative, which could have
            // different host
            if (Uri.IsWellFormedUriString(actionqs, UriKind.Relative) &&
               !actionqs.StartsWith("//", StringComparison.Ordinal))
            {
                return true;
            }

            if (Uri.IsWellFormedUriString(actionqs, UriKind.RelativeOrAbsolute))
            {
                Uri actionUri = new Uri(actionqs, UriKind.RelativeOrAbsolute);
                if (actionUri.Host.Equals(Request.Url.Host, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                foreach (var host in HealthVault.Config.AllowedRedirectSites.Split(','))
                {
                    if (actionUri.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static string GetTargetLocation(string target, string actionqs)
        {
            if (string.IsNullOrEmpty(target))
            {
                return null;
            }

            Uri uri;
            try
            {
                uri = HealthVault.Config.GetActionUrl(target);
            }
            catch (HealthServiceException)
            {
                // our .Net SDK throws on missing config values
                uri = null;
            }

            if (uri == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(actionqs))
            {
                return uri.OriginalString;
            }

            if (actionqs.StartsWith("?", StringComparison.Ordinal))
            {
                return uri.OriginalString + actionqs;
            }

            return uri.OriginalString + "?" + actionqs;
        }
    }
}
