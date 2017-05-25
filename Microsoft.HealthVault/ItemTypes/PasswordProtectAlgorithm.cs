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
    /// Represents the algorithm used to protect a package of data with a password.
    /// </summary>
    ///
    internal enum PasswordProtectAlgorithm
    {
        /// <summary>
        /// The algorithm name returned from the server was not understood
        /// by this client.
        /// </summary>
        ///
        /// <remarks>
        /// This can happen if new algorithms are supported by the server but
        /// the client has not been updated.
        /// </remarks>
        ///
        Unknown = 0,

        /// <summary>
        /// No encryption was used to protect the package.
        /// </summary>
        ///
        None = 1,

        /// <summary>
        /// The package is encrypted using the HMAC-SHA1 pseudo-random and
        /// 3DES encryption functions.
        /// </summary>
        ///
        HmacSha13Des = 2,

        /// <summary>
        /// The package is encrypted using the HMAC-SHA256 and AES256
        /// encryption functions.
        /// </summary>
        ///
        HmacSha256Aes256 = 3
    }
}
