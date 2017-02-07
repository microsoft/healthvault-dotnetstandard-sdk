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

        double _value;

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
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "ExerciseSampleOneValueToStringFormat"),
                    OffsetInSeconds.ToString(),
                    Value.ToString());
        }
    }
}
