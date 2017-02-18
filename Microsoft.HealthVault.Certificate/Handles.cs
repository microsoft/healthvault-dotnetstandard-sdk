// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.HealthVault.Certificate
{
    /// <summary>
    /// Safe handle for a PCERT_CONTEXT
    /// </summary>
    [SecurityCritical]
    internal sealed class CertificateHandle : SafeHandle
    {
        /// <summary>
        /// Creates an instance of CertificateHandle
        /// </summary>
        private CertificateHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Checks whether handle is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero; }
        }

        ///	<summary>
        ///	Gets or sets whether to delete the certificate from the store when the handle is released.
        ///	</summary>
        public bool DeleteOnRelease
        {
            [SecurityCritical]
            get { return deleteOnRelease; }

            [SecurityCritical]
            set { deleteOnRelease = value; }
        }

        private bool deleteOnRelease;

        /// <summary>
        /// Releases the handle. Deletes certificate from store if deleteOnRelease is true.
        /// </summary>
        /// <returns></returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            if (deleteOnRelease)
            {
                NativeMethods.CertDeleteCertificateFromStore(handle);
            }

            return NativeMethods.CertFreeCertificateContext(handle);
        }
    }

    /// <summary>
    /// Safe handle for a HCERTSTORE
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    [SecurityCritical]
    internal sealed class CertificateStoreHandle : SafeHandle
    {

        /// <summary>
        /// Constructor for CertificateStoreHandle.
        /// </summary>
        private CertificateStoreHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Gets whether handle is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero; }
        }

        /// <summary>
        /// Releases the handle associated with the certificate store.
        /// </summary>
        /// <returns>True if CertCloseStore succeeds, false otherwise.</returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CertCloseStore(handle, 0);
        }
    }

    /// <summary>
    /// Safe handle for a HCRYPTPROV
    /// </summary>
    [SecurityCritical]
    [SuppressUnmanagedCodeSecurity]
    internal sealed class KeyContainerHandle : SafeHandle
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        private KeyContainerHandle()
            : base(IntPtr.Zero, true)
        {
            return;
        }

        /// <summary>
        /// Gets whether handle is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero; }
        }

        /// <summary>
        /// Releases the handle associated with crypto context.
        /// </summary>
        /// <returns>true of false indicating success of CryptReleaseContext.</returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CryptReleaseContext(handle, 0);
        }
    }

    /// <summary>
    /// Safe handle for a HCRYPTKEY
    /// </summary>
    [SecurityCritical]
    internal sealed class KeyHandle : SafeHandle
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        private KeyHandle()
            : base(IntPtr.Zero, true)
        {
        }

        /// <summary>
        /// Gets whether handle is invalid.
        /// </summary>
        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return handle == IntPtr.Zero; }
        }

        /// <summary>
        /// Releases 
        /// </summary>
        /// <returns>Releases the handle associated with the public key </returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return NativeMethods.CryptDestroyKey(handle);
        }
    }
}
