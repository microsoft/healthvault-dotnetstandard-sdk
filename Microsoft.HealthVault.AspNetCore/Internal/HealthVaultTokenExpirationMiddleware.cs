using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal sealed class HealthVaultTokenExpirationMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public HealthVaultTokenExpirationMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<HealthVaultTokenExpirationMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HealthServiceCredentialTokenExpiredException)
            {
                try
                {
                    await context.Authentication.SignOutAsync(HealthVaultAuthenticationDefaults.AuthenticationScheme);
                    await context.Authentication.ChallengeAsync(HealthVaultAuthenticationDefaults.AuthenticationScheme);
                    return;
                }
                catch (Exception ex2)
                {
                    _logger.LogError(0, ex2,
                        "An exception was thrown attempting " +
                        "to execute the error handler.");
                }

                throw;
            }
        }
    }
}