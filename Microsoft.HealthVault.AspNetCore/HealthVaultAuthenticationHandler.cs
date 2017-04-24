using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.HealthVault.AspNetCore.Connection;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.AspNetCore.Providers;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.PlatformInformation;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore
{
    internal static class ShellTargetQsReturnParameters
    {
        internal const string WcToken = "wctoken";
        internal const string InstanceId = "instanceid";
        internal const string SuggestedTokenTtl = "suggestedtokenttl";
    }

    internal class HealthVaultAuthenticationHandler : RemoteAuthenticationHandler<HealthVaultAuthenticationOptions>
    {
        protected override Task HandleSignInAsync(SignInContext context)
        {
            //https://github.com/aspnet/Security/blob/99aa3bd35dd5fbe46a93eef8a2c8ab1f9fe8d05b/src/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs
            throw new NotImplementedException();
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            var options = Ioc.Get<HealthVaultAuthenticationOptions>();

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var properties = context.Properties;
            if (!properties.ContainsKey("RedirectUri"))
            {
                properties["RedirectUri"] = CurrentUri;
            }

            var authProps = new AuthenticationProperties(properties);

            GenerateCorrelationId(authProps);

            var urlBuilder = new ShellUrlBuilder(Context, HealthVaultConstants.ShellRedirectTargets.Auth, 
                new Dictionary<string, object>
                {
                    {"appid", options.Configuration.MasterApplicationId.ToString() },
                    {"redirect", BuildRedirectUri(options.CallbackPath) },
                    {"ismra", options.Configuration.IsMultiRecordApp}
                });

            Response.Redirect(urlBuilder.ToString());

            return Task.FromResult(true);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            return base.HandleAuthenticateAsync();
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            AuthenticationProperties properties = null;
            var query = Request.Query;

            string authToken = query[ShellTargetQsReturnParameters.WcToken];
            string instanceId = query[ShellTargetQsReturnParameters.InstanceId];


            if (authToken == null)
            {
                return AuthenticateResult.Fail("The authe token was missing or invalid.");
            }

            var identity = new ClaimsIdentity(Options.ClaimsIssuer);

            var webConnectionInfo = await CreateWebConnectionInfoAsync(authToken, instanceId);
            identity.AddClaim(new Claim(KnownClaims.WebConnectionInfo, JsonConvert.SerializeObject(webConnectionInfo)));

            var ticket = await CreateTicketAsync(identity, properties, null);
            if (ticket != null)
            {
                return AuthenticateResult.Success(ticket);
            }
            else
            {
                return AuthenticateResult.Fail("Failed to retrieve user information from remote server.");
            }
        }

        private static async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            HealthVaultConfiguration webHealthVaultConfiguration = Ioc.Get<HealthVaultConfiguration>();

            IWebHealthVaultConnection webHealthVaultConnection = new WebHealthVaultConnection(
                serviceLocator, 
                serviceLocator.GetInstance<IHttpContextAccessor>(), 
                serviceInstance,
                null, 
                token);

            var serviceInstanceHealthServiceUrl = serviceInstance.HealthServiceUrl;

            // Set socket to be refreshed in case the end point has been changed based on the healthvault service instance
            if (!webHealthVaultConfiguration.HealthVaultUrl.Equals(serviceInstanceHealthServiceUrl))
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
            //var sp = ServicePointManager.FindServicePoint(healthVaultUrl);
            //sp.ConnectionLeaseTimeout = 60 * 1000; // 1 minute
        }

        private async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, object tokens)
        {
            return new AuthenticationTicket(new ClaimsPrincipal(identity), properties, HealthVaultAuthenticationMiddleware.AuthenticationScheme);
        }
    }
}