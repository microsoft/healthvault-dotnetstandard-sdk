using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{

    /// <summary>
    /// An HMAC implementation which delegates to the native Capi libarary.
    /// </summary>
    internal class CryptoServiceProviderHmac : AbstractCryptoServiceProviderHash
    {
        private byte[] _key;

        [SecurityCritical]
        private SafeCryptoKeyHandle _keyHandle;

        [SecurityCritical]
        private SafeCryptoHashHandle _hashHandle;
        private HMAC_INFO _hmacInfo = new HMAC_INFO();

        public CryptoServiceProviderHmac(
            String provider,
            ProviderType providerType,
            AlgorithmId algorithm)
            : base(provider, providerType, algorithm)
        {
        }

        public CryptoServiceProviderHmac(
            byte[] key,
            String provider,
            ProviderType providerType,
            AlgorithmId algorithm)
            : this(provider, providerType, algorithm)
        {
            _key = key;
        }

        public byte[] Key
        {
            get { return (byte[])_key.Clone(); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Key cannot be null");
                }

                _key = value;
            }
        }

        [SecurityCritical]
        internal override SafeCryptoHashHandle NewHashHandle()
        {
            SafeCryptoHashHandle resultHashHandle;

            if (!UnsafeNativeMethods.CryptCreateHash(
                CryptoContext,
                AlgorithmId,
                SafeCryptoKeyHandle.ZeroHandle,
                0,
                out _hashHandle))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            UnsafeNativeMethods.CryptHashDataWrapper(
                _hashHandle, _key, 0, _key.Length);

            byte[] keyBlob = UnsafeNativeMethods.GetKeyBlob(_key);

            if (!UnsafeNativeMethods.CryptImportKey(
                CryptoContext,
                keyBlob,
                (uint)keyBlob.Length,
                SafeCryptoKeyHandle.ZeroHandle,
                CryptImportKeyFlags.CryptIpsecHmacKey,
                out _keyHandle))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            if (!UnsafeNativeMethods.CryptCreateHash(
                CryptoContext,
                AlgorithmId.Hmac,
                _keyHandle,
                0,
                out resultHashHandle))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            _hmacInfo.HashAlgid = AlgorithmId.Sha256;

            if (!UnsafeNativeMethods.CryptSetHashParam(
                resultHashHandle,
                HashParameter.HmacInfo,
                ref _hmacInfo,
                0))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            return resultHashHandle;
        }

        [SecurityCritical]
        internal override void Reset()
        {
            CryptoUtil.DisposeHandle(ref _keyHandle);
            CryptoUtil.DisposeHandle(ref _hashHandle);
            base.Reset();
        }
    }
}
