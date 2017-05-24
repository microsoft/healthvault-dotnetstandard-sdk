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
    /// Represents a relative rating for attributes such as emotion or activity.
    /// </summary>
    ///
    public enum RelativeRating
    {
        /// <summary>
        /// The relative rating is not known.
        /// </summary>
        ///
        None = 0,

        /// <summary>
        /// The rating is very low.
        /// </summary>
        ///
        VeryLow = 1,

        /// <summary>
        /// The rating is low.
        /// </summary>
        ///
        Low = 2,

        /// <summary>
        /// The rating is moderate.
        /// </summary>
        ///
        Moderate = 3,

        /// <summary>
        /// The rating is high.
        /// </summary>
        ///
        High = 4,

        /// <summary>
        /// The rating is very high.
        /// </summary>
        ///
        VeryHigh = 5
    }
}
