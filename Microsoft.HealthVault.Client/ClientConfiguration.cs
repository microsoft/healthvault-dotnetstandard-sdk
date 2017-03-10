using System;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Client
{
    public class ClientConfiguration : ConfigurationBase
    {
        private bool allowInstanceBounce = true;

        public bool AllowInstanceBounce
        {
            get
            {
                return this.allowInstanceBounce;
            }

            set
            {
                this.EnsureNotLocked();
                this.allowInstanceBounce = value;
            }
        }
    }
}
