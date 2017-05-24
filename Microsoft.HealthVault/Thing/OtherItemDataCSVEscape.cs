// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Information about a single escape entity from an OtherDataHelper object.
    /// </summary>
    /// <remarks>
    /// Escapes are stored using a "name=value" format in the other data text string.
    /// </remarks>
    internal class OtherItemDataCsvEscape : OtherItemDataCsvItem
    {
        /// <summary>
        /// Create an OtherDataEscape instance.
        /// </summary>
        /// <param name="name">The name of the escape value.</param>
        /// <param name="value">The value.</param>
        internal OtherItemDataCsvEscape(string name, string value)
        {
            Name = name;
            Value = value;
        }

        private string _name;

        /// <summary>
        /// Gets or sets the name of the escape.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If the name is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the name contains the "=" character.
        /// </exception>
        internal string Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.CSVNameNull);

                if (value.Contains("="))
                {
                    throw new ArgumentException(Resources.CSVNameInvalid, nameof(Name));
                }

                _name = value;
            }
        }

        private string _value;

        /// <summary>
        /// Gets or sets the value of the escape.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// If the value is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the value contains the "=" character.
        /// </exception>
        internal string Value
        {
            get { return _value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.CSVValueNull);

                if (value.Contains("="))
                {
                    throw new ArgumentException(Resources.CSVValueInvalid, nameof(Value));
                }

                _value = value;
            }
        }

        /// <summary>
        /// Gets the escape in "Name=value" format.
        /// </summary>
        internal string NameEqualsValue => _name + "=" + _value;
    }
}
