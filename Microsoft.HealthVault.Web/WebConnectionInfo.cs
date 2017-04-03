using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;

namespace Microsoft.HealthVault.Web
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
    }
}