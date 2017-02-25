using Microsoft.HealthVault.Authentication;

namespace Microsoft.HealthVault.DesktopWeb.Common
{
    public class AppCryptoConfiguration : ICryptoConfiguration
    {
        // Security related keys
        private const string ConfigKeyHmacAlgorithmName = "HmacAlgorithmName";
        private const string ConfigKeyHashAlgorithmName = "HashAlgorithmName";
        private const string ConfigKeySignatureHashAlgorithmName = "SignatureHashAlgorithmName";
        private const string ConfigKeySignatureAlgorithmName = "SignatureAlgorithmName";
        private const string ConfigSymmetricAlgorithmName = "SymmetricAlgorithmName";

        /// <summary>
        /// Gets the preferred application-wide Hash Message Authentication Code
        /// (HMAC) algorithm name.
        /// </summary>
        /// <remarks>
        /// The application-wide algorithm name may be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// This algorithm name can be used to construct an HMAC primitive
        /// using <see cref="M:Microsoft.HealthVault.Authentication.ICryptoService.CreateHmac(System.String)" />
        /// </remarks>
        public virtual string HmacAlgorithmName
        {
            get
            {
                if (_hmacAlgorithmName == null)
                {
                    _hmacAlgorithmName = ApplicationConfigurationManager.GetConfigurationString(
                        ConfigKeyHmacAlgorithmName, 
                        BaseCryptoConfiguration.DefaultHmacAlgorithmName);
                }

                return _hmacAlgorithmName;
            }
            set
            {
                _hmacAlgorithmName = value;
            }
        }
        private volatile string _hmacAlgorithmName;

        /// <summary>
        /// Gets the preferred application-wide hash algorithm name.
        /// </summary>
        /// <remarks>
        /// The application-wide algorithm name can be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// This algorithm name can be used to construct a hash primitive using
        /// <see cref="M:Microsoft.HealthVault.Authentication.ICryptoService.CreateHashAlgorithm(System.String)" />
        /// .
        /// </remarks>
        public virtual string HashAlgorithmName
        {
            get
            {
                if (_hashAlgorithmName == null)
                {
                    _hashAlgorithmName = ApplicationConfigurationManager.GetConfigurationString(
                        ConfigKeyHashAlgorithmName, 
                        BaseCryptoConfiguration.DefaultHashAlgorithmName);
                }

                return _hashAlgorithmName;
            }
            set
            {
                _hashAlgorithmName = value;
            }
        }
        private volatile string _hashAlgorithmName;

        /// <summary>
        /// Gets the preferred application-wide hash algorithm name for
        /// computing digests to be used for signature generation.
        /// </summary>
        /// <remarks>
        /// The application-wide algorithm name can be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// This algorithm name can be used to construct a hash primitive using
        /// <see cref="M:Microsoft.HealthVault.Authentication.ICryptoService.CreateHashAlgorithm(System.String)" />.
        /// </remarks>
        public virtual string SignatureHashAlgorithmName
        {
            get
            {
                if (_signatureHashAlgorithmName == null)
                {
                    _signatureHashAlgorithmName = ApplicationConfigurationManager.GetConfigurationString(
                        ConfigKeySignatureHashAlgorithmName,
                        BaseCryptoConfiguration.DefaultSignatureHashAlgorithmName);
                }

                return _signatureHashAlgorithmName;
            }
            set { _signatureHashAlgorithmName = value; }
        }
        private volatile string _signatureHashAlgorithmName;

        /// <summary>
        /// Gets the preferred application-wide signature algorithm name.
        /// </summary>
        /// <remarks>
        /// The application-wide algorithm name can be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// The signature signing algorithm is currently RSA. The RSA algorithm
        /// name is prepended to the default
        /// <seealso cref="P:Microsoft.HealthVault.Authentication.ICryptoConfiguration.SignatureHashAlgorithmName" />.
        /// </remarks>
        public virtual string SignatureAlgorithmName
        {
            get
            {
                if (_signatureAlgorithmName == null)
                {
                    _signatureAlgorithmName = ApplicationConfigurationManager.GetConfigurationString(
                        ConfigKeySignatureAlgorithmName,
                        BaseCryptoConfiguration.DefaultSignatureAlgorithmName);
                }

                return _signatureAlgorithmName;
            }
            set
            {
                _signatureAlgorithmName = value;
            }
        }
        private volatile string _signatureAlgorithmName;

        /// <summary>
        /// Gets the preferred application-wide symmetric algorithm name.
        /// </summary>
        /// <remarks>
        /// The application-wide algorithm name can be specified in the
        /// configuration, but if it is not, then a default value is used.
        /// The symmetric algorithm name can be used to construct a
        /// symmetric algorithm via <see cref="M:Microsoft.HealthVault.Authentication.ICryptoService.CreateSymmetricAlgorithm(System.String,System.Byte[])" />.
        /// </remarks>
        public string SymmetricAlgorithmName
        {
            get
            {
                if (_symmetricAlgorithmName == null)
                {
                    _symmetricAlgorithmName = ApplicationConfigurationManager.GetConfigurationString(
                        ConfigSymmetricAlgorithmName,
                        BaseCryptoConfiguration.DefaultSymmetricAlgorithmName);
                }

                return _symmetricAlgorithmName;
            }
            set { _symmetricAlgorithmName = value; }
        }
        private volatile string _symmetricAlgorithmName;
    }
}
