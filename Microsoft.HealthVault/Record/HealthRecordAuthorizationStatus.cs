// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.HealthVault.Record
{
    /// <summary>
    /// The record's current authorization status.
    /// </summary>
    /// <remarks>
    /// The members of HealthRecordAuthorizationStatus represent the current
    /// status of the application's authorization to the record.  Any status
    /// other than NoActionRequired, requires user intervention in HealthVault
    /// before the application may successfully access the record.
    /// </remarks>
    ///
    public enum HealthRecordAuthorizationStatus
    {
        /// <summary>
        /// An unknown state was returned from the server.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The record should be accessible to the application.
        /// </summary>
        NoActionRequired = 1,

        /// <summary>
        ///  The user must authorize this application.
        /// </summary>
        AuthorizationRequired = 2,

        /// <summary>
        /// It is not possible to reauthorize this application.
        /// </summary>
        ReauthorizationNotPossible = 3,

        /// <summary>
        /// The user must reauthorize this application.
        /// </summary>
        ReauthorizationRequired = 4
    }
}
