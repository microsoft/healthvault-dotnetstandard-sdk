// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// A linear conversion of the form x' = mx + b.
    /// </summary>
    internal class LinearItemTypePropertyConversion : IItemTypePropertyConversion
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LinearItemTypePropertyConversion"/> class.
        /// </summary>
        ///
        /// <param name="multiplier">
        /// The multiplier to use in the linear conversion.
        /// </param>
        ///
        /// <param name="offset">
        /// The offset to use in the linear conversion.
        /// </param>
        public LinearItemTypePropertyConversion(double multiplier, double offset)
        {
            Multiplier = multiplier;
            Offset = offset;
        }

        /// <summary>
        /// The value by which to multiply the original value;
        /// the 'm' in the equation x' = mx + b.
        /// </summary>
        public double Multiplier { get; private set; }

        /// <summary>
        /// The offset to add in the linear conversion;
        /// the 'b' in the equation x' = mx + b.
        /// </summary>
        public double Offset { get; private set; }

        /// <summary>
        /// Performs a linear conversion of the form
        /// returnValue = Multiplier * value + offset.
        /// </summary>
        ///
        /// <param name="value">
        /// The value to be converted.
        /// </param>
        ///
        /// <returns>
        /// The result of the linear conversion.
        /// </returns>
        public double Convert(double value)
        {
            return (value * Multiplier) + Offset;
        }

        /// <summary>
        /// This method converts the Conversion xml to the
        /// <see cref="LinearItemTypePropertyConversion"/> object.
        /// </summary>
        public static LinearItemTypePropertyConversion CreateFromXml(XPathNavigator linearConversionNav)
        {
            double multiplier = linearConversionNav.SelectSingleNode("multiplier").ValueAsDouble;
            double offset = linearConversionNav.SelectSingleNode("offset").ValueAsDouble;

            return new LinearItemTypePropertyConversion(multiplier, offset);
        }
    }
}
