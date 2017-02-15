using System.Security;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Sha256 Hmac implementation which delegates to Win32 apis.
    /// </summary>
    internal class Sha256 : HashAlgorithm
    {
        private CryptoServiceProviderHash _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sha256Hmac"/> class.
        /// </summary>
        [SecuritySafeCritical]
        internal Sha256()
        {
            _hash = new CryptoServiceProviderHash(
                CryptoConfiguration.CryptoServiceProviderName,
                ProviderType.RsaAes,
                AlgorithmId.Sha256);

            try
            {
                _hash.Initialize();
            }
            catch
            {
                _hash.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
        [SecuritySafeCritical]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_hash != null)
                {
                    _hash.Dispose();
                    _hash = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the size of the hash.
        /// </summary>
        /// <value>The size of the hash.</value>
        public override int HashSize
        {
            get { return 0x100; }
        }

        /// <summary>
        /// Core hashing function called by parent.
        /// </summary>
        ///
        /// <param name="array">
        /// The data to hash.
        /// </param>
        ///
        /// <param name="ibStart">
        /// The offset into the array.
        /// </param>
        ///
        /// <param name="cbSize">
        /// The count of bytes to hash.
        /// </param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hash.HashCore(array, ibStart, cbSize);
        }

        /// <summary>
        /// Finalizes the hash.
        /// </summary>
        ///
        /// <returns>
        /// The final hash.
        /// </returns>
        protected override byte[] HashFinal()
        {
            return _hash.HashFinal();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SecuritySafeCritical]
        public override void Initialize()
        {
            _hash.Initialize();
        }
    }
}
