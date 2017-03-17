// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Web.Certificate
{
    /// <summary>
    /// Blob to pass to CAPI.
    /// </summary>
    /// <remarks>
    /// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/seccrypto/security/cryptoapi_blob.asp
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal sealed class CryptoApiBlob : IDisposable
    {
        #region Private variables
        private int cbData;
        private IntPtr pbData;
        #endregion

        #region Contructors

        /// <summary>
        /// Create a null blob
        /// </summary>
        internal CryptoApiBlob()
        {
            this.pbData = IntPtr.Zero;
        }

        /// <summary>
        /// Create a new blob from the given data.
        /// </summary>
        /// <exception cref="ArgumentNullException">If data is null or of zero length.</exception>
        [SecurityCritical]
        internal CryptoApiBlob(byte[] data)
        {
            Validator.ThrowIfArgumentNull(data, nameof(data), Resources.CryptoApiBlobNotNullData);

            this.AllocateBlob(data.Length);
            Marshal.Copy(data, 0, this.pbData, data.Length);
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the size of the contained blob
        /// </summary>
        internal int DataSize
        {
            get
            {
                Debug.Assert(this.cbData >= 0, "Crypto blob data not initialized");
                return this.cbData;
            }
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Allocate space for the blob.
        /// </summary>
        /// <remarks>
        /// Will also free the blob if it was already allocated.
        /// </remarks>
        /// <param name="size">Size of the blob to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException">If size is less than zero.</exception>
        /// <exception cref="CryptographicException">If the blob could not be allocated.</exception>
        [SecurityCritical]
        internal void AllocateBlob(int size)
        {
            Debug.Assert(this.cbData >= 0, "Crypto blob data not initialized");
            Debug.Assert(
                    (this.pbData == IntPtr.Zero && this.cbData == 0) ||
                    (this.pbData != IntPtr.Zero && this.cbData != 0), "Crypto blob data not initialized");

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), Resources.CryptoApiBlobSizeGreaterThanZero);
            }

            // allocate the new memory block
            IntPtr newMemory = IntPtr.Zero;
            if (size > 0)
            {
                newMemory = Marshal.AllocHGlobal(size);
                if (newMemory == IntPtr.Zero)
                {
                    throw new CryptographicException(Resources.CryptoApiBlobUnableToAllocateBlob);
                }
            }

            // if that succeeds then replace the old one
            IntPtr oldMemory = this.pbData;
            this.pbData = newMemory;
            this.cbData = size;

            // then release the old memory
            if (oldMemory != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(oldMemory);
            }
        }

        /// <summary>
        /// Clear the blob, releasing held memory if necessary.
        /// </summary>
        [SecurityCritical]
        internal void ClearBlob()
        {
            if (this.pbData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.pbData);
            }

            this.pbData = IntPtr.Zero;
            this.cbData = 0;
        }

        /// <summary>
        /// Create a byte array for this blob.
        /// </summary>
        [SecurityCritical]
        internal byte[] GetBytes()
        {
            Debug.Assert(this.cbData >= 0, "Crypto blob data not initialized");
            Debug.Assert(
                    (this.pbData == IntPtr.Zero && this.cbData == 0) ||
                    (this.pbData != IntPtr.Zero && this.cbData != 0), "Crypto blob data not initialized");

            if (this.pbData == IntPtr.Zero)
            {
                return null;
            }

            byte[] bytes = new byte[this.cbData];
            Marshal.Copy(this.pbData, bytes, 0, this.cbData);
            return bytes;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Clean up after the blob
        /// </summary>
        /// <param name="disposing">true if called from Dispose, false if from the finalizer</param>
        [SecuritySafeCritical]
        private void Dispose(bool disposing)
        {
            if (this.pbData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(this.pbData);
                this.pbData = IntPtr.Zero;
            }

            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Clean up the blob
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CryptoApiBlob"/> class.
        /// Last resort blob cleanup
        /// </summary>
        ~CryptoApiBlob()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
