using Grace.DependencyInjection;
using Microsoft.HealthVault.Client.Platform;

namespace Microsoft.HealthVault.Client
{
    internal static class ClientIoc
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

                    PlatformClientIoc.RegisterTypes(Ioc.Container);
                    typesRegistered = true;
                }
            }
        }

        private static void RegisterTypes(DependencyInjectionContainer container)
        {
        }
    }
}
