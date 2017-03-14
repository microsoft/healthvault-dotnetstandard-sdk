// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A measurement using specific units.
    /// </summary>
    ///
    public class StructuredMeasurement : ItemBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="StructuredMeasurement"/>
        /// class with default values.
        /// </summary>
        ///
        public StructuredMeasurement()
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="StructuredMeasurement"/>
        /// class with mandatory parameters.
        /// </summary>
        ///
        /// <param name="value">
        /// The value of the measurement.
        /// </param>
        ///
        /// <param name="units">
        /// The units of the measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="units"/> is <b>null</b>.
        /// </exception>
        ///
        public StructuredMeasurement(double value, CodableValue units)
        {
            this.Value = value;
            this.Units = units;
        }

        /// <summary>
        /// Populates this <see cref="StructuredMeasurement"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the structured measurement data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The first node in <paramref name="navigator"/> is not
        /// a structured measurement node.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.value = navigator.SelectSingleNode("value").ValueAsDouble;

            this.units = new CodableValue();
            this.units.ParseXml(navigator.SelectSingleNode("units"));
        }

        /// <summary>
        /// Writes the structured measurement data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the node to write XML.
        /// </param>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the concern data to.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b> null </b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b> null </b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Units"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.units, "StructuredMeasurementUnitsNotSet");

            // <structured-measurement>
            writer.WriteStartElement(nodeName);

            // value
            writer.WriteElementString("value", XmlConvert.ToString(this.value));

            // units
            this.units.WriteXml("units", writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets unit of measure for the value.
        /// </summary>
        ///
        /// <remarks>
        /// A list of vocabularies that can be used to code the units may be found
        /// in the measurement-unit-sets vocabulary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Units
        {
            get { return this.units; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Units", "StructuredMeasurementUnitsNotSet");
                this.units = value;
            }
        }

        private CodableValue units;

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        ///
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        private double value;

        /// <summary>
        /// Gets a string representation of the structured measurement item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the structured measurement item.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    ResourceRetriever.GetResourceString(
                        "StructuredMeasurementToStringFormat"),
                    this.value.ToString(),
                    this.units.ToString());
        }
    }
}
