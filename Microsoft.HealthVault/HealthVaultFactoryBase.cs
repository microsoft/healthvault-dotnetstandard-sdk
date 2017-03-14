using Microsoft.HealthVault.Clients;
using System;

namespace Microsoft.HealthVault
{
    public abstract class HealthVaultFactoryBase : IHealthVaultFactoryBase
    {
        private volatile bool getConnectionCalled;

        protected bool GetConnectionCalled
        {
            get { return this.getConnectionCalled; }
            set { this.getConnectionCalled = value; }
        }

        public void RegisterClientType<T>(Func<T, T> func)
            where T : IClient
        {
            Ioc.OverrideClientType(func);
        }

        protected void ThrowIfGetConnectionCalled()
        {
            if (this.GetConnectionCalled)
            {
                throw new InvalidOperationException("Cannot set configuration after calling GetConnectionAsync.");
            }
        }
    }
}
