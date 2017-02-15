using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Implements base functionality for Capi hashes.  This class manages
    /// the hash algorithm callbacks and manages the Capi context and hash
    /// handles.
    /// </summary>
    internal abstract class AbstractCryptoServiceProviderHash : IDisposable
    {
        private AlgorithmId _algorithmId;

        [SecurityCritical]
        private SafeCryptoContextHandle _cryptoContextHandle;

        [SecurityCritical]
        private SafeCryptoHashHandle _cryptoHashHandle;
        private String _provider;
        private ProviderType _providerType;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AbstractCryptoServiceProviderHash"/> class.
        /// </summary>
        ///
        /// <param name="provider">
        /// The name of the Capi provider.
        /// </param>
        ///
        /// <param name="providerType">
        /// Type of the provider.
        /// </param>
        ///
        /// <param name="algorithm">
        /// The hash algorithm.
        /// </param>
        internal AbstractCryptoServiceProviderHash(
            String provider,
            ProviderType providerType,
            AlgorithmId algorithm)
        {
            this._algorithmId = algorithm;
            this._provider = provider;
            this._providerType = providerType;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [SecurityCritical]
        internal void Initialize()
        {
            Reset();

            if (!UnsafeNativeMethods.CryptAcquireContext(
                out _cryptoContextHandle,
                null,
                _provider,
                _providerType,
                CryptoAcquireContextFlags.None
                    | CryptoAcquireContextFlags.VerifyContext))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            _cryptoHashHandle = NewHashHandle();
        }

        [SecurityCritical]
        internal virtual void Reset()
        {
            CryptoUtil.DisposeHandle(ref _cryptoHashHandle);
            CryptoUtil.DisposeHandle(ref _cryptoContextHandle);
        }

        [SecurityCritical]
        internal abstract SafeCryptoHashHandle NewHashHandle();

        /// <summary>
        /// Gets the crypto context.
        /// </summary>
        ///
        /// <value>
        /// The crypto context.
        /// </value>
        internal SafeCryptoContextHandle CryptoContext
        {
            [SecurityCritical]
            get { return _cryptoContextHandle; }
        }

        /// <summary>
        /// Gets the algorithm id.
        /// </summary>
        ///
        /// <value>
        /// The algorithm id.
        /// </value>
        internal AlgorithmId AlgorithmId
        {
            get { return _algorithmId; }
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        [SecuritySafeCritical]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        ///
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// </param>
        [SecurityCritical]
        protected virtual void Dispose(bool disposing)
        {
            Reset();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="AbstractCryptoServiceProviderHash"/> is reclaimed by garbage collection.
        /// </summary>
        [SecuritySafeCritical]
        ~AbstractCryptoServiceProviderHash()
        {
            Dispose(false);
        }

        /// <summary>
        /// Implements the core hashing functionality.
        /// </summary>
        ///
        /// <param name="data">
        /// The array containing the bytes to hash.
        /// </param>
        ///
        /// <param name="offset">
        /// The offset into the array.
        /// </param>
        ///
        /// <param name="count">
        /// The count of bytes to hash.
        /// </param>
        [SecuritySafeCritical]
        internal void HashCore(byte[] data, int offset, int count)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if ((offset < 0) || (offset >= data.Length))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if ((count < 0) || (count > (data.Length - offset)))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            Validator.ThrowInvalidIfNull(_cryptoHashHandle, "CryptoHashNotInitialized");

            UnsafeNativeMethods.CryptHashDataWrapper(
                _cryptoHashHandle, data, offset, count);
        }

        /// <summary>
        /// Finalizes the hash.
        /// </summary>
        ///
        /// <returns>
        /// Returns the hash value.
        /// </returns>
        [SecuritySafeCritical]
        internal byte[] HashFinal()
        {
            return UnsafeNativeMethods.GetHashParameterWrapper(_cryptoHashHandle, HashParameter.HashValue);
        }
    }
}
