using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Web
{
    internal static class WebIoc
    {
        private static readonly object RegistrationLock = new object();

        private static bool typesRegistered;

        public static void EnsureTypesRegistered()
        {
            lock (RegistrationLock)
            {
                if (!typesRegistered)
                {
                    RegisterTypes(Ioc.Container);
                    typesRegistered = true;
                }
            }
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
            container.RegisterTransient<ISessionCredentialClient, WebSessionCredentialClient>();
            container.RegisterTransient<ICryptographer, WebCryptographer>();
            container.RegisterTransient<IConnectionInternal, WebConnection>();
        }
    }
}
