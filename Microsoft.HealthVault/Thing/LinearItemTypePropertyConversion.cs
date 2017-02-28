// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml.XPath;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// A linear conversion of the form x' = mx + b.
    /// </summary>
    public class LinearItemTypePropertyConversion : IItemTypePropertyConversion
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
            this.Multiplier = multiplier;
            this.Offset = offset;
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
            return (value * this.Multiplier) + this.Offset;
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
