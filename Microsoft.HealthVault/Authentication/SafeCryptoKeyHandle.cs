using System;
using System.Security;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// SafeHandle implemetation for crypto key handles.
    /// </summary>
    [SecurityCritical]
    internal class SafeCryptoKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        private SafeCryptoKeyHandle()
            : base(true)
        {
        }

        [SecurityCritical]
        private SafeCryptoKeyHandle(IntPtr key)
            : base(true)
        {
            base.SetHandle(key);
        }

        /// <summary>
        /// Executes the code required to free the handle.
        /// </summary>
        ///
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event
        /// of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed
        /// Managed Debugging Assistant.
        /// </returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            return UnsafeNativeMethods.CryptDestroyKey(base.handle);
        }

        internal static SafeCryptoKeyHandle ZeroHandle
        {
            [SecurityCritical]
            get
            {
                SafeCryptoKeyHandle handle = new SafeCryptoKeyHandle(IntPtr.Zero);
                return handle;
            }
        }
    }
}
