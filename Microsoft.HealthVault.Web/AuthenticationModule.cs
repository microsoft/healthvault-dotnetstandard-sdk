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
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.PlatformInformation;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Web.Configuration;
using Microsoft.HealthVault.Web.Connection;
using Microsoft.HealthVault.Web.Constants;
using Microsoft.HealthVault.Web.Cookie;
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

            // Set socket to be refreshed for default healthvault platform end point
            SetConnectionLeaseTimeOut(Ioc.Get<HealthVaultConfiguration>().DefaultHealthVaultUrl);
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
            bool hasAuthTokenInQuery = !string.IsNullOrEmpty(processAuthToken.Item1);

            ICookieManager cookieManager = Ioc.Container.Locate<ICookieManager>();
            WebConnectionInfo webConnectionInfo;

            // In case the app is not authentiated yet, then the user is directed to 
            // shell for authentication purposes. The redirected URL from shell will contain
            // wctoken and instanceId. Below we will check if the query contains those specific
            // query params.
            if (hasAuthTokenInQuery)
            {
                webConnectionInfo = await CreateWebConnectionInfoAsync(processAuthToken.Item1, processAuthToken.Item2);
                cookieManager.Save(context, webConnectionInfo);
                Redirect(context);

                return;
            }

            // In case the request doesn't contain the query params, try to load from the request context
            webConnectionInfo = cookieManager.TryLoad(context);

            // Set user context for cases where we were able to load the cookie
            if (webConnectionInfo != null)
            {
                context.User = new GenericPrincipal(
                    new HealthVaultIdentity { WebConnectionInfo = webConnectionInfo },
                    null);
            }
        }

        private static Tuple<string, string> ProcessAuthToken(HttpContextBase context)
        {
            var httpRequestBase = context.Request;

            string authToken = httpRequestBase.Params[HealthVaultWebConstants.ShellTargetQsReturnParameters.WcToken];
            string instanceId = httpRequestBase.Params[HealthVaultWebConstants.ShellTargetQsReturnParameters.InstanceId];

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

            query.Remove(HealthVaultWebConstants.ShellTargetQsReturnParameters.WcToken);
            query.Remove(HealthVaultWebConstants.ShellTargetQsReturnParameters.SuggestedTokenTtl);

            UriBuilder newUrl = new UriBuilder(context.Request.Url) { Query = query.ToString() };

            context.Response.Redirect(newUrl.Uri.OriginalString);
        }

        private static async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            IServiceLocator serviceLocator = new ServiceLocator();
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            WebHealthVaultConfiguration webHealthVaultConfiguration = Ioc.Get<WebHealthVaultConfiguration>();

            IWebHealthVaultConnection webHealthVaultConnection = new WebHealthVaultConnection(
                serviceLocator,
                serviceLocator.GetInstance<IHealthWebRequestClient>(),
                serviceLocator.GetInstance<HealthVaultConfiguration>(),
                serviceInstance, 
                null,
                token);

            var serviceInstanceHealthServiceUrl = serviceInstance.HealthServiceUrl;

            // Set socket to be refreshed in case the end point has been changed based on the healthvault service instance
            if (!webHealthVaultConfiguration.DefaultHealthVaultUrl.Equals(serviceInstanceHealthServiceUrl))
            {
                SetConnectionLeaseTimeOut(serviceInstanceHealthServiceUrl);
            }

            IPersonClient personClient = webHealthVaultConnection.CreatePersonClient();

            var personInfo = await personClient.GetPersonInfoAsync();

            WebConnectionInfo webConnectionInfo = new WebConnectionInfo()
            {
                PersonInfo = personInfo,
                ServiceInstanceId = instanceId,
                SessionCredential = webHealthVaultConnection.SessionCredential,
                UserAuthToken = token
            };

            return webConnectionInfo;
        }

        // We are using a singleton httpclient via <see cref="WebHttpClientFactory">, however, the tcp socket needs be
        // refreshed to honor DNS changes. More information here http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html.
        private static void SetConnectionLeaseTimeOut(Uri healthVaultUrl)
        {
            var sp = ServicePointManager.FindServicePoint(healthVaultUrl);
            sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
        }
    }
}
