// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents the volume of a gas, liquid, etc. and a display value
    /// associated with the measurement.
    /// </summary>
    ///
    /// <remarks>
    /// In HealthVault, volume measurements have values and display values.
    /// All values are stored in a base unit of liters (L).
    /// An application can take a volume value using any scale the application
    /// chooses and can store the user-entered value as the display value,
    /// but the volumn value must be converted to liters to be stored in HealthVault.
    /// </remarks>
    ///
    public class VolumeMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VolumeMeasurement"/> class
        /// with empty values.
        /// </summary>
        ///
        public VolumeMeasurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VolumeMeasurement"/> class
        /// with the specified value in liters.
        /// </summary>
        ///
        /// <param name="liters">
        /// The volume in liters.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="liters"/> is less than zero.
        /// </exception>
        ///
        public VolumeMeasurement(double liters)
            : base(liters)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VolumeMeasurement"/> class with
        /// the specified value in liters and optional display value.
        /// </summary>
        ///
        /// <param name="liters">
        /// The volume in liters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The display value of the volume. This should contain the
        /// exact volume as entered by the user even if it uses some
        /// other unit of measure besides liters. The display value
        /// <see cref="DisplayValue.Units"/> and
        /// <see cref="DisplayValue.UnitsCode"/>
        /// represents the unit of measure for the user-entered value.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="liters"/> is less than zero.
        /// </exception>
        ///
        public VolumeMeasurement(double liters, DisplayValue displayValue)
            : base(liters, displayValue)
        {
        }

        /// <summary>
        /// Verifies the value is a legal volume value in liters.
        /// </summary>
        ///
        /// <param name="value">
        /// The volume measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than zero.
        /// </exception>
        ///
        protected override void AssertMeasurementValue(double value)
        {
            if (value < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), Resources.VolumeNotPositive);
            }
        }

        /// <summary>
        /// Populates the data for the volume from the XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML node representing the volume.
        /// </param>
        ///
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("liters").ValueAsDouble;
        }

        /// <summary>
        /// Writes the volume to the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the volume to.
        /// </param>
        ///
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "liters",
                XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the volume in the base units.
        /// </summary>
        ///
        /// <returns>
        /// The volume as a string in the base units.
        /// </returns>
        ///
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}
