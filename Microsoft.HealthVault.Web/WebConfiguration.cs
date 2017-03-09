using Microsoft.HealthVault.DesktopWeb.Common;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.HealthVault.Configurations;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Class used for web configurations
    /// </summary>
    public class WebConfiguration: ConfigurationBase
    {
        private static readonly object instanceLock = new object();

        // Application related configuration keys
        private const string ConfigKeyApplicationCertificateFileName = "ApplicationCertificateFilename";
        private const string ConfigKeyApplicationCertificatePassword = "ApplicationCertificatePassword";
        private const string ConfigKeyCertSubject = "AppCertSubject";
        private const string ConfigKeySignatureCertStoreLocation = "SignatureCertStoreLocation";
        private bool isLocked = false;

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

        internal void lockConfiguration()
        {
            isLocked = true;
        }

        public override string ApplicationCertificatePassword
        {
            get
            {
                return _applicationCertificatePassword;
            }
            set
            {
                if (!isLocked)
                {
                    _applicationCertificatePassword = value;
                }
                else
                {
                    throw Validator.InvalidConfigurationException("ApplicationCertificatePassword");
                }
            }
        }

        private volatile string _applicationCertificatePassword;


        public override string ApplicationCertificateFileName
        {
            get
            {
                return _applicationCertificateFileName;
            }
            set
            {
                if (!isLocked)
                {
                    _applicationCertificateFileName = value;
                }
                else
                {
                    throw Validator.InvalidConfigurationException("ApplicationCertificateFileName");
                }
            }
        }

        private volatile string _applicationCertificateFileName;

        public virtual StoreLocation SignatureCertStoreLocation
        {
            get
            {
                return _signatureCertStoreLocation;
            }
            set
            {
                if (!isLocked)
                {
                    _signatureCertStoreLocation = value;
                }
                else
                {
                    throw Validator.InvalidConfigurationException("SignatureCertStoreLocation");
                }
            }
        }

        private StoreLocation _signatureCertStoreLocation;
        private const string DefaultSignatureCertStoreLocation = "LocalMachine";

        public override string CertSubject
        {
            get
            {
                return _certSubject;
            }
            set
            {
                if (!isLocked)
                {
                    _certSubject = value;
                }
                else
                {
                    throw Validator.InvalidConfigurationException("CertSubject");
                }
            }
        }

        private volatile string _certSubject;

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
               return _useSslForSecurity;
            }
            set
            {
                if (!isLocked)
                {
                    _useSslForSecurity = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile bool _useSslForSecurity;
        private volatile bool _useSslForSecurityInitialized;
        private const bool DefaultUseSslForSecurity = true;

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
                return _useAspSession;
            }
            set
            {
                if (!isLocked)
                {
                    _useAspSession = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile bool _useAspSession;
        private volatile bool _useAspSessionInitialized;
        private const bool DefaultUseAspSession = false;

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
                return _cookieEncryptionKey;
            }
            set
            {
                if (!isLocked)
                {
                    _cookieEncryptionKey = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile byte[] _cookieEncryptionKey;
        private volatile bool _cookeEncryptionInitialized;

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
                return _isSignupCodeRequired;
            }
            set
            {
                if (!isLocked)
                {
                    _isSignupCodeRequired = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile bool _isSignupCodeRequired;
        private volatile bool _isSignupCodeRequiredInitialized;
        private const bool DefaultIsSignupCodeRequired = false;

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
                return _actionUrlRedirectOverride;
            }
            set
            {
                if (!isLocked)
                {
                    _actionUrlRedirectOverride = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile Uri _actionUrlRedirectOverride;
        private volatile bool _actionUrlRedirectOverrideInitialized;

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
                return _defaultCookieTimeout;
            }
            set
            {
                if (!isLocked)
                {
                    _defaultCookieTimeout = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile int _defaultCookieTimeout;
        private volatile bool _defaultCookieTimeoutInitialized;
        private const int DefaultCookieTimeoutMinutes = 20;

        private int GetMaxCookieTimeout()
        {
            return DefaultMaxCookieTimeoutMinutes;
        }

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
                return _cookieDomain;
            }
            set
            {
                if (!isLocked)
                {
                    _cookieDomain = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
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
                return _cookiePath;
            }
            set
            {
                if (!isLocked)
                {
                    _cookiePath = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile string _cookiePath;
        private volatile bool _cookiePathInitialized;
        private const string DefaultCookiePath = "";

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
               return _allowedRedirectSites;
            }
            set
            {
                if (!isLocked)
                {
                    _allowedRedirectSites = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
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
                return _isMultipleRecordApplication;
            }
            set
            {
                if (!isLocked)
                {
                    _isMultipleRecordApplication = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
            }
        }
        private volatile bool _isMultipleRecordApplication;
        private volatile bool _isMultipleRecordApplicationInitialized;
        private const bool DefaultIsMra = false;

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
                return _dataGridItemsPerPage.Value;
            }
            set
            {
                if (!isLocked)
                {
                    _dataGridItemsPerPage = value;
                }
                else
                {
                    throw Validator.InvalidOperationException("CannotSetPropertyAfterConnectionCreation");
                }
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
            // TODO:  We need to refactor to not get values from config file
            // string resultUrl = GetConfigurationString(ConfigKeyActionPagePrefix + action, null);
            // return !String.IsNullOrEmpty(resultUrl) ?
            //        new Uri(resultUrl, UriKind.RelativeOrAbsolute) : null;
            return new Uri(action);
        }
    }
}
