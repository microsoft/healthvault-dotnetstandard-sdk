using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// Safe handle wrapping the Capi Native CryptoContextHandle.
    /// </summary>
    [SecurityCritical]
    class SafeCryptoContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        private SafeCryptoContextHandle()
            : base(true)
        {
        }

        /// <summary>
        /// Executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic 
        /// failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging 
        /// Assistant.
        /// </returns>
        [SecurityCritical]
        protected override bool ReleaseHandle()
        {
            // Here, we must obey all rules for constrained execution regions.
            return UnsafeNativeMethods.CryptReleaseContext(base.handle, 0);
        }
    }
}
