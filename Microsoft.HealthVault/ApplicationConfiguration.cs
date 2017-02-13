// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Specialized;
using System.Configuration;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Gives access to the configuration file for the application and
    /// exposes some of the settings directly.
    /// </summary>
    /// 
    internal static class ApplicationConfiguration
    {
        /// <summary>
        /// Retrieves the specified settings.
        /// </summary>
        /// 
        /// <param name="configurationKey">
        /// A string specifying the name of the settings to retrieve.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the settings.
        /// </returns>
        /// 
        internal static string GetConfigurationValue(string configurationKey)
        {
            string result = null;

            if (UserAppSettings != null && UserAppSettings.Count > 0)
            {
                result = UserAppSettings[configurationKey];
            }

            if (result == null && AppSettings != null && AppSettings.Count > 0)
            {
                result = AppSettings[configurationKey];
            }

            return result;
        }

        /// <summary>
        /// Gets the application settings.
        /// </summary>
        internal static NameValueCollection AppSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = ConfigurationManager.AppSettings;
                }

                return _appSettings;
            }
        }
        private static volatile NameValueCollection _appSettings;

        /// <summary>
        /// Gets the user application settings.
        /// </summary>
        internal static NameValueCollection UserAppSettings
        {
            get
            {
                if (_userAppSettings == null)
                {
                    _userAppSettings = (NameValueCollection)ConfigurationManager.GetSection("appSettingsUser");
                }

                return _userAppSettings;
            }
        }
        private static volatile NameValueCollection _userAppSettings;
    }
}

