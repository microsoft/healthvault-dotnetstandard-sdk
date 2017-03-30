using Grace.DependencyInjection;
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

            Container.RegisterTransient<IHealthWebRequestFactory, HealthWebRequestFactory>();
            Container.RegisterTransient<IConnectionInternal, HealthVaultConnectionBase>();
            Container.RegisterTransient<ISessionCredentialClient, SessionCredentialClientBase>();
            Container.RegisterSingleton<IServiceLocator, ServiceLocator>();
            Container.RegisterSingleton<ICryptographer, Cryptographer>();
            Container.RegisterSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            Container.RegisterSingleton<IHttpClientFactory, HttpClientFactory>();
            Container.RegisterSingleton<IDateTimeService, DateTimeService>();
        }

        public static DependencyInjectionContainer Container { get; }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
