using System;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore.Internal
{
    internal static class WebConnectionInfoExtensions
    {
        public static WebConnectionInfo GetConnectionInfo(this ClaimsIdentity identity)
        {
            string str;
            if ((str = identity?.FindFirst(KnownClaims.WebConnectionInfo)?.Value) == null)
            {
                throw new NotSupportedException("WebConnectionInfo is expected for authenticated connections");
            }

            return JsonConvert.DeserializeObject<WebConnectionInfo>(str);
        }
    }
}