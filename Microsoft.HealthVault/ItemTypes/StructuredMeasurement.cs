// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
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
            Value = value;
            Units = units;
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

            _value = navigator.SelectSingleNode("value").ValueAsDouble;

            _units = new CodableValue();
            _units.ParseXml(navigator.SelectSingleNode("units"));
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Units"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_units, Resources.StructuredMeasurementUnitsNotSet);

            // <structured-measurement>
            writer.WriteStartElement(nodeName);

            // value
            writer.WriteElementString("value", XmlConvert.ToString(_value));

            // units
            _units.WriteXml("units", writer);

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
            get { return _units; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Units), Resources.StructuredMeasurementUnitsNotSet);
                _units = value;
            }
        }

        private CodableValue _units;

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        ///
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private double _value;

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
                    Resources.StructuredMeasurementToStringFormat,
                    _value.ToString(),
                    _units.ToString());
        }
    }
}
