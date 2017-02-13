using System;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Sha256 Hmac implementation which delegates to Win32 apis.
    /// </summary>
    internal class Sha256Hmac : HMAC
    {
        private CryptoServiceProviderHmac _hmac;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sha256Hmac"/> class.
        /// </summary>
        internal Sha256Hmac()
            : this(NewKey())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sha256Hmac"/> class.
        /// </summary>
        /// 
        /// <param name="key">
        /// The hmac key.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="key"/> is null.
        /// </exception>
        [SecuritySafeCritical]
        internal Sha256Hmac(byte[] key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            _hmac = new CryptoServiceProviderHmac(
                CryptoConfiguration.CryptoServiceProviderName,
                ProviderType.RsaAes,
                AlgorithmId.Sha256);

            _hmac.Key = key;

            try
            {
                _hmac.Initialize();
            }
            catch
            {
                _hmac.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// 
        /// <param name="disposing">true to release both managed and unmanaged resources; 
        /// false to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_hmac != null)
                {
                    _hmac.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the size of the hash.
        /// </summary>
        /// 
        /// <value>
        /// The size of the hash.
        /// </value>
        public override int HashSize
        {
            get { return 0x100; }
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// 
        /// <value>
        /// The key.
        /// </value>
        public override byte[] Key
        {
            get { return _hmac.Key; }
            set
            {
                _hmac.Key = value;
                Initialize();
            }
        }

        /// <summary>
        /// Core hashing function called by parent.
        /// </summary>
        /// 
        /// <param name="rgb">
        /// The data to hash.
        /// </param>
        /// 
        /// <param name="ib">
        /// The offset into the array.
        /// </param>
        /// 
        /// <param name="cb">
        /// The count of bytes to hash.
        /// </param>
        protected override void HashCore(byte[] rgb, int ib, int cb)
        {
            _hmac.HashCore(rgb, ib, cb);
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
            return _hmac.HashFinal();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SecuritySafeCritical]
        public override void Initialize()
        {
            _hmac.Initialize();
        }

        private static byte[] NewKey()
        {
            byte[] key = new byte[64];

            using (RNGCryptoServiceProvider randomProvider = new RNGCryptoServiceProvider())
            {
                randomProvider.GetBytes(key);
            }

            return key;
        }
    }
}
