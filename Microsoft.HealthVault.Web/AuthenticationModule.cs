// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Providers;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Provides Authentication by plugging into the MVC request pipeline
    /// </summary>
    /// <seealso cref="IHttpModule" />
    public class AuthenticationModule : IHttpModule
    {
        /// <summary>
        /// Initializes the module at application start.
        /// </summary>
        /// <param name="context">The application context</param>
        public void Init(HttpApplication context)
        {
            var wrapper = new EventHandlerTaskAsyncHelper(AuthenticateRequest);
            context.AddOnAuthenticateRequestAsync(wrapper.BeginEventHandler, wrapper.EndEventHandler);

            WebIoc.EnsureTypesRegistered();
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
        }

        private static async Task AuthenticateRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = new HttpContextWrapper(app.Context);

            var processAuthToken = ProcessAuthToken(context);
            bool hasAuthTokenInQuery = string.IsNullOrEmpty(processAuthToken.Item1);

            ICookieManager cookieManager = Ioc.Container.Locate<ICookieManager>();
            WebConnectionInfo webConnectionInfo;

            if (hasAuthTokenInQuery)
            {
                webConnectionInfo = await CreateWebConnectionInfoAsync(processAuthToken.Item1, processAuthToken.Item2);
                cookieManager.Save(context, webConnectionInfo);
            }
            else
            {
                webConnectionInfo = cookieManager.Load(context);
            }

            if (webConnectionInfo == null)
            {
                cookieManager.Clear(context);
                context.User = new GenericPrincipal(new HealthVaultIdentity(), null);
            }
            else
            {
                context.User = new GenericPrincipal(
                    new HealthVaultIdentity { WebConnectionInfo = webConnectionInfo },
                    null);
            }

            if (hasAuthTokenInQuery)
            {
                Redirect(context);
            }
        }

        private static Tuple<string, string> ProcessAuthToken(HttpContextBase context)
        {
            string authToken = context.Request.Params["wctoken"];
            string instanceId = context.Request.Params["instanceid"];

            return new Tuple<string, string>(authToken, instanceId);
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

        private static async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            WebHealthVaultConfiguration webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();

            IWebHealthVaultConnection webHealthVaultConnection = Ioc.Container.Locate<IWebHealthVaultConnection>(
                new { serviceLocator, webHealthVaultConfiguration, serviceInstance, token });

            IPersonClient personClient = ClientHealthVaultFactory.GetPersonClient(webHealthVaultConnection);

            PersonInfo personInfo = (await personClient.GetAuthorizedPeopleAsync()).FirstOrDefault();

            WebConnectionInfo webConnectionInfo = new WebConnectionInfo()
            {
                PersonInfo = personInfo,
                ServiceInstanceId = instanceId,
                SessionCredential = webHealthVaultConnection.SessionCredential,
            };

            return webConnectionInfo;
        }
    }
}
