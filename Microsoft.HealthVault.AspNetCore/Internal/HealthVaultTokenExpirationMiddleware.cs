using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal sealed class HealthVaultTokenExpirationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public HealthVaultTokenExpirationMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.logger = loggerFactory.CreateLogger<HealthVaultTokenExpirationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await this.next(context);
            }
            catch (HealthServiceCredentialTokenExpiredException)
            {
                try
                {
                    await context.Authentication.SignOutAsync(HealthVaultAuthenticationMiddleware.AuthenticationScheme);
                    await context.Authentication.ChallengeAsync(HealthVaultAuthenticationMiddleware.AuthenticationScheme);
                    return;
                }
                catch (Exception ex2)
                {
                    LoggerExtensions.LogError(this.logger, 0, ex2,
                        "An exception was thrown attempting " +
                        "to execute the error handler.");
                }

                throw;
            }
        }
    }
}