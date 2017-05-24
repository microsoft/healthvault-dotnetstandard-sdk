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
    /// A exercise sample that contains two data values.
    /// </summary>
    public class ExerciseSampleTwoValue
    {
        /// <summary>
        /// Create an instance of the <see cref="ExerciseSampleTwoValue"/> type with the specified values.
        /// </summary>
        /// <param name="offsetInSeconds">The offset of this sample from the beginning of the sample set.</param>
        /// <param name="value1">The first data value of this sample.</param>
        /// <param name="value2">The second data value of this sample.</param>
        public ExerciseSampleTwoValue(double offsetInSeconds, double value1, double value2)
        {
            OffsetInSeconds = offsetInSeconds;
            Value1 = value1;
            Value2 = value2;
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

        private double _value1;

        /// <summary>
        /// Gets or sets the first data value stored in the sample.
        /// </summary>
        public double Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

        private double _value2;

        /// <summary>
        /// Gets or sets the second data value stored in the sample.
        /// </summary>
        public double Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        /// <summary>
        /// Gets a string representation of the sample.
        /// </summary>
        public override string ToString()
        {
            return
                string.Format(
                    Resources.ExerciseSampleTwoValueToStringFormat,
                    OffsetInSeconds.ToString(),
                    Value1.ToString(),
                    Value2.ToString());
        }
    }
}
