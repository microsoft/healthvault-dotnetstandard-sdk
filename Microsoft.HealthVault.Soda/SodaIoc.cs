using Grace.DependencyInjection;
using Microsoft.HealthVault.Soda.Platform;

namespace Microsoft.HealthVault.Soda
{
    internal static class SodaIoc
    {
        private static readonly object RegistrationLock = new object();

        private static bool typesRegistered;

        public static void EnsureTypesRegistered()
        {
            lock (RegistrationLock)
            {
                if (!typesRegistered)
                {
                    // Register SODA types
                    RegisterTypes(Ioc.Container);

                    PlatformSodaIoc.RegisterTypes(Ioc.Container);
                    typesRegistered = true;
                }
            }
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
        }
    }
}
