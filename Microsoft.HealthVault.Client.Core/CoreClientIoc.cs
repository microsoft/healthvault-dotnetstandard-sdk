using System;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.Client
{
    internal static class CoreClientIoc
    {
        private static readonly object RegistrationLock = new object();

        private static bool typesRegistered;

        internal static void EnsureTypesRegistered(Action<DependencyInjectionContainer> registerClientTypesAction)
        {
            lock (RegistrationLock)
            {
                if (!typesRegistered)
                {
                    // Register SODA types
                    RegisterTypes(Ioc.Container);

                    registerClientTypesAction(Ioc.Container);
                    typesRegistered = true;
                }
            }
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
            container.RegisterSingleton<IHealthVaultConnectionFactory, HealthVaultConnectionFactoryBase>();
            container.RegisterSingleton<ILocalObjectStore, LocalObjectStore>();
            container.RegisterSingleton<IShellAuthService, ShellAuthService>();
            container.RegisterTransient<IClientSessionCredentialClient, ClientSessionCredentialClient>();
        }
    }
}
