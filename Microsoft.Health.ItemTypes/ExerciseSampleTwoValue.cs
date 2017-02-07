// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.ItemTypes
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

        double _offsetInSeconds;

        /// <summary>
        /// Gets or sets the offset in seconds of this data sample from the beginning of the sample set. 
        /// </summary>
        /// 
        /// <value>
        /// The offset in seconds.
        /// </value>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="OffsetInSeconds"/> value is less than zero.
        /// </exception>
        public double OffsetInSeconds
        {
            get { return _offsetInSeconds; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value < 0,
                    "OffsetInSeconds",
                    "OffsetMustBePositive");
                _offsetInSeconds = value;
            }
        }

        double _value1;

        /// <summary>
        /// Gets or sets the first data value stored in the sample.
        /// </summary>
        public double Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

        double _value2;

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
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "ExerciseSampleTwoValueToStringFormat"),
                    OffsetInSeconds.ToString(),
                    Value1.ToString(),
                    Value2.ToString());
        }
    }
}
