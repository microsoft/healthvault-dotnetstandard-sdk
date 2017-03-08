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

        public override StoreLocation SignatureCertStoreLocation
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
       
    }
}
