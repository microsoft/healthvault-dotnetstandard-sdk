namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// <inheritdoc cref="ICryptoConfiguration"/>
    /// </summary>
    public class DefaultCryptoConfiguration : ICryptoConfiguration
    {
        /// <summary>
        /// The default HMAC algorithm name.
        /// </summary>
        protected const string DefaultHmacAlgorithmName = "HMACSHA256";

        private volatile string _hmacAlgorithmName;

        public string HmacAlgorithmName
        {
            get { return _hmacAlgorithmName ?? (_hmacAlgorithmName = DefaultHmacAlgorithmName); }

            set { _hmacAlgorithmName = value; }
        }
    }
}
