using Grace.DependencyInjection;
using Microsoft.HealthVault.Soda.Platform;

namespace Microsoft.HealthVault.Soda
{
    internal static class SodaIoc
    {
        static SodaIoc()
        {
            Container = new DependencyInjectionContainer();

            CoreIoc.RegisterTypes(Container);
            PlatformSodaIoc.RegisterTypes(Container);
        }

        public static DependencyInjectionContainer Container { get; }
    }
}
