// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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