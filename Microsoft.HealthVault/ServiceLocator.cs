using System;
using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault
{
    internal class ServiceLocator : IServiceLocator
    {
        public T GetInstance<T>()
        {
            return Ioc.Get<T>();
        }
    }
}
