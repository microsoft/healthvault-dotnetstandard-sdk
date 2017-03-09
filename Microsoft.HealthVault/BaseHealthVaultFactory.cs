using System;
using Microsoft.HealthVault.Clients;

namespace Microsoft.HealthVault
{
    public abstract class BaseHealthVaultFactory : IHealthVaultFactoryBase
    {
        public void RegisterClientType<T>(Func<T, T> func)
            where T : IClient
        {
            Ioc.OverrideClientType(func);
        }
    }
}
