using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.HealthVault.AspNetCore.Internal;

namespace Microsoft.HealthVault.AspNetCore
{
    public sealed class HealthVaultAuthenticationMiddleware : AuthenticationMiddleware<HealthVaultAuthenticationOptions>
    {
        public const string AuthenticationScheme = "healthvault";

        public HealthVaultAuthenticationMiddleware(
            RequestDelegate next, 
            IOptions<HealthVaultAuthenticationOptions> options, 
            ILoggerFactory loggerFactory, 
            UrlEncoder encoder)
            : base(next, options, loggerFactory, encoder)
        {
        }

        protected override AuthenticationHandler<HealthVaultAuthenticationOptions> CreateHandler()
        {
            return new HealthVaultAuthenticationHandler();
        }
    }
}