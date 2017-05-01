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
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.PlatformInformation;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal class HealthVaultAuthenticationHandler : RemoteAuthenticationHandler<HealthVaultAuthenticationOptions>
    {
        private Task<AuthenticateResult> readCookieTask;
        private readonly string Purpose = nameof(HealthVaultAuthenticationMiddleware);
        private const string CookiePrefix = ".";

        public string CookieName => CookiePrefix + this.Options.AuthenticationScheme;

        protected override Task HandleSignInAsync(SignInContext context)
        {
            if (context.Principal == null)
            {
                this.RedirectChallenge(context.Properties);
            }

            return Task.FromResult(true);
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            this.Options.CookieManager.DeleteCookie(
                this.Context,
                this.CookieName,
                this.BuildCookieOptions());

            this.Context.User = null;

            return Task.FromResult(true);
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var properties = context.Properties;
            this.RedirectChallenge(properties);

            return Task.FromResult(true);
        }

        private void RedirectChallenge(IDictionary<string, string> properties)
        {
            var options = this.Options;

            if (!properties.ContainsKey("RedirectUri"))
            {
                properties["RedirectUri"] = this.CurrentUri;
            }

            //TODO: Does HV signin support State parameter for xsrf?
            //var authProps = new AuthenticationProperties(properties);
            //GenerateCorrelationId(authProps);

            var urlBuilder = new ShellUrlBuilder(this.Context, HealthVaultConstants.ShellRedirectTargets.Auth,
                new Dictionary<string, object>
                {
                    {"appid", options.Configuration.MasterApplicationId.ToString()},
                    {"redirect", this.BuildRedirectUri(options.CallbackPath)},
                    {"ismra", options.Configuration.IsMultiRecordApp}
                });

            this.Response.Redirect(urlBuilder.ToString());
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            this.InitializeStateFormatter();

            var result = await this.EnsureCookieTicket();
            if (!result.Succeeded)
            {
                return result;
            }

            if (result.Ticket.Principal == null)
            {
                return AuthenticateResult.Fail("No principal.");
            }

            return AuthenticateResult.Success(result.Ticket);
        }
        
        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            var query = this.Request.Query;

            string authToken = query[ShellTargetQsReturnParameters.WcToken];
            string instanceId = query[ShellTargetQsReturnParameters.InstanceId];
            string action = query[ShellTargetQsReturnParameters.ActionQs];

            if (authToken == null)
            {
                return AuthenticateResult.Fail("The authe token was missing or invalid.");
            }

            var properties = new AuthenticationProperties();

            //Checks xsrf token, check for HV support of a state variable
            //this.ValidateCorrelationId(properties);

            var identity = new ClaimsIdentity(this.Options.ClaimsIssuer);

            var webConnectionInfo = await this.CreateWebConnectionInfoAsync(authToken, instanceId);
            identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, webConnectionInfo.PersonInfo.Name));
            identity.AddClaim(new Claim(KnownClaims.WebConnectionInfo, JsonConvert.SerializeObject(webConnectionInfo)));

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, HealthVaultAuthenticationMiddleware.AuthenticationScheme);
            ticket.Properties.IssuedUtc = this.Options.SystemClock.UtcNow;
            ticket.Properties.RedirectUri = action;

            var cookieOptions = this.BuildCookieOptions();

            this.InitializeStateFormatter();
            var cookieValue = this.Options.StateDataFormat.Protect(ticket, this.Purpose);

            this.Options.CookieManager.AppendResponseCookie(
                this.Context,
                this.CookieName,
                cookieValue,
                cookieOptions);

            return AuthenticateResult.Success(ticket);
        }

        private async Task<WebConnectionInfo> CreateWebConnectionInfoAsync(string token, string instanceId)
        {
            IServiceLocator serviceLocator = Ioc.Get<IServiceLocator>();
            IServiceInstanceProvider serviceInstanceProvider = serviceLocator.GetInstance<IServiceInstanceProvider>();
            HealthServiceInstance serviceInstance = await serviceInstanceProvider.GetHealthServiceInstanceAsync(instanceId);

            IWebHealthVaultConnection webHealthVaultConnection = new WebHealthVaultConnection(
                 (ClaimsIdentity)this.Context.User.Identity,
                serviceInstance,
                null,
                token);

            IPersonClient personClient = webHealthVaultConnection.CreatePersonClient();

            var personInfo = await personClient.GetPersonInfoAsync();

            WebConnectionInfo webConnectionInfo = new WebConnectionInfo
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
            if (this.readCookieTask == null)
            {
                this.readCookieTask = this.ReadCookieTicket();
            }
            return this.readCookieTask;
        }

        private async Task<AuthenticateResult> ReadCookieTicket()
        {
            var cookie = this.Options.CookieManager.GetRequestCookie(this.Context, this.CookieName);
            if (string.IsNullOrEmpty(cookie))
            {
                return AuthenticateResult.Skip();
            }

            var ticket = this.Options.StateDataFormat.Unprotect(cookie, this.Purpose);
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Unprotect ticket failed");
            }

            var currentUtc = DateTime.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                return AuthenticateResult.Fail("Ticket expired");
            }

            return AuthenticateResult.Success(ticket);
        }

        private CookieOptions BuildCookieOptions()
        {
            var cookieOptions = new CookieOptions
            {
                Domain = this.Options.CookieDomain,
                HttpOnly = this.Options.CookieHttpOnly,
                Path = this.Options.CookiePath ?? (this.OriginalPathBase.HasValue ? this.OriginalPathBase.ToString() : "/"),
            };

            if (this.Options.CookieSecure == CookieSecurePolicy.SameAsRequest)
            {
                cookieOptions.Secure = this.Request.IsHttps;
            }
            else
            {
                cookieOptions.Secure = this.Options.CookieSecure == CookieSecurePolicy.Always;
            }

            return cookieOptions;
        }

        private void InitializeStateFormatter()
        {
            if (this.Options.StateDataFormat == null)
            {
                var provider = this.Context.RequestServices.GetRequiredService<IDataProtectionProvider>();
                var dataProtector = provider.CreateProtector(this.Purpose, this.Options.AuthenticationScheme, "v1");
                this.Options.StateDataFormat = new TicketDataFormat(dataProtector);
            }
        }
    }
}