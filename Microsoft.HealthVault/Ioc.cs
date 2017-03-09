using Grace.DependencyInjection;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using System;

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
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
            container.RegisterTransient<ICryptoConfiguration, BaseCryptoConfiguration>();
            container.RegisterTransient<IConnectionInternal, ConnectionInternalBase>();
            container.RegisterTransient<ISessionCredentialClient, SessionCredentialClientBase>();
            container.RegisterSingleton<ICryptoService, CryptoService>();
            container.RegisterSingleton<IServiceLocator, ServiceLocator>();
        }

        public static void OverrideClientType<T>(Func<T, T> func)
            where T : IClient
        {
            // TODO: Throw an exception if a connection has already been created.
            Container.Configure(c => c.ExportFactory(() => func(clientContainer.Locate<T>())));
        }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
