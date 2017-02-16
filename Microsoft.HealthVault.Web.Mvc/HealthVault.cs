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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// Basic and common methods for interacting with HealthVault
    /// </summary>
    public static class HealthVault
    {
        /// <summary>
        /// The current configuration values.
        /// </summary>
        public static HealthWebApplicationConfiguration Config
        {
            get { return HealthWebApplicationConfiguration.Current; }
            set { HealthWebApplicationConfiguration.Current = value; }
        }

        /// <summary>
        /// Registers routes to HealthVault controllers.
        /// </summary>
        /// <param name="routes">The application's route collection</param>
        /// <remarks>Call this from your global.asax if you wish to use the supplied redirect controller.</remarks>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                "Redirect",
                "Redirect/{action}",
                new { controller = "Redirect", action = "Index" },
                new[] { "Microsoft.Health.Web.Mvc" });
        }

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
            return new RedirectResult(Shell.Url(context, "AUTH", parameters));
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
            var manager = new PersonInfoManager(context.HttpContext);
            string token = manager.CredentialToken;
            manager.Clear();

            if (!string.IsNullOrEmpty(token))
            {
                var parameterValues = new RouteValueDictionary(parameters) { ["credtoken"] = token };
                return new RedirectResult(Shell.Url(context, "APPSIGNOUT", parameterValues));
            }

            return new RedirectResult(Shell.Url(context, "APPSIGNOUT", parameters));
        }

        /// <summary>
        /// Utility methods for creating various urls targetting HealthVault Shell
        /// </summary>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1034:NestedTypesShouldNotBeVisible",
            Justification = "Nested for readability purposes.")]
        public static class Shell
        {
            /// <summary>
            /// Creates a url to the Shell homepage
            /// </summary>
            /// <param name="context">The current controller context</param>
            /// <returns>A string containing the Shell homepage url.</returns>
            public static string Url(ControllerContext context)
            {
                return Url(context, null, new RouteValueDictionary());
            }

            /// <summary>
            /// Creates a url to a specific Shell target
            /// </summary>
            /// <param name="context">The current controller context</param>
            /// <param name="target">The name of the target</param>
            /// <returns>A string containing the Shell url for the given target</returns>
            public static string Url(ControllerContext context, string target)
            {
                return Url(context, target, new RouteValueDictionary());
            }

            /// <summary>
            /// Creates a url to a specific Shell target
            /// </summary>
            /// <param name="context">The current controller context</param>
            /// <param name="target">The name of the target</param>
            /// <param name="parameters">Parameters to pass to the Shell page</param>
            /// <returns>A string containing the Shell url for the given target</returns>
            public static string Url(ControllerContext context, string target, object parameters)
            {
                return Url(context, target, new RouteValueDictionary(parameters));
            }

            /// <summary>
            /// Creates a url to a specific Shell target
            /// </summary>
            /// <param name="context">The current controller context</param>
            /// <param name="target">The name of the target</param>
            /// <param name="parameters">Parameters to pass to the Shell page</param>
            /// <returns>A string containing the Shell url for the given target</returns>
            public static string Url(
                ControllerContext context,
                string target,
                IDictionary<string, object> parameters)
            {
                HttpContextBase httpContext = context.HttpContext;
                var urlBuilder = new ShellUrlBuilder(httpContext, target, parameters);
                return urlBuilder.ToString();
            }
        }
    }
}
