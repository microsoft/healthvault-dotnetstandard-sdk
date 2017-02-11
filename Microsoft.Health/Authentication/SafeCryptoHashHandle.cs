using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Health.Authentication
{
    /// <summary>
    /// SafeHandle implementation for crypto hash handles.
    /// </summary>
    [SecurityCritical]
    internal class SafeCryptoHashHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityCritical]
        private SafeCryptoHashHandle()
            : base(true)
        {
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
            return UnsafeNativeMethods.CryptDestroyHash(base.handle);
        }
    }
}

