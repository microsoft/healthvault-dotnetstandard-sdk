namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// <inheritdoc cref="ICryptoConfiguration"/>
    /// </summary>
    internal class BaseCryptoConfiguration : ICryptoConfiguration
    {
        /// <summary>
        /// The default HMAC algorithm name.
        /// </summary>
        public const string DefaultHmacAlgorithmName = "HMACSHA256";

        /// <summary>
        /// The default hash algorithm name.
        /// </summary>
        public const string DefaultHashAlgorithmName = "SHA256";

        /// <summary>
        /// The default signature hash algorithm name.
        /// </summary>
        public const string DefaultSignatureHashAlgorithmName = "SHA1";

        /// <summary>
        /// The default signature algorithm name.
        /// </summary>
        public const string DefaultSignatureAlgorithmName = "RSA-SHA1";

        /// <summary>
        /// The default symmetric algorithm name.
        /// </summary>
        public const string DefaultSymmetricAlgorithmName = "AES256";

        private volatile string hmacAlgorithmName;

        private volatile string hashAlgorithmName;

        private volatile string signatureHashAlgorithmName;

        private volatile string signatureAlgorithmName;

        private volatile string symmetricAlgorithmName;

        public string HmacAlgorithmName
        {
            get { return this.hmacAlgorithmName ?? (this.hmacAlgorithmName = DefaultHmacAlgorithmName); }

            set { this.hmacAlgorithmName = value; }
        }

        public string HashAlgorithmName
        {
            get { return this.hashAlgorithmName ?? (this.hashAlgorithmName = DefaultHashAlgorithmName); }

            set { this.hashAlgorithmName = value; }
        }

        public string SignatureHashAlgorithmName
        {
            get { return this.signatureHashAlgorithmName ?? (this.signatureHashAlgorithmName = DefaultSignatureHashAlgorithmName); }

            set { this.signatureHashAlgorithmName = value; }
        }

        public string SignatureAlgorithmName
        {
            get { return this.signatureAlgorithmName ?? (this.signatureAlgorithmName = DefaultSignatureAlgorithmName); }

            set { this.signatureAlgorithmName = value; }
        }

        public string SymmetricAlgorithmName
        {
            get { return this.symmetricAlgorithmName ?? (this.symmetricAlgorithmName = DefaultSymmetricAlgorithmName); }

            set { this.symmetricAlgorithmName = value; }
        }
    }
}
