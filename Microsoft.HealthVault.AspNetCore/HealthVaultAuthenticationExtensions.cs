using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.Configuration;

namespace Microsoft.HealthVault.AspNetCore
{
    public static class HealthVaultAuthenticationExtensions
    {
        public static IApplicationBuilder UseHealthVault(this IApplicationBuilder app, HealthVaultAuthenticationOptions options)
        {
            WebIoc.EnsureTypesRegistered(options);
            return app
                .UseMiddleware<HealthVaultAuthenticationMiddleware>(Options.Create(options))
                .UseMiddleware<HealthVaultTokenExpirationMiddleware>();
        }

        public static IApplicationBuilder UseHealthVault(this IApplicationBuilder app, HealthVaultConfiguration configuration)
        {
            return app.UseHealthVault(new HealthVaultAuthenticationOptions(configuration));
        }
    }
}
