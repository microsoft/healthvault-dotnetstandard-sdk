// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Unit conversion representation.
    /// </summary>
    ///
    public class UnitConversion : ItemBase
    {
        /// <summary>
        /// Populates this <see cref="UnitConversion"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the UnitConversion data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.multiplier = XPathHelper.GetOptNavValueAsDouble(navigator, "multiplier");
            this.offset = XPathHelper.GetOptNavValueAsDouble(navigator, "offset");
        }

        /// <summary>
        /// Writes the XML representation of the UnitConversion into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the unit conversion.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the UnitConversion should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            writer.WriteStartElement("unit-conversion");

            XmlWriterHelper.WriteOptDouble(writer, "multiplier", this.multiplier);
            XmlWriterHelper.WriteOptDouble(writer, "offset", this.offset);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the multiplier.
        /// </summary>
        ///
        public double? Multiplier
        {
            get
            {
                return this.multiplier;
            }

            set
            {
                this.multiplier = value;
            }
        }

        private double? multiplier;

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        ///
        public double? Offset
        {
            get
            {
                return this.offset;
            }

            set
            {
                this.offset = value;
            }
        }

        private double? offset;

        /// <summary>
        /// Gets a string representation of the UnitConversion.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the UnitConversion.
        /// </returns>
        ///
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentUICulture,
                Resources.UnitConversionFormat,
                this.multiplier,
                this.offset);
        }

        /// <summary>
        /// Convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>the value using the selected conversion</returns>
        public double Convert(double value)
        {
            if (this.multiplier.HasValue)
            {
                value = value * this.multiplier.Value;
            }

            if (this.offset.HasValue)
            {
                value = value + this.offset.Value;
            }

            return value;
        }

        /// <summary>
        /// Reverse convert a value using this conversion.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>the value using the reverse of this conversion</returns>
        public double ReverseConvert(double value)
        {
            if (this.offset.HasValue)
            {
                value = value - this.offset.Value;
            }

            if (this.multiplier.HasValue)
            {
                value = value / this.multiplier.Value;
            }

            return value;
        }
    }
}
