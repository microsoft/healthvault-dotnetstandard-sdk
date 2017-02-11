using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.Health.Authentication
{
    /// <summary>
    /// A Hash implementation which delegates to the native Capi library.
    /// </summary>
    internal class CryptoServiceProviderHash : AbstractCryptoServiceProviderHash
    {
        internal CryptoServiceProviderHash(
            String provider,
            ProviderType providerType,
            AlgorithmId algorithm)
            : base(provider, providerType, algorithm)
        {
        }

        [SecurityCritical]
        internal override SafeCryptoHashHandle NewHashHandle()
        {
            SafeCryptoHashHandle hashHandle = null;
            if (!UnsafeNativeMethods.CryptCreateHash(
                CryptoContext, AlgorithmId, SafeCryptoKeyHandle.ZeroHandle, 0, out hashHandle))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            return hashHandle;
        }
    }
}
