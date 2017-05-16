using System;
using System.Reflection;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Client;
using Microsoft.HealthVault.Extensions;

namespace Microsoft.HealthVault.IntegrationTest
{
    internal static class ClientIoc
    {
        internal static void EnsureTypesRegistered()
        {
            // CoreClientIoc will call RegisterPlatformTypes.  Do all our registering there.
            CoreClientIoc.EnsureTypesRegistered(RegisterPlatformTypes);
        }

        private static void RegisterPlatformTypes(DependencyInjectionContainer container)
        {
            container.RegisterSingleton<IBrowserAuthBroker, StubBrowserAuthBroker>();
            container.RegisterSingleton<ISecretStore, TestSecretStore>();

            RegisterTelemetryInformation(container);
        }

        /// <summary>
        /// Register Telemetry information for SDK
        /// </summary>
        /// <param name="container">IOC container</param>
        private static void RegisterTelemetryInformation(DependencyInjectionContainer container)
        {
            var sdkTelemetryInformation = new SdkTelemetryInformation
            {
                Category = HealthVaultConstants.SdkTelemetryInformationCategories.IntegrationTest,
                FileVersion = typeof(ClientIoc).GetTypeInfo().Assembly.GetName().Version.ToString(),
                OsInformation = $"Test platform 1.0"
            };

            container.Configure(c => c.ExportInstance(sdkTelemetryInformation).As<SdkTelemetryInformation>());
        }
    }
}
