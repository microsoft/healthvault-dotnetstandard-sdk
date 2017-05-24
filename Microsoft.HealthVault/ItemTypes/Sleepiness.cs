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
    /// Provides values representing how awake a person felt during the day.
    /// </summary>
    ///
    public enum Sleepiness
    {
        /// <summary>
        /// The sleepiness state is unknown.
        /// </summary>
        ///
        /// <remarks>
        /// This is not a valid state and will cause an exception if used.
        /// </remarks>
        ///
        Unknown = 0,

        /// <summary>
        /// The person was very sleepy throughout the day.
        /// </summary>
        ///
        VerySleepy = 1,

        /// <summary>
        /// The person was somewhat tired throughout the day.
        /// </summary>
        ///
        Tired = 2,

        /// <summary>
        /// The person was fairly alert throughout the day.
        /// </summary>
        ///
        Alert = 3,

        /// <summary>
        /// The person was wide awake throughout the day.
        /// </summary>
        ///
        WideAwake = 4
    }
}
