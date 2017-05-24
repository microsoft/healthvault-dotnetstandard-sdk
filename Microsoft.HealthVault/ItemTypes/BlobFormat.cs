// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Determines how BLOB information will be returned when fetching a
    /// <see cref="ThingBase"/>.
    /// </summary>
    ///
    public enum BlobFormat
    {
        /// <summary>
        /// The requested BLOB format is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Only BLOB information is returned.
        /// </summary>
        ///
        Information,

        /// <summary>
        /// The BLOB information and data is returned inline with the response XML.
        /// </summary>
        ///
        Inline,

        /// <summary>
        /// Information about the BLOB sufficient to be able to retrieve the BLOB through the
        /// streaming interfaces.
        /// </summary>
        ///
        Streamed,

        /// <summary>
        /// The default format is Streamed.
        /// </summary>
        Default = Streamed
    }
}
