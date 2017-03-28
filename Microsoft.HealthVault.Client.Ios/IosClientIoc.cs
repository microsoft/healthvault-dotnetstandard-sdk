using System.Diagnostics;
using System.Reflection;
using Grace.DependencyInjection;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Extensions;
using UIKit;

namespace Microsoft.HealthVault.Client
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
            IosBrowserAuthBroker authBroker = container.Locate<IosBrowserAuthBroker>();
            container.Configure(c => c.ExportInstance(authBroker).As<IosBrowserAuthBroker>());
            container.Configure(c => c.ExportInstance(authBroker).As<IBrowserAuthBroker>());

            container.RegisterSingleton<ISecretStore, IosSecretStore>();
            container.RegisterSingleton<IMessageHandlerFactory, IosMessageHandlerFactory>();

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
                Category = HealthVaultConstants.SdkTelemetryInformationCategories.IosClient,
                FileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion,
                OsInformation = $"iOS {UIDevice.CurrentDevice.SystemVersion}"
            };

            container.Configure(c => c.ExportInstance(sdkTelemetryInformation).As<SdkTelemetryInformation>());
        }
    }
}
