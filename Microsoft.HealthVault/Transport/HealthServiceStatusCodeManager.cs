// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Helper factory class to help cast any integer status code
    /// to an appropriate HealthServiceStatusCode enum value.
    /// </summary>
    ///
    internal static class HealthServiceStatusCodeManager
    {
        /// <summary>
        /// Helper factory method to help cast any integer status code
        /// to an appropriate HealthServiceStatusCode enum value.
        /// </summary>
        ///
        /// <param name="statusCodeId">
        /// the integer status code id
        /// to be converted  to a HealthServiceStatusCode enum value.
        /// </param>
        ///
        /// <returns>
        /// HealthVaultStatus code enum object appropriately initialized.
        /// </returns>
        ///
        internal static HealthServiceStatusCode GetStatusCode(int statusCodeId)
        {
            HealthServiceStatusCode statusCode = HealthServiceStatusCode.UnmappedError;

            // Update this when HealthServiceStatusCode enum gets new values
            if (statusCodeId < (int)HealthServiceStatusCode.Max)
            {
                statusCode = (HealthServiceStatusCode)statusCodeId;
            }

            return statusCode;
        }
    }
}
