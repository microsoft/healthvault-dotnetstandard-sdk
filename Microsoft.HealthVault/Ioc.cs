using Grace.DependencyInjection;

namespace Microsoft.HealthVault
{
    internal static class Ioc
    {
        static Ioc()
        {
            Container = new DependencyInjectionContainer();

            RegisterTypes(Container);
        }

        public static DependencyInjectionContainer Container { get; }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
        }

        public static T Get<T>()
        {
            return Container.Locate<T>();
        }
    }
}
