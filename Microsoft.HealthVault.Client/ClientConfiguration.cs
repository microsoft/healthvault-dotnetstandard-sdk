using System;
using Microsoft.HealthVault.Configurations;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Client
{
    public class ClientConfiguration
    {
        private static IConfiguration configuration = Ioc.Get<IConfiguration>();
        private bool isLocked = false;
        private bool allowInstanceBounce = true;

        public bool AllowInstanceBounce
        {
            get
            {
                return allowInstanceBounce;
            }
            set
            {
                if (!isLocked)
                {
                    allowInstanceBounce = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }

        internal void LockConfiguration()
        {
            this.isLocked = true;
        }
    }
}
