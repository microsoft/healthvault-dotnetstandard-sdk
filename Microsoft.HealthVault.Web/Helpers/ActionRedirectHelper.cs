// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Extensions;

namespace Microsoft.HealthVault.Web.Helpers
{
    internal class ActionRedirectHelper
    {
        private readonly WebHealthVaultConfiguration webHealthVaultConfiguration;

        public ActionRedirectHelper(IServiceLocator serviceLocator)
        {
            this.webHealthVaultConfiguration = serviceLocator.GetInstance<WebHealthVaultConfiguration>();
        }

        /// <summary>
        /// Gets the URL of the location and the query string for the
        /// specified action.
        /// </summary>
        /// 
        /// <param name="action">
        /// The action identifier used to tell the application which
        /// action page should be shown.
        /// </param>
        /// 
        /// <param name="actionQueryString">
        /// The query string parameters that should be passed to the
        /// target action URL.
        /// </param>
        /// 
        /// <returns>
        /// The full URL including query string of the action page to
        /// redirect to.
        /// </returns>
        /// 
        /// <remarks>
        /// The default implementation reads the action URL from the
        /// web.config file using the key "HV_Action" + action
        ///
        /// The currently supported actions are:
        ///  Home
        ///  ServiceAgreement
        ///  Help
        ///  AppAuthSuccess
        ///  AppAuthFailure
        ///  AppAuthReject
        ///  AppAuthInvalidRecord
        ///  CreateRecordFailure
        ///  CreateRecordCanceled
        ///  ReconcileComplete
        ///  ReconcileFailure
        ///  ReconcileCanceled
        ///  SelectedRecordChanged
        ///  SignOut
        ///  ShareRecordSuccess
        ///  ShareRecordFailed
        ///  Privacy
        /// </remarks>
        /// 
        public string TryGetTargetLocation(string action, string actionQueryString)
        {
            if (string.IsNullOrEmpty(action))
            {
                return null;
            }

            Uri uri = this.webHealthVaultConfiguration.TryGetActionUrl(action);

            if (uri == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(actionQueryString))
            {
                return uri.OriginalString;
            }

            if (actionQueryString.StartsWith("?", StringComparison.Ordinal))
            {
                return uri.OriginalString + actionQueryString;
            }

            return uri.OriginalString + "?" + actionQueryString;
        }

        public bool IsValidActionUrl(string appHost, string actionqs)
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
                if (actionUri.Host.Equals(appHost, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                foreach (var host in webHealthVaultConfiguration.AllowedRedirectSites.Split(','))
                {
                    if (actionUri.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
