using System;
using Microsoft.HealthVault.Configurations;

namespace Microsoft.HealthVault.Client
{
    public class ClientConfiguration
    {
        private static Lazy<IConfiguration> configuration = Ioc.Get<Lazy<IConfiguration>>();
        private bool isLocked = false;

        public bool AllowInstanceBounce { get; set; } = true;

        internal void LockConfiguration()
        {
            this.isLocked = true;
        }
    }
}
