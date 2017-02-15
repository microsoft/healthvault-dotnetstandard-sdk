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

namespace Microsoft.HealthVault.Web.Mvc
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
        /// Gets or sets whether to sign in as MRA or SRA.
        /// </summary>
        public bool Mra { get; set; }

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
                filterContext.Result = HealthVault.SignIn(
                    filterContext,
                    new { ismra = Mra });
                filterContext.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Redirects unauthorized users to sign in
        /// </summary>
        /// <param name="filterContext">The authorization context</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = HealthVault.SignIn(
                filterContext,
                new { ismra = Mra });
        }
    }
}
