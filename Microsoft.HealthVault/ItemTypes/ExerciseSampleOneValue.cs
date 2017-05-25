// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A exercise sample that contains one data value.
    /// </summary>
    public class ExerciseSampleOneValue
    {
        /// <summary>
        /// Create an instance of the <see cref="ExerciseSampleOneValue"/> type with the specified values.
        /// </summary>
        /// <param name="offsetInSeconds">The offset of this sample from the beginning of the sample set.</param>
        /// <param name="value">The data value of this sample.</param>
        public ExerciseSampleOneValue(double offsetInSeconds, double value)
        {
            OffsetInSeconds = offsetInSeconds;
            Value = value;
        }

        private double _offsetInSeconds;

        /// <summary>
        /// Gets or sets the offset in seconds of this data sample from the beginning of the sample set.
        /// </summary>
        ///
        /// <value>
        /// The offset in seconds.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> value is less than zero.
        /// </exception>
        public double OffsetInSeconds
        {
            get { return _offsetInSeconds; }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(OffsetInSeconds), Resources.OffsetMustBePositive);
                }

                _offsetInSeconds = value;
            }
        }

        private double _value;

        /// <summary>
        /// Gets or sets the data value stored in the sample.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Gets a string representation of the sample.
        /// </summary>
        public override string ToString()
        {
            return
                string.Format(
                    Resources.ExerciseSampleOneValueToStringFormat,
                    OffsetInSeconds.ToString(),
                    Value.ToString());
        }
    }
}
