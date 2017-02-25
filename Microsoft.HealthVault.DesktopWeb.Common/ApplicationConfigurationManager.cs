using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;

namespace Microsoft.HealthVault.DesktopWeb.Common
{
    /// <summary>
    /// Gives access to the configuration file for the application and
    /// exposes some of the settings directly.
    /// </summary>
    /// 
    internal static class ApplicationConfigurationManager
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

        #region configuration retrieval helpers

        /// <summary>
        /// Gets the string configuration value given the key
        /// </summary>
        /// <param name="configurationKey">Key to look up the configuration item.</param>
        /// <returns>String value of the configuration item, should return null if key not found.</returns>
        internal static string GetConfigurationString(string configurationKey)
        {
            return GetConfigurationValue(configurationKey);
        }

        /// <summary>
        /// Retrieves the specified setting for strings.
        /// </summary>
        /// 
        /// <param name="key">
        /// A string specifying the name of the setting.
        /// </param>
        /// 
        /// <param name="defaultValue">
        /// A string representing the default string value.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the settings.
        /// </returns>
        /// 
        internal static string GetConfigurationString(string key, string defaultValue)
        {
            string result = GetConfigurationString(key);

            if (result == null)
            {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Bool value from configuration if exists or the specified default value.</returns>
        internal static bool GetConfigurationBoolean(string key, bool defaultValue)
        {
            bool result = defaultValue;
            string resultString = GetConfigurationString(key);
            if (!String.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = bool.Parse(resultString);
            }

            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Int value from configuration if exists or the specified default value.</returns>
        internal static int GetConfigurationInt32(string key, int defaultValue)
        {
            int result = defaultValue;

            string resultString = GetConfigurationString(key);
            if (resultString != null)
            {
                if (!Int32.TryParse(resultString, out result))
                {
                    result = defaultValue;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the URL value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="appendSlash">If set to true, append a '/' character at the end of URL.</param>
        /// <returns>URL value from configuration if exists, null if not found.</returns>
        internal static Uri GetConfigurationUrl(string key, bool appendSlash)
        {
            string resultString = GetConfigurationString(key);
            if (String.IsNullOrEmpty(resultString))
            {
                return null;
            }
            else
            {
                if (appendSlash)
                {
                    return
                        new Uri(resultString.EndsWith("/", StringComparison.Ordinal)
                            ? resultString
                            : (resultString + "/"));
                }
                else
                {
                    return new Uri(resultString);
                }
            }
        }

        /// <summary>
        /// Retrieves the specified setting for GUIDs.
        /// </summary>
        /// 
        /// <param name="key">
        /// A string specifying the name of the setting.
        /// </param>
        /// 
        /// <returns>
        /// The GUID of the setting.
        /// </returns>
        /// 
        internal static Guid GetConfigurationGuid(string key)
        {
            Guid result = Guid.Empty;
            string resultString = GetConfigurationString(key);
            if (!String.IsNullOrEmpty(resultString))
            {
                // Let the FormatException propagate out.
                result = new Guid(resultString);
            }

            return result;
        }

        internal static TimeSpan GetConfigurationTimeSpanMilliseconds(string key, TimeSpan defaultValue)
        {
            int resultInMs = GetConfigurationInt32(key, (int)defaultValue.TotalMilliseconds);
            return TimeSpan.FromMilliseconds(resultInMs);
        }

        /// <summary>
        /// Appends the specified path to the URL after trimming the path.
        /// </summary>
        /// <param name="baseUrl">The base URL to trim and append the path to.
        /// </param>
        /// <param name="path">The path to append to the URL.</param>
        /// <returns>The combined URL and path.</returns>
        private static Uri UrlPathAppend(Uri baseUrl, string path)
        {
            return new Uri(baseUrl, path);
        }

        #endregion
    }
}
