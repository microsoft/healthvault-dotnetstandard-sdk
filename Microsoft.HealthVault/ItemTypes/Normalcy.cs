// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Indicates how a value compares to normal values of the same type.
    /// </summary>
    ///
    public enum Normalcy
    {
        /// <summary>
        /// The server returned a value that is unknown to this client.
        /// </summary>
        ///
        /// <remarks>
        /// This value can occur when an update to the server has been made
        /// to add a new value but the client has not been updated. All data
        /// will persist correctly but may not be programmatically accessible.
        /// </remarks>
        ///
        Unknown = 0,

        /// <summary>
        /// The value is well below the norm when compared to values of
        /// the same type.
        /// </summary>
        ///
        WellBelowNormal = 1,

        /// <summary>
        /// The value is below the norm when compared to values of
        /// the same type.
        /// </summary>
        ///
        BelowNormal = 2,

        /// <summary>
        /// The value is normal when compared to values of
        /// the same type.
        /// </summary>
        ///
        Normal = 3,

        /// <summary>
        /// The value is above the norm when compared to values of
        /// the same type.
        /// </summary>
        ///
        AboveNormal = 4,

        /// <summary>
        /// The value is well above the norm when compared to values of
        /// the same type.
        /// </summary>
        ///
        WellAboveNormal = 5
    }
}
