using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace Microsoft.HealthVault.Authentication
{
    [Flags]
    internal enum CryptoAcquireContextFlags : uint
    {
        None = 0,
        VerifyContext = 0xF0000000
    }

    [Flags]
    internal enum CryptImportKeyFlags : uint
    {
        CryptIpsecHmacKey = 0x0100,
    }

    internal enum ProviderType : uint
    {
        None = 0,
        RsaFull = 1,
        RsaAes = 0x18
    }

    internal enum AlgorithmId : uint
    {
        Aes128 = 0x660e,
        Aes192 = 0x660f,
        Aes256 = 0x6610,
        MD5 = 0x8003,
        None = 0,
        Sha1 = 0x8004,
        Sha256 = 0x800c,
        Sha384 = 0x800d,
        Sha512 = 0x800e,
        Hmac = 0x8009,
        Rc4 = 0x6801,
    }

    internal enum HashParameter : uint
    {
        None = 0,
        AlgorithmId = 1,
        HashValue = 2,
        HashSize = 4,
        HmacInfo = 5,
    }

    [StructLayout(
            System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct HMAC_INFO
    {
        public AlgorithmId HashAlgid;
        public System.IntPtr pbInnerString;
        public uint cbInnerString;
        public System.IntPtr pbOuterString;
        public uint cbOuterString;
    }

    internal class UnsafeNativeMethods
    {
        /// <summary>
        /// Acquire the crypto context.
        /// </summary>
        /// 
        /// <param name="cryptoContext">
        /// The crypto context.
        /// </param>
        /// 
        /// <param name="container">
        /// The container.
        /// </param>
        /// 
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// 
        /// <param name="providerType">
        /// Type of the provider.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool CryptAcquireContext(
            out SafeCryptoContextHandle cryptoContext,
            String container,
            String provider,
            ProviderType providerType,
            CryptoAcquireContextFlags flags);

        /// <summary>
        /// Release the crypto context.
        /// </summary>
        /// 
        /// <param name="prov">
        /// The provider handle.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags to pass to Capi.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32")]
        internal static extern bool CryptReleaseContext(IntPtr prov, int flags);

        /// <summary>
        /// Creates the hash handle.
        /// </summary>
        /// 
        /// <param name="cryptoContext">
        /// The crypto context.
        /// </param>
        /// 
        /// <param name="algorithmId">
        /// The algorithm id.
        /// </param>
        /// 
        /// <param name="key">
        /// The key.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <param name="hash">
        /// The hash
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32", SetLastError = true)]
        [SecurityCritical]
        internal static extern bool CryptCreateHash(
            SafeCryptoContextHandle cryptoContext,
            AlgorithmId algorithmId,
            SafeCryptoKeyHandle key,
            int flags,
            out SafeCryptoHashHandle hash);

        /// <summary>
        /// Destroys the crypto key.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key handle.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32", SetLastError = true)]
        internal static extern bool CryptDestroyKey(IntPtr key);

        /// <summary>
        /// Hashes the data.
        /// </summary>
        /// 
        /// <param name="hash">
        /// The hash handle.
        /// </param>
        /// 
        /// <param name="plainText">
        /// The plain text.
        /// </param>
        /// 
        /// <param name="length">
        /// The plain text length.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32", SetLastError = true)]
        [SecurityCritical]
        internal static extern bool CryptHashData(
            SafeCryptoHashHandle hash,
            IntPtr plainText,
            int length,
            int flags);

        /// <summary>
        /// Gets the Hash parameter.
        /// </summary>
        /// 
        /// <param name="hash">
        /// The hash handle.
        /// </param>
        /// 
        /// <param name="hashParameter">
        /// The hash parameter.
        /// </param>
        /// 
        /// <param name="hashValue">
        /// The hash value.
        /// </param>
        /// 
        /// <param name="length">
        /// The length of the value.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32", SetLastError = true)]
        [SecurityCritical]
        internal static extern bool CryptGetHashParam(
            SafeCryptoHashHandle hash,
            HashParameter hashParameter,
            [Out, MarshalAs(UnmanagedType.LPArray)] byte[] hashValue,
            [In, Out] ref int length,
            int flags);

        /// <summary>
        /// Sets the hash param.
        /// </summary>
        /// 
        /// <param name="hash">
        /// The hash handle.
        /// </param>
        /// 
        /// <param name="paramter">
        /// The paramter.
        /// </param>
        /// 
        /// <param name="hmacInfo">
        /// The hmac info.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32.dll", SetLastError = true)]
        [SecurityCritical]
        internal static extern bool CryptSetHashParam(
            SafeCryptoHashHandle hash,
            HashParameter paramter,
            [In, Out] ref HMAC_INFO hmacInfo,
            uint flags
        );

        /// <summary>
        /// Gets the hash parameter.  This method wraps the underlying
        /// api to obtain the length of the result and appropriately
        /// size a result byte buffer.
        /// </summary>
        /// 
        /// <param name="hashHandle">
        /// The hash handle.
        /// </param>
        /// 
        /// <param name="hashParameter">
        /// The hash parameter.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [SecurityCritical]
        internal static byte[] GetHashParameterWrapper(
            SafeCryptoHashHandle hashHandle,
            HashParameter hashParameter)
        {
            int hashLength = 0;
            if (!UnsafeNativeMethods.CryptGetHashParam(
                hashHandle, hashParameter, null, ref hashLength, 0))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            byte[] hash = new byte[hashLength];
            if (!UnsafeNativeMethods.CryptGetHashParam(
                hashHandle, hashParameter, hash, ref hashLength, 0))
            {
                throw new CryptographicException(Marshal.GetLastWin32Error());
            }

            return hash;
        }

        /// <summary>
        /// Import a hash key to the hash.
        /// </summary>
        /// 
        /// <param name="cryptoContext">
        /// The crypto context.
        /// </param>
        /// 
        /// <param name="key">
        /// The key data.
        /// </param>
        /// 
        /// <param name="keyLength">
        /// The key data length.
        /// </param>
        /// 
        /// <param name="pubKey">
        /// The pub key.
        /// </param>
        /// 
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// 
        /// <param name="keyHandle">
        /// The key.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool CryptImportKey(
            SafeCryptoContextHandle cryptoContext,
            byte[] key,
            uint keyLength,
            SafeCryptoKeyHandle pubKey,
            CryptImportKeyFlags flags,
            out SafeCryptoKeyHandle keyHandle);

        // Import key requires a particular key format.  The following 
        // byte array formats the key blob header for a plain text key
        // suitable for an hmac.
        private static byte[] blobHeader = {
            0x08,0x02,0x00,0x00,0x02,0x66,0x00,0x00 }; // plaintext blob header 

        /// <summary>
        /// Gets the key BLOB suitable for calling CryptoImportKey for
        /// an hmac.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key.
        /// </param>
        /// 
        /// <returns>
        /// The formatted key blob.
        /// </returns>
        internal static byte[] GetKeyBlob(byte[] key)
        {
            byte[] blob = new byte[
                blobHeader.Length
                + sizeof(uint)  // key size length
                + key.Length];

            int offset = 0;

            Buffer.BlockCopy(blobHeader, 0, blob, 0, blobHeader.Length);
            offset += blobHeader.Length;

            // Set key length in blob
            Buffer.BlockCopy(
                BitConverter.GetBytes(key.Length),
                0,
                blob,
                offset,
                sizeof(uint));
            offset += sizeof(uint);

            // Set the key
            Buffer.BlockCopy(
                key,
                0,
                blob,
                offset,
                key.Length);

            return blob;
        }

        /// <summary>
        /// Destroys the underlying crypto hash.
        /// </summary>
        /// 
        /// <param name="hash">
        /// The hash handle.
        /// </param>
        /// 
        /// <returns>
        /// <c>true</c> if successful, otherwise <c>false</c>.
        /// </returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("advapi32", SetLastError = true)]
        internal static extern bool CryptDestroyHash(IntPtr hash);

        /// <summary>
        /// Wraps the CryptHashData method to marshal the correct
        /// byte pointer.
        /// </summary>
        /// 
        /// <param name="hash">
        /// The hash handle.
        /// </param>
        /// 
        /// <param name="data">
        /// The data to hash.
        /// </param>
        /// 
        /// <param name="offset">
        /// The offset into the data.
        /// </param>
        /// 
        /// <param name="count">
        /// The count of bytes to hash.
        /// </param>
        [SecurityCritical]
        internal static void CryptHashDataWrapper(
            SafeCryptoHashHandle hash,
            byte[] data,
            int offset,
            int count)
        {
            // We could marshal the data as UnmanagedType.LPArray but we would 
            // have to copy the array so that the first byte is at pos 0.  This
            // way we can take correct the region of the array in situ.
            unsafe
            {
                fixed (byte* b = &data[offset])
                {
                    IntPtr intPtr = new IntPtr((void*)b);

                    if (!UnsafeNativeMethods.CryptHashData(
                        hash, intPtr, count, 0))
                    {
                        throw new CryptographicException(Marshal.GetLastWin32Error());
                    }
                }
            }
        }
    }
}
