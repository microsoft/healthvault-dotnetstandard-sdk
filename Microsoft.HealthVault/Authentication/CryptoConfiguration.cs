namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// <inheritdoc cref="ICryptoConfiguration"/>
    /// </summary>
    public class CryptoConfiguration : ICryptoConfiguration
    {
        /// <summary>
        /// The default HMAC algorithm name.
        /// </summary>
        protected const string DefaultHmacAlgorithmName = "HMACSHA256";

        /// <summary>
        /// The default hash algorithm name.
        /// </summary>
        protected const string DefaultHashAlgorithmName = "SHA256";

        /// <summary>
        /// The default signature hash algorithm name.
        /// </summary>
        protected const string DefaultSignatureHashAlgorithmName = "SHA1";

        /// <summary>
        /// The default signature algorithm name.
        /// </summary>
        protected const string DefaultSignatureAlgorithmName = "RSA-SHA1";

        /// <summary>
        /// The default symmetric algorithm name.
        /// </summary>
        protected const string DefaultSymmetricAlgorithmName = "AES256";

        private volatile string _hmacAlgorithmName;

        private volatile string _hashAlgorithmName;

        private volatile string _signatureHashAlgorithmName;

        private volatile string _signatureAlgorithmName;

        private volatile string _symmetricAlgorithmName;

        public string HmacAlgorithmName
        {
            get { return _hmacAlgorithmName ?? (_hmacAlgorithmName = DefaultHmacAlgorithmName); }

            set { _hmacAlgorithmName = value; }
        }

        public string HashAlgorithmName
        {
            get { return _hashAlgorithmName ?? (_hashAlgorithmName = DefaultHashAlgorithmName); }

            set { _hashAlgorithmName = value; }
        }

        public string SignatureHashAlgorithmName
        {
            get { return _signatureHashAlgorithmName ?? (_signatureHashAlgorithmName = DefaultSignatureHashAlgorithmName); }

            set { _signatureHashAlgorithmName = value; }
        }

        public string SignatureAlgorithmName
        {
            get { return _signatureAlgorithmName ?? (_signatureAlgorithmName = DefaultSignatureAlgorithmName); }

            set { _signatureAlgorithmName = value; }
        }

        public string SymmetricAlgorithmName
        {
            get { return _symmetricAlgorithmName ?? (_symmetricAlgorithmName = DefaultSymmetricAlgorithmName); }

            set { _symmetricAlgorithmName = value; }
        }
    }
}
