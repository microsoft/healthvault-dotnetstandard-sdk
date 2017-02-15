// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// </summary>
    ///
    public class HealthWebApplicationConfiguration : HealthApplicationConfiguration
    {
        #region configuration key constants
        private const string ConfigKeyItemsPerPage = "DataGrid_ItemsPerPage";
        private const string ConfigKeyActionPagePrefix = "WCPage_Action";
        private const string ConfigKeyAllowedRedirectSites = "WCPage_AllowedRedirectSites";
        private const string ConfigKeyIsMra = "WCPage_IsMRA";
        private const string ConfigKeyUseAspSession = "WCPage_UseAspSession";
        private const string ConfigKeyUseSslForSecurity = "WCPage_SSLForSecure";
        private const string ConfigKeyCookieEncryptionKey = "WCPage_CookieEncryptionKey";
        private const string ConfigKeyMaxCookieTimeoutMinutesSpelledCorrectly = "WCPage_MaxCookieTimeoutMinutes";
        private const string ConfigKeyMaxCookieTimeoutMinutes = "WCPage_MaxCookieTimeoutMintes";
        private const string ConfigKeyCookieTimeoutMinutes = "WCPage_CookieTimeoutMinutes";
        private const string ConfigKeyCookieDomain = "WCPage_CookieDomain";
        private const string ConfigKeyCookiePath = "WCPage_CookiePath";
        private const string ConfigKeyNonProductionActionUrlRedirectOverride = "NonProductionActionUrlRedirectOverride";
        private const string ConfigKeyIsSignupCodeRequired = "WCPage_IsSignupCodeRequired";
        #endregion

        static HealthWebApplicationConfiguration()
        {
            HealthApplicationConfiguration.Current = Current;
        }

        /// <summary>
        /// Gets or sets the current configuration object for the app-domain.
        /// </summary>
        public new static HealthWebApplicationConfiguration Current
        {
            get { return _current; }
            set
            {
                _current = value;
                HealthApplicationConfiguration.Current = _current;
            }
        }
        private static volatile HealthWebApplicationConfiguration _current =
            new HealthWebApplicationConfiguration();

        /// <summary>
        /// Gets the number of items that are shown per page when using the
        /// <see cref="HealthRecordItemDataGrid"/>.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "DataGrid_ItemsPerPage" configuration
        /// value. The value defaults to 20.
        /// </remarks>
        ///
        public virtual int DataGridItemsPerPage
        {
            get
            {
                if (_dataGridItemsPerPage == null)
                {
                    _dataGridItemsPerPage = GetConfigurationInt32(ConfigKeyItemsPerPage, DefaultItemsPerPage);
                }

                return _dataGridItemsPerPage.Value;
            }
        }
        private int? _dataGridItemsPerPage;
        private const int DefaultItemsPerPage = 20;

        /// <summary>
        /// Gets the URL of the page corresponding to the action.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_Action*" configuration
        /// values.
        /// </remarks>
        ///
        public virtual Uri GetActionUrl(string action)
        {
            string resultUrl = GetConfigurationString(ConfigKeyActionPagePrefix + action, null);
            return !String.IsNullOrEmpty(resultUrl) ?
                    new Uri(resultUrl, UriKind.RelativeOrAbsolute) : null;
        }

        /// <summary>
        /// Gets the list of allowed redirect sites.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_AllowedRedirectSites" configuration
        /// value.
        /// </remarks>
        ///
        public virtual string AllowedRedirectSites
        {
            get
            {
                if (!_allowedRedirectSitesInitialized)
                {
                    _allowedRedirectSites = GetConfigurationString(ConfigKeyAllowedRedirectSites, null);
                    _allowedRedirectSitesInitialized = true;
                }

                return _allowedRedirectSites;
            }
        }
        private volatile string _allowedRedirectSites;
        private volatile bool _allowedRedirectSitesInitialized;

        /// <summary>
        /// Gets a value indicating whether the application works with multiple records
        /// at one time or just one.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_IsMRA" configuration
        /// value. The value defaults to false.
        /// </remarks>
        ///
        public virtual bool IsMultipleRecordApplication
        {
            get
            {
                if (!_isMultipleRecordApplicationInitialized)
                {
                    _isMultipleRecordApplication = GetConfigurationBoolean(ConfigKeyIsMra, DefaultIsMra);
                    _isMultipleRecordApplicationInitialized = true;
                }

                return _isMultipleRecordApplication;
            }
        }
        private volatile bool _isMultipleRecordApplication;
        private volatile bool _isMultipleRecordApplicationInitialized;
        private const bool DefaultIsMra = false;

        /// <summary>
        /// Gets a value indicating whether the HealthVault token should be stored
        /// in the ASP.NET session rather than a cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_UseAspSession" configuration
        /// value. The value defaults to "false".
        /// </remarks>
        ///
        public virtual bool UseAspSession
        {
            get
            {
                if (!_useAspSessionInitialized)
                {
                    _useAspSession = GetConfigurationBoolean(ConfigKeyUseAspSession, DefaultUseAspSession);
                    _useAspSessionInitialized = true;
                }

                return _useAspSession;
            }
        }
        private volatile bool _useAspSession;
        private volatile bool _useAspSessionInitialized;
        private const bool DefaultUseAspSession = false;

        /// <summary>
        /// Gets a value indicating whether <see cref="HealthServicePage"/> should automatically
        /// redirect to SSL ports when reached through an unsecured port.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_SSLForSecure" configuration
        /// value. The value defaults to "true".
        /// </remarks>
        ///
        public virtual bool UseSslForSecurity
        {
            get
            {
                if (!_useSslForSecurityInitialized)
                {
                    _useSslForSecurity = GetConfigurationBoolean(ConfigKeyUseSslForSecurity, DefaultUseSslForSecurity);
                    _useSslForSecurityInitialized = true;
                }

                return _useSslForSecurity;
            }
        }
        private volatile bool _useSslForSecurity;
        private volatile bool _useSslForSecurityInitialized;
        private const bool DefaultUseSslForSecurity = true;

        /// <summary>
        /// Gets the name to use for the cookie which stores login information for the
        /// user.
        /// </summary>
        ///
        /// <remarks>
        /// The value defaults to <see cref="HttpRuntime.AppDomainAppVirtualPath"/> + "_wcpage".
        /// </remarks>
        ///
        public virtual string CookieName
        {
            get
            {
                return HttpContext.Current == null ? String.Empty : (HttpRuntime.AppDomainAppVirtualPath + "_wcpage").Substring(1);
            }
        }

        /// <summary>
        /// Gets the key used to encrypt the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_CookieEncryptionKey" configuration
        /// value.
        /// </remarks>
        ///
        public virtual byte[] CookieEncryptionKey
        {
            get
            {
                if (!_cookeEncryptionInitialized)
                {
                    _cookieEncryptionKey = GetEncryptionKey();
                    _cookeEncryptionInitialized = true;
                }

                return _cookieEncryptionKey;
            }
        }
        private volatile byte[] _cookieEncryptionKey;
        private volatile bool _cookeEncryptionInitialized;

        /// <summary>
        /// Gets the maximum time a cookie will be stored.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_MaxCookieTimeoutMinutes" configuration
        /// value. The value defaults to 129600 (90 days).
        /// </remarks>
        ///
        public virtual int MaxCookieTimeoutMinutes
        {
            get
            {
                if (!_maxCookieTimeoutMinutesInitialized)
                {
                    _maxCookieTimeoutMinutes = GetMaxCookieTimeout();
                    _maxCookieTimeoutMinutesInitialized = true;
                }

                return _maxCookieTimeoutMinutes;
            }
        }
        private volatile int _maxCookieTimeoutMinutes;
        private volatile bool _maxCookieTimeoutMinutesInitialized;
        private const int DefaultMaxCookieTimeoutMinutes = 129600;

        /// <summary>
        /// Gets the time a cookie will be stored.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_CookieTimeoutMinutes" configuration
        /// value. The value defaults to 20.
        /// </remarks>
        ///
        public virtual int CookieTimeoutMinutes
        {
            get
            {
                if (!_defaultCookieTimeoutInitialized)
                {
                    _defaultCookieTimeout = GetConfigurationInt32(ConfigKeyCookieTimeoutMinutes, DefaultCookieTimeoutMinutes);
                    _defaultCookieTimeoutInitialized = true;
                }

                return _defaultCookieTimeout;
            }
        }
        private volatile int _defaultCookieTimeout;
        private volatile bool _defaultCookieTimeoutInitialized;
        private const int DefaultCookieTimeoutMinutes = 20;

        /// <summary>
        /// Gets the domain that will be used for the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_CookieDomain" configuration
        /// value. The value defaults to "".
        /// </remarks>
        ///
        public virtual string CookieDomain
        {
            get
            {
                if (!_cookieDomainInitialized)
                {
                    _cookieDomain = GetConfigurationString(ConfigKeyCookieDomain, DefaultCookieDomain);
                    _cookieDomainInitialized = true;
                }

                return _cookieDomain;
            }
        }
        private volatile string _cookieDomain;
        private volatile bool _cookieDomainInitialized;
        private const string DefaultCookieDomain = "";

        /// <summary>
        /// Gets the path to be used for the cookie.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "WCPage_CookiePath" configuration
        /// value. The value defaults to "".
        /// </remarks>
        ///
        public virtual string CookiePath
        {
            get
            {
                if (!_cookiePathInitialized)
                {
                    _cookiePath = GetConfigurationString(ConfigKeyCookiePath, DefaultCookiePath);
                    _cookiePathInitialized = true;
                }

                return _cookiePath;
            }
        }
        private volatile string _cookiePath;
        private volatile bool _cookiePathInitialized;
        private const string DefaultCookiePath = "";

        /// <summary>
        /// Gets a URL to use in place of the action URL in testing environments.
        /// </summary>
        ///
        /// <remarks>
        /// This property corresponds to the "NonProductionActionUrlRedirectOverride" configuration
        /// value.
        /// </remarks>
        ///
        public virtual Uri ActionUrlRedirectOverride
        {
            get
            {
                if (!_actionUrlRedirectOverrideInitialized)
                {
                    string url = GetConfigurationString(ConfigKeyNonProductionActionUrlRedirectOverride, String.Empty);

                    _actionUrlRedirectOverride = String.IsNullOrEmpty(url) ? null : new Uri(url, UriKind.RelativeOrAbsolute);
                    _actionUrlRedirectOverrideInitialized = true;
                }

                return _actionUrlRedirectOverride;
            }
        }
        private volatile Uri _actionUrlRedirectOverride;
        private volatile bool _actionUrlRedirectOverrideInitialized;

        /// <summary>
        /// Gets the scheme to use for secure HTTP addresses.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to "https://".
        /// </remarks>
        ///
        public virtual string SecureHttpScheme
        {
            get
            {
                return "https://";
            }
        }

        /// <summary>
        /// Gets the scheme to use for insecure HTTP addresses.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to "http://".
        /// </remarks>
        ///
        public virtual string InsecureHttpScheme
        {
            get
            {
                return "http://";
            }
        }

        /// <summary>
        /// Gets a value indicating whether a signup code is required when a user
        /// signs up for a HealthVault account.
        /// </summary>
        ///
        /// <remarks>
        /// A signup code is only required under certain conditions. For instance,
        /// the account is being created from outside the United States.
        /// This property corresponds to the "WCPage_IsSignupCodeRequired" configuration
        /// value. The value defaults to "false".
        /// </remarks>
        ///
        public virtual bool IsSignupCodeRequired
        {
            get
            {
                if (!_isSignupCodeRequiredInitialized)
                {
                    _isSignupCodeRequired = GetConfigurationBoolean(ConfigKeyIsSignupCodeRequired, DefaultIsSignupCodeRequired);
                    _isSignupCodeRequiredInitialized = true;
                }

                return _isSignupCodeRequired;
            }
        }
        private volatile bool _isSignupCodeRequired;
        private volatile bool _isSignupCodeRequiredInitialized;
        private const bool DefaultIsSignupCodeRequired = false;

        /// <summary>
        /// Gets the URL of the HealthVault Shell authentication page.
        /// </summary>
        ///
        /// <remarks>
        /// This property uses the "ShellUrl" configuration value to construct the
        /// redirector URL with a target of "AUTH".
        /// </remarks>
        ///
        public virtual Uri HealthVaultShellAuthenticationUrl
        {
            get
            {
                var redirect = new ShellRedirectParameters(HealthVaultShellUrl.OriginalString)
                {
                    TargetLocation = "AUTH",
                    ApplicationId = ApplicationId,
                };

                return redirect.ConstructRedirectUrl();
            }
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
        private string GetConfigurationString(string key, string defaultValue)
        {
            string result = GetConfigurationString(key);

            if (result == null)
            {
                if (defaultValue == null)
                {
                    ThrowIfConfigValueMissing(key);
                }
                else
                {
                    result = defaultValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Bool value from configuration if exists or the specified default value.</returns>
        private bool GetConfigurationBoolean(string key, bool defaultValue)
        {
            string resultString = GetConfigurationString(key, defaultValue.ToString(CultureInfo.InvariantCulture));

            bool result = defaultValue;
            if (resultString != null)
            {
                if (!Boolean.TryParse(resultString, out result))
                {
                    ThrowIfConfigValueMissing(key);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the bool value matching the configuration key.
        /// </summary>
        /// <param name="key">Key to use to find the value.</param>
        /// <param name="defaultValue">Set the value to provided default value if no configuration value can be found for the key.</param>
        /// <returns>Int value from configuration if exists or the specified default value.</returns>
        private int GetConfigurationInt32(string key, int defaultValue)
        {
            string resultString = GetConfigurationString(key, defaultValue.ToString(CultureInfo.InvariantCulture));

            int result = -1;
            if (!Int32.TryParse(resultString, out result))
            {
                ThrowIfConfigValueMissing(key);
            }

            return result;
        }

        private int GetMaxCookieTimeout()
        {
            string maxCookieTimeoutMinutesString = null;
            int result = DefaultMaxCookieTimeoutMinutes;
            try
            {
                maxCookieTimeoutMinutesString = GetConfigurationString(ConfigKeyMaxCookieTimeoutMinutesSpelledCorrectly, null);
            }
            catch (HealthServiceException)
            {
                try
                {
                    // try the misspelled key instead
                    maxCookieTimeoutMinutesString = GetConfigurationString(ConfigKeyMaxCookieTimeoutMinutes, null);
                }
                catch (HealthServiceException)
                {
                    maxCookieTimeoutMinutesString = DefaultMaxCookieTimeoutMinutes.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (maxCookieTimeoutMinutesString != null)
            {
                if (!Int32.TryParse(maxCookieTimeoutMinutesString, out result))
                {
                    ThrowIfConfigValueMissing(
                        ConfigKeyMaxCookieTimeoutMinutesSpelledCorrectly);
                }
            }

            return result;
        }

        private byte[] GetEncryptionKey()
        {
            byte[] encryptionKey = null;

            string encryptionKeyString = GetConfigurationString(ConfigKeyCookieEncryptionKey, String.Empty);

            if (encryptionKeyString.Length > 0)
            {
                try
                {
                    encryptionKey = HexToBytes(encryptionKeyString);
                    if (encryptionKey.Length != 32)
                    {
                        encryptionKey = null;
                        // Throw if config is present but malformed.
                        ThrowIfConfigValueMissing(ConfigKeyCookieEncryptionKey);
                    }
                }
                catch (FormatException)
                {
                    // Config value is present but malformed.
                    ThrowIfConfigValueMissing(ConfigKeyCookieEncryptionKey);
                }
            }

            return encryptionKey;
        }

        private static void ThrowIfConfigValueMissing(string key)
        {
            HealthServiceResponseError error = new HealthServiceResponseError();
            error.Message = ResourceRetriever.FormatResourceString(
                "ConfigValueAbsentOrMalformed",
                key);
            HealthServiceException e =
                HealthServiceExceptionHelper.GetHealthServiceException(
                    HealthServiceStatusCode.ConfigValueMissingOrMalformed,
                    error);
            throw e;
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
    }
}
