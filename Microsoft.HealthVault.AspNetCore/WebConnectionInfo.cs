using System;
using System.Security.Claims;
using Microsoft.HealthVault.AspNetCore.Internal;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Newtonsoft.Json;

namespace Microsoft.HealthVault.AspNetCore
{
    /// <summary>
    /// Connection info stored in cookie used to authenticate web requests
    /// </summary>
    internal class WebConnectionInfo
    {
        public PersonInfo PersonInfo { get; set; }

        public string ServiceInstanceId { get; set; }

        public SessionCredential SessionCredential { get; set; }

        public string UserAuthToken { get; set; }

        public bool MinimizedPersonInfoRecords { get; set; }

        public bool MinimizedPersonInfoApplicationSettings { get; set; }
    }

    internal static class WebConnectionInfoExtensions
    {
        public static WebConnectionInfo GetConnectionInfo(this ClaimsIdentity identity)
        {
            var str = identity.FindFirst(KnownClaims.WebConnectionInfo)?.Value;

            if (str == null)
            {
                throw new NotSupportedException("WebConnectionInfo is expected for authenticated connections");
            }

            return JsonConvert.DeserializeObject<WebConnectionInfo>(str);
        }
    }
}