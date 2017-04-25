using Grace.DependencyInjection;
using Microsoft.HealthVault.Clients.Deserializers;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Services;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault
{

    internal static class Ioc
    {
        static Ioc()
        {
            Container = new DependencyInjectionContainer();

            Container.RegisterTransient<IConnectionInternal, HealthVaultConnectionBase>();
            Container.RegisterTransient<ISessionCredentialClient, SessionCredentialClientBase>();

            Container.RegisterSingleton<IServiceLocator, ServiceLocator>();
            Container.RegisterSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            Container.RegisterSingleton<IHttpClientFactory, HttpClientFactory>();
            Container.RegisterSingleton<IDateTimeService, DateTimeService>();

            Container.RegisterSingleton<IRequestMessageCreator, RequestMessageCreator>();
            Container.RegisterSingleton<IHealthServiceResponseParser, HealthServiceResponseParser>();
            Container.RegisterSingleton<IThingDeserializer, ThingDeserializer>();

            Container.RegisterSingleton<IHealthWebRequestClient, HealthWebRequestClient>();
            Container.RegisterSingleton<ICryptographer, Cryptographer>();
        }

        public static DependencyInjectionContainer Container { get; internal set; }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
