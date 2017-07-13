// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.ItemTypes;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Web.Extensions;
using static Microsoft.HealthVault.Web.Constants.HealthVaultWebConstants;

namespace Microsoft.HealthVault.Web.Configuration
{
    /// <summary>
    /// A reader for the WebConfiguration
    /// </summary>
    internal class WebConfigurationReader
    {
        private static readonly Lazy<WebHealthVaultConfiguration> GeneratedConfig = new Lazy<WebHealthVaultConfiguration>(GenerateConfiguration);
        private static IThingTypeRegistrar s_thingTypeRegistrar;

        public static WebHealthVaultConfiguration GetConfiguration()
        {
            s_thingTypeRegistrar = Ioc.Get<ThingTypeRegistrar>();

            return GeneratedConfig.Value;
        }

        private static WebHealthVaultConfiguration GenerateConfiguration()
        {
            NameValueCollection appSettings = WebConfigurationManager.AppSettings;
            WebHealthVaultConfiguration config = new WebHealthVaultConfiguration
            {
                // Base HealthVaultConfiguration properties
                MasterApplicationId = appSettings.GetGuid(ConfigKeys.AppId),
                DefaultHealthVaultShellUrl = appSettings.GetUrl(ConfigKeys.ShellUrl, true),
                DefaultHealthVaultUrl = appSettings.GetUrl(ConfigKeys.HealthServiceUrl, true),

                RequestTimeoutDuration = appSettings.GetTimeSpanFromSeconds(
                    ConfigKeys.DefaultRequestTimeoutSeconds,
                    HealthVaultConfigurationDefaults.RequestTimeoutDuration) ??
                    HealthVaultConfigurationDefaults.RequestTimeoutDuration,

                RequestTimeToLiveDuration = appSettings.GetTimeSpanFromSeconds(
                    ConfigKeys.DefaultRequestTimeToLiveSeconds,
                    HealthVaultConfigurationDefaults.RequestTimeToLiveDuration) ??
                    HealthVaultConfigurationDefaults.RequestTimeToLiveDuration,

                InlineBlobHashBlockSize = appSettings.GetTypedValue(ConfigKeys.DefaultInlineBlobHashBlockSize, BlobHasher.DefaultInlineBlobHashBlockSizeBytes),
                IsMultiRecordApp = appSettings.GetTypedValue(ConfigKeys.IsMra, ConfigDefaults.IsMra),
                MultiInstanceAware = appSettings.GetTypedValue(ConfigKeys.MultiInstanceAware, true),
                RestHealthVaultUrl = appSettings.GetUrl(ConfigKeys.RestHealthServiceUrl, true),
                RetryOnInternal500Count = appSettings.GetTypedValue(ConfigKeys.RequestRetryOnInternal500Count, HealthVaultConfigurationDefaults.RetryOnInternal500Count),
                RetryOnInternal500SleepDuration = appSettings.GetTimeSpanFromSeconds(
                    ConfigKeys.RequestRetryOnInternal500SleepSeconds,
                    HealthVaultConfigurationDefaults.RetryOnInternal500SleepDuration) ??
                    HealthVaultConfigurationDefaults.RetryOnInternal500SleepDuration,
                SupportedTypeVersions = GetSupportedTypeVersions(appSettings[ConfigKeys.SupportedType]),
                UseLegacyTypeVersionSupport = appSettings.GetTypedValue(ConfigKeys.UseLegacyTypeVersionSupport, false),

                // WebHealthVaultConfiguration properties
                ActionPageUrls = GetActionUrls(appSettings),
                ActionUrlRedirectOverride = TryReadingActionUrlRedirectOverride(appSettings),
                AllowedRedirectSites = appSettings[ConfigKeys.AllowedRedirectSites],
                ApplicationCertificateFileName = appSettings[ConfigKeys.ApplicationCertificateFileName],
                ApplicationCertificatePassword = appSettings[ConfigKeys.ApplicationCertificatePassword],
                CertStore = (StoreLocation)appSettings.GetTypedValue(ConfigKeys.CertStore, (int)ConfigDefaults.CertStore),
                CertSubject = appSettings[ConfigKeys.CertSubject],
                CookieDomain = appSettings[ConfigKeys.CookieDomain] ?? ConfigDefaults.CookieDomain,
                CookieEncryptionKey = GetEncryptionKey(appSettings[ConfigKeys.CookieEncryptionKey]),
                CookieName = HttpContext.Current == null ? string.Empty : (HttpRuntime.AppDomainAppVirtualPath + ConfigDefaults.CookieNameSuffix).Substring(1),
                CookiePath = appSettings[ConfigKeys.CookiePath] ?? ConfigDefaults.CookiePath,
                CookieTimeoutDuration = appSettings.GetTimeSpanFromMinutes(ConfigKeys.CookieTimeoutMinutes, null) ?? ConfigDefaults.CookieTimeoutDuration,
                IsSignupCodeRequired = appSettings.GetTypedValue(ConfigKeys.IsSignupCodeRequired, ConfigDefaults.IsSignupCodeRequired),
                UseAspSession = appSettings.GetTypedValue(ConfigKeys.UseAspSession, ConfigDefaults.UseAspSession),
                UseSslForSecurity = appSettings.GetTypedValue(ConfigKeys.UseSslForSecurity, ConfigDefaults.UseSslForSecurity),
            };

            return config;
        }

        private static Uri TryReadingActionUrlRedirectOverride(NameValueCollection appSettings)
        {
            return string.IsNullOrEmpty(appSettings[ConfigKeys.NonProductionActionUrlRedirectOverride])
                ? null
                : new Uri(appSettings[ConfigKeys.NonProductionActionUrlRedirectOverride], UriKind.RelativeOrAbsolute);
        }

        private static IList<Guid> GetSupportedTypeVersions(string typeVersionsString)
        {
            Collection<Guid> supportedTypeVersions = new Collection<Guid>();

            typeVersionsString = typeVersionsString ?? string.Empty;
            string[] typeVersions = typeVersionsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string typeVersionClassName in typeVersions)
            {
                Guid supportedTypeId;

                if (s_thingTypeRegistrar.RegisteredTypeHandlersByClassName.TryGetValue(typeVersionClassName, out supportedTypeId))
                {
                    supportedTypeVersions.Add(supportedTypeId);
                }
                else
                {
                    throw new InvalidConfigurationException(Resources.InvalidSupportedTypeVersions);
                }
            }

            return supportedTypeVersions;
        }

        private static byte[] GetEncryptionKey(string encryptionKeyString)
        {
            byte[] encryptionKey = null;

            encryptionKeyString = encryptionKeyString ?? string.Empty;

            if (encryptionKeyString.Length > 0)
            {
                try
                {
                    encryptionKey = HexToBytes(encryptionKeyString);
                    if (encryptionKey.Length != 32)
                    {
                        throw new HealthServiceException(string.Format(Resources.ConfigValueAbsentOrInvalid, ConfigKeys.CookieEncryptionKey));
                    }
                }
                catch (FormatException)
                {
                    throw new HealthServiceException(string.Format(Resources.ConfigValueAbsentOrInvalid, ConfigKeys.CookieEncryptionKey));
                }
            }

            return encryptionKey;
        }

        private static byte[] HexToBytes(string hexString)
        {
            if (hexString.Length % 2 != 0) hexString = "0" + hexString;

            int length = hexString.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }

            return bytes;
        }

        private static Dictionary<string, Uri> GetActionUrls(NameValueCollection appSettings)
        {
            Dictionary<string, Uri> actionUrls = new Dictionary<string, Uri>();
            var keys = appSettings.Keys.Cast<string>();
            int prefixLength = ConfigKeys.ActionPagePrefix.Length;
            foreach (var key in keys)
            {
                if (key.StartsWith(ConfigKeys.ActionPagePrefix))
                {
                    string urlString = appSettings[key];
                    Uri url = string.IsNullOrEmpty(urlString) ? null : new Uri(urlString, UriKind.RelativeOrAbsolute);
                    actionUrls.Add(key.Substring(prefixLength), url);
                }
            }

            return actionUrls;
        }
    }
}
