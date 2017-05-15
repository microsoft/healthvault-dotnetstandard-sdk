// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Providers;

namespace Microsoft.HealthVault.Web.Attributes
{
    /// <summary>
    /// Indicates that this action or controller requires that the user be signed in to HealthVault.
    /// An unauthorized user will be redirected to HealthVault Shell to sign in.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1813:AvoidUnsealedAttributes",
        Justification = "Intended to be extendable")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireSignInAttribute : AuthorizeAttribute, IExceptionFilter
    {
        /// <summary>
        /// Handles exceptions from HealthVault
        /// </summary>
        /// <param name="filterContext">The exception context</param>
        /// <remarks>
        /// If a user is already signed in, but their session expires, this application may not
        /// realize this until the HealthVault service throws an exception. This handles those cases
        /// by redirecting the user to HealthVault Shell to re-sign in.
        /// </remarks>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            if (filterContext.ExceptionHandled)
            {
                return;
            }

            if (filterContext.Exception is HealthServiceCredentialTokenExpiredException ||
                filterContext.Exception is HealthServiceInvalidPersonException)
            {
                WebHealthVaultConfiguration webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();

                filterContext.Result = AuthProvider.SignIn(
                    filterContext,
                    new { ismra = webHealthVaultConfiguration.IsMultiRecordApp });
                filterContext.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Redirects unauthorized users to sign in
        /// </summary>
        /// <param name="filterContext">The authorization context</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            WebHealthVaultConfiguration webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();

            filterContext.Result = AuthProvider.SignIn(
                filterContext,
                new { ismra = webHealthVaultConfiguration.IsMultiRecordApp });
        }
    }
}
