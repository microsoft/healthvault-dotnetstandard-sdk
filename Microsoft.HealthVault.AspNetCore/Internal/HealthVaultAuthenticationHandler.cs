using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.HealthVault.AspNetCore.Connection;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal class HealthVaultAuthenticationHandler : RemoteAuthenticationHandler<HealthVaultAuthenticationOptions>
    {
        private const string CookiePrefix = ".";
        private readonly string Purpose = nameof(HealthVaultAuthenticationMiddleware);
        private Task<AuthenticateResult> _readCookieTask;

        public string CookieName => CookiePrefix + Options.AuthenticationScheme;

        protected override Task HandleSignInAsync(SignInContext context)
        {
            if (context.Principal == null)
                RedirectChallenge(context.Properties);

            return Task.FromResult(true);
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            Options.CookieManager.DeleteCookie(
                Context,
                CookieName,
                BuildCookieOptions());

            Context.User = null;

            return Task.FromResult(true);
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            var properties = context.Properties;
            RedirectChallenge(properties);

            return Task.FromResult(true);
        }

        private void RedirectChallenge(IDictionary<string, string> properties)
        {
            var options = Options;

            if (!properties.ContainsKey("RedirectUri"))
                properties["RedirectUri"] = CurrentUri;

            //TODO: Does HV signin support State parameter for xsrf?
            //var authProps = new AuthenticationProperties(properties);
            //GenerateCorrelationId(authProps);

            var urlBuilder = new ShellUrlBuilder(Context, HealthVaultConstants.ShellRedirectTargets.Auth,
                new Dictionary<string, object>
                {
                    {"appid", options.Configuration.MasterApplicationId.ToString()},
                    {"redirect", BuildRedirectUri(options.CallbackPath)},
                    {"ismra", options.Configuration.IsMultiRecordApp}
                });

            Response.Redirect(urlBuilder.ToString());
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            InitializeStateFormatter();

            var result = await EnsureCookieTicket();
            if (!result.Succeeded)
                return result;

            if (result.Ticket.Principal == null)
                return AuthenticateResult.Fail("No principal.");

            return AuthenticateResult.Success(result.Ticket);
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            var query = Request.Query;

            string authToken = query[ShellTargetQsReturnParameters.WcToken];
            string instanceId = query[ShellTargetQsReturnParameters.InstanceId];
            string action = query[ShellTargetQsReturnParameters.ActionQs];

            if (authToken == null)
                return AuthenticateResult.Fail("The authe token was missing or invalid.");

            var properties = new AuthenticationProperties();

            //Checks xsrf token, check for HV support of a state variable
            //this.ValidateCorrelationId(properties);

            var identity = new ClaimsIdentity(Options.ClaimsIssuer);

            var webConnectionInfo = await CreateWebConnectionInfoAsync(authToken, instanceId);
            identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, webConnectionInfo.PersonInfo.Name));
            identity.AddClaim(new Claim(KnownClaims.WebConnectionInfo, JsonConvert.SerializeObject(webConnectionInfo)));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, HealthVaultAuthenticationDefaults.AuthenticationScheme);
            ticket.Properties.IssuedUtc = Options.SystemClock.UtcNow;
            ticket.Properties.RedirectUri = action;

            var cookieOptions = BuildCookieOptions();

            InitializeStateFormatter();
            string cookieValue = Options.StateDataFormat.Protect(ticket, Purpose);

            Options.CookieManager.AppendResponseCookie(
                Context,
                CookieName,
                cookieValue,
                cookieOptions);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            var serviceLocator = Ioc.Get<IServiceLocator>();
            var serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            var serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            IWebHealthVaultConnection webHealthVaultConnection = new WebHealthVaultConnection(
                (ClaimsIdentity) Context.User.Identity,
                serviceInstance,
                null,
                token);

            var personClient = webHealthVaultConnection.CreatePersonClient();

            var personInfo = await personClient.GetPersonInfoAsync();

            var webConnectionInfo = new WebConnectionInfo
            {
                PersonInfo = personInfo,
                ServiceInstanceId = instanceId,
                SessionCredential = webHealthVaultConnection.SessionCredential,
                UserAuthToken = token
            };

            return webConnectionInfo;
        }

        private Task<AuthenticateResult> EnsureCookieTicket()
        {
            // We only need to read the ticket once
            if (_readCookieTask == null)
                _readCookieTask = ReadCookieTicket();
            return _readCookieTask;
        }

        private Task<AuthenticateResult> ReadCookieTicket()
        {
            string cookie = Options.CookieManager.GetRequestCookie(Context, CookieName);
            if (string.IsNullOrEmpty(cookie))
                return Task.FromResult(AuthenticateResult.Skip());

            var ticket = Options.StateDataFormat.Unprotect(cookie, Purpose);
            if (ticket == null)
                return Task.FromResult(AuthenticateResult.Fail("Unprotect ticket failed"));

            var currentUtc = DateTime.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
                return Task.FromResult(AuthenticateResult.Fail("Ticket expired"));

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        private CookieOptions BuildCookieOptions()
        {
            var cookieOptions = new CookieOptions
            {
                Domain = Options.CookieDomain,
                HttpOnly = Options.CookieHttpOnly,
                Path = Options.CookiePath ?? (OriginalPathBase.HasValue ? OriginalPathBase.ToString() : "/")
            };

            if (Options.CookieSecure == CookieSecurePolicy.SameAsRequest)
                cookieOptions.Secure = Request.IsHttps;
            else
                cookieOptions.Secure = Options.CookieSecure == CookieSecurePolicy.Always;

            return cookieOptions;
        }

        private void InitializeStateFormatter()
        {
            if (Options.StateDataFormat == null)
            {
                var provider = Context.RequestServices.GetRequiredService<IDataProtectionProvider>();
                var dataProtector = provider.CreateProtector(Purpose, Options.AuthenticationScheme, "v1");
                Options.StateDataFormat = new TicketDataFormat(dataProtector);
            }
        }
    }
}