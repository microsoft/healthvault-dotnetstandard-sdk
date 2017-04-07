// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.HealthVault.Web.Cookie;
using Microsoft.HealthVault.Web.Utilities;

namespace Microsoft.HealthVault.Web.Providers
{
    /// <summary>
    /// Provides SignIn and SignOut methods
    /// </summary>
    internal class AuthProvider
    {
        /// <summary>
        /// Sign in to HealthVault
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <returns>A redirect result to the HealthVault Shell sign in page</returns>
        public static RedirectResult SignIn(ControllerContext context)
        {
            return SignIn(context, null);
        }

        /// <summary>
        /// Sign in to HealthVault
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <param name="parameters">Parameters to be passed to the sign in page</param>
        /// <returns>A redirect result to the HealthVault Shell sign in page</returns>
        public static RedirectResult SignIn(ControllerContext context, object parameters)
        {
            return new RedirectResult(ShellUrls.Url(context, HealthVaultConstants.ShellRedirectTargets.Auth, parameters));
        }

        /// <summary>
        /// Sign out of HealthVault
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <returns>A redirect result to the HealthVault Shell sign out page</returns>
        public static RedirectResult SignOut(ControllerContext context)
        {
            return SignOut(context, null);
        }

        /// <summary>
        /// Sign out of HealthVault
        /// </summary>
        /// <param name="context">The current controller context</param>
        /// <param name="parameters">Parameters to be passed to the sign out page</param>
        /// <returns>A redirect result to the HealthVault Shell sign out page</returns>
        public static RedirectResult SignOut(ControllerContext context, object parameters)
        {
            var cookieManager = Ioc.Get<ICookieManager>();

            var httpContextBase = context.HttpContext;

            WebConnectionInfo webConnectionInfo = cookieManager.TryLoad(httpContextBase);
            string userAuthToken = webConnectionInfo.UserAuthToken;

            cookieManager.Clear(httpContextBase);
            httpContextBase.User = new GenericPrincipal(new HealthVaultIdentity(), null);

            if (!string.IsNullOrEmpty(userAuthToken))
            {
                var parameterValues = new RouteValueDictionary(parameters) { [HealthVaultConstants.ShellRedirectTargetQueryStrings.CredentialToken] = userAuthToken };
                return new RedirectResult(ShellUrls.Url(context, HealthVaultConstants.ShellRedirectTargets.AppSignOut, parameterValues));
            }

            return new RedirectResult(ShellUrls.Url(context, HealthVaultConstants.ShellRedirectTargets.AppSignOut, parameters));
        }
    }
}
