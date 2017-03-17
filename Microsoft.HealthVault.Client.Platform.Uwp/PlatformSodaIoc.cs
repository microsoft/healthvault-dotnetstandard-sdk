using System;
using System.Reflection;
using Windows.Foundation.Metadata;
using Grace.DependencyInjection;

namespace Microsoft.HealthVault.Client.Platform
{
    internal static class PlatformSodaIoc
    {
        public static void RegisterTypes(DependencyInjectionContainer container)
        {
            RegisterTelemetryInformation(container);
        }

        /// <summary>
        /// Register Telemetry information for SDK
        /// </summary>
        /// <param name="container">IOC container</param>
        private static void RegisterTelemetryInformation(DependencyInjectionContainer container)
        {
            var hostOsVersionInfo = GetHostOSVersionInfo().ToString();

            var sdkTelemetryInformation = new SdkTelemetryInformation
            {
                Category = HealthVaultConstants.SdkTelemetryInformationCategories.WindowsClient,
                FileVersion = new AssemblyName(typeof(PlatformSodaIoc).AssemblyQualifiedName).Version.ToString(),
                OsInformation = $"Windows {hostOsVersionInfo}"
            };

            container.Configure(c => c.ExportInstance(sdkTelemetryInformation).As<SdkTelemetryInformation>());
        }

        /// <summary>
        /// Gets the OS version for Windows 10 through AnalyticsInfo.
        /// </summary>
        private static Version GetHostOSVersionInfo()
        {
            if (!ApiInformation.IsTypePresent("Windows.System.Profile.AnalyticsInfo") || !ApiInformation.IsTypePresent("Windows.System.Profile.AnalyticsVersionInfo"))
            {
                // Should not happen, but if we are running on an earlier version of Windows 10 without the API, return default
                return new Version(10, 0, 0, 0);
            }

            var familyVersion = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;

            long versionBytes;
            if (!long.TryParse(familyVersion, out versionBytes))
            {
                return new Version(10, 0, 0, 0);
            }

            Version uapVersion = new Version(
                (ushort)(versionBytes >> 48),
                (ushort)(versionBytes >> 32),
                (ushort)(versionBytes >> 16),
                (ushort)versionBytes);

            return uapVersion;
        }
    }
}
