// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.HealthVault.Authentication
{
    internal sealed class CryptoUtil
    {
        private CryptoUtil()
        {
        }

        internal static void GetRandomBytes(byte[] buffer)
        {
            // TODO: RNGCryptoServiceProvider is not available in .NetStandard until 2.0
            // using (RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider())
            // {
            //     Gen.GetBytes(buffer);
            // }
        }

        internal static int GetInt32(byte[] aData, int offset)
        {
            return BitConverter.ToInt32(aData, offset);
        }

        internal static int SetInt32(byte[] aData, int offset, int value)
        {
            byte[] bushort = BitConverter.GetBytes(value);
            Buffer.BlockCopy(bushort, 0, aData, offset, bushort.Length);
            return bushort.Length;
        }

        /// <summary>
        /// Calls Dispose() given a non-null object and sets it to null.
        /// Does nothing if the object is null.
        /// </summary>
        ///
        /// <param name="handle">
        /// Reference to a SafeHande.
        /// </param>
        [SecurityCritical]
        internal static void DisposeHandle<T>(ref T handle)
            where T : SafeHandle
        {
            if (handle != null)
            {
                handle.Dispose();
                handle = null;
            }
        }
    }
}
