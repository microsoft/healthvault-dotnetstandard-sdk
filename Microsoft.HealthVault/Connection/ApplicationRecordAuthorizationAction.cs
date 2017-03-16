// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Connection
{
    /// <summary>
    /// Enumeration of the application record authorization action codes.
    /// </summary>
    ///
    public enum ApplicationRecordAuthorizationAction
    {
        /// <summary>
        /// The server returned a value that is not understood by this client.
        /// </summary>
        ///
        Unknown = 0,

        /// <summary>
        /// The application has never been authorized.
        /// </summary>
        ///
        AuthorizationRequired = 1,

        /// <summary>
        /// The application both requires and can do re-authorization.
        /// </summary>
        ///
        ReauthorizationRequired = 2,

        /// <summary>
        /// The application requires but cannot do re-authorization.
        /// </summary>
        ///
        ReauthorizationNotPossible = 3,

        /// <summary>
        /// There are no actions required.
        /// </summary>
        ///
        NoActionRequired = 4,

        /// <summary>
        /// The record location isn't supported by the application.
        /// </summary>
        RecordLocationNotSupported = 5
    }
}
