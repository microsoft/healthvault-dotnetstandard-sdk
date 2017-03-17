// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.HealthVault.Web.Certificate
{
    /// <summary>
    /// Utility class.
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Get the formatted string of the last error message
        /// </summary>
        [SecuritySafeCritical]
        internal static string GetLastErrorMessage()
        {
            return new Win32Exception(Marshal.GetLastWin32Error()).Message;
        }
    }
}
