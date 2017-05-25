// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The usage intentions for items being retrieved.
    /// </summary>
    [Flags]
    public enum ItemRetrievalIntentions
    {
        /// <summary>
        /// None of the specifiable intentions are applicable.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Intentions for retrieved items are unspecified in the request.
        /// </summary>
        /// <remarks>
        /// When intentions are unspecified, HealthVault may
        /// infer a default intention such as "user view" based
        /// on the context of the request.
        /// </remarks>
        Unspecified = 0x1,

        /// <summary>
        /// Retrieved items are intended for immediate user view.
        /// </summary>
        View = 0x2,

        /// <summary>
        /// Retrieved items are intended for immediate user download.
        /// </summary>
        Download = 0x4,

        /// <summary>
        /// Retrieved items are intended for immediate transmission via the Direct protocol.
        /// </summary>
        Transmit = 0x8
    }
}