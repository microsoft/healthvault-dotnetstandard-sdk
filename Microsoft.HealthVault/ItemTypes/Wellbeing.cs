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
    /// Describes the general wellbeing of a person from sick to healthy.
    /// </summary>
    ///
    public enum Wellbeing
    {
        /// <summary>
        /// The wellbeing of the person is unknown.
        /// </summary>
        ///
        None = 0,

        /// <summary>
        /// The person is sick.
        /// </summary>
        ///
        Sick = 1,

        /// <summary>
        /// The person is not functioning at a normal level.
        /// </summary>
        ///
        Impaired = 2,

        /// <summary>
        /// The person is functioning at a normal level but might still have
        /// symptoms.
        /// </summary>
        ///
        Able = 3,

        /// <summary>
        /// The person is functioning at a normal level without any
        /// symptoms.
        /// </summary>
        ///
        Healthy = 4,

        /// <summary>
        /// The person is functioning beyond their normal level.
        /// </summary>
        ///
        Vigorous = 5
    }
}
