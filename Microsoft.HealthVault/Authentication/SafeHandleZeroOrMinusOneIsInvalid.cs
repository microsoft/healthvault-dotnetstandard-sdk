using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.HealthVault.Authentication
{
    /// <summary>
    /// SafeHandle that considers zero or negative one to be invalid.
    /// </summary>
    /// <remarks>
    /// Created to match the official one (see https://referencesource.microsoft.com/mscorlib/R/b62492e85f3730d7.html)
    /// Equivalent to the official one that checks for a null value, just slower (see https://referencesource.microsoft.com/mscorlib/R/fd586ac3c2a2bed1.html)
    /// </remarks>
    public abstract class SafeHandleZeroOrMinusOneIsInvalid : SafeHandle
    {
        public SafeHandleZeroOrMinusOneIsInvalid(bool ownsHandle) : base(IntPtr.Zero, ownsHandle)
        {
        }

        public override bool IsInvalid
        {
            [SecurityCritical]
            get { return this.handle == IntPtr.Zero || this.handle == new IntPtr(-1); }
        }
    }
}