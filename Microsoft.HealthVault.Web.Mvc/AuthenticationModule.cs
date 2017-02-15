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
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// Handles authentication of HealthVault users and the loading of the <see cref="PersonInfo"/>
    /// object before the request is passed to the application.
    /// </summary>
    public class AuthenticationModule : IHttpModule
    {
        /// <summary>
        /// Initializes the module at application start.
        /// </summary>
        /// <param name="context">The application context</param>
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest +=
                (s, e) => AuthenticateRequest((HttpApplication)s);
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
        }

        private static void AuthenticateRequest(HttpApplication app)
        {
            var context = new HttpContextWrapper(app.Context);
            var manager = new PersonInfoManager(context);
            if (!ProcessAuthToken(context, manager))
            {
                // Try the cookie
                var personInfo = manager.Load();
                if (personInfo == null)
                {
                    manager.Clear();
                }
            }
        }

        private static bool ProcessAuthToken(HttpContextBase context, PersonInfoManager manager)
        {
            string authToken = context.Request.Params["wctoken"];
            string instanceId = context.Request.Params["instanceid"];
            if (string.IsNullOrEmpty(authToken))
            {
                return false;
            }

            // Use the authToken issued by HealthVault to retrieve a PersonInfo object
            PersonInfo personInfo = RequestPersonInfo(authToken, instanceId);
            manager.Save(personInfo);

            // Then redirect to the original Url. The user's identity etc will be written to a cookie.
            // All subsequent authentication will use the cookie
            Redirect(context);
            return true;
        }

        // Redirect to the original Url, minus tokens from the query string
        // The user's identity etc will be written to a cookie.
        // All subsequent authentication will use the cookie
        [SuppressMessage(
            "Microsoft.Security.Web",
            "CA3007:ReviewCodeForOpenRedirectVulnerabilities",
            Justification = "Redirecting to self, no user input")]
        private static void Redirect(HttpContextBase context)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(context.Request.Url.Query);
            query.Remove("wctoken");
            query.Remove("suggestedtokenttl");

            UriBuilder newUrl = new UriBuilder(context.Request.Url) { Query = query.ToString() };

            context.Response.Redirect(newUrl.Uri.OriginalString);
        }

        private static PersonInfo RequestPersonInfo(string token, string instanceId)
        {
            var serviceInstance = ServiceInfo.Current.ServiceInstances[instanceId];

            return WebApplicationUtilities.GetPersonInfo(token, HealthVault.Config.ApplicationId, serviceInstance);
        }
    }
}