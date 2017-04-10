// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Exceptions
{
    /// <summary>
    /// The exception is raised when an authenticated connection is required, but was called from a connection
    /// which has not yet been authenticated.
    /// </summary>
    public sealed class HealthAuthorizedConnectionRequiredException : HealthVaultException
    {
        /// <summary>
        /// Creates an instance of the <see cref="HealthAuthorizedConnectionRequiredException"/>.
        /// </summary>
        /// 
        ///
        /// <remarks>
        /// This constructor is internal to the SDK. Application developers
        /// using the SDK should catch instances of this exception instead of
        /// throwing new exceptions of this type.
        /// </remarks>
        ///
        internal HealthAuthorizedConnectionRequiredException() 
            : base(Resources.AuthenticatedConnectionRequired)
        {
        }
    }
}
