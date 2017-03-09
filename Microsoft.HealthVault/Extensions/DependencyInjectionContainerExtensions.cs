using Grace.DependencyInjection;

namespace Microsoft.HealthVault.Extensions
{
    internal static class DependencyInjectionContainerExtensions
    {
        public static void RegisterSingleton<TInterface, TType>(this DependencyInjectionContainer container)
        {
            container.Configure(c => c.Export<TType>().As<TInterface>().Lifestyle.Singleton());
        }

        public static void RegisterTransient<TInterface, TType>(this DependencyInjectionContainer container)
        {
            container.Configure(c => c.Export<TType>().As<TInterface>());
        }
    }
}
