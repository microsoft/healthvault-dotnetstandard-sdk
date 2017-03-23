using Grace.DependencyInjection;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using System;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault
{
    internal static class Ioc
    {
        private static DependencyInjectionContainer clientContainer;

        static Ioc()
        {
            clientContainer = new DependencyInjectionContainer();
            Container = new DependencyInjectionContainer();

            // register the clients in the main container and the client container
            RegisterClients(clientContainer);
            RegisterClients(Container);

            // register all other types in the main container only
            RegisterTypes(Container);
        }

        public static DependencyInjectionContainer Container { get; private set; }

        private static void RegisterClients(DependencyInjectionContainer container)
        {
            container.RegisterTransient<IPlatformClient, PlatformClient>();
            container.RegisterTransient<IPersonClient, PersonClient>();
            container.RegisterTransient<IVocabularyClient, VocabularyClient>();
            container.RegisterTransient<IThingClient, ThingClient>();
            container.RegisterTransient<IHealthWebRequestFactory, HealthWebRequestFactory>();
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
            container.RegisterTransient<IConnectionInternal, HealthVaultConnectionBase>();
            container.RegisterTransient<ISessionCredentialClient, SessionCredentialClientBase>();
            container.RegisterSingleton<IServiceLocator, ServiceLocator>();
            container.RegisterSingleton<ICryptographer, Cryptographer>();
        }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
