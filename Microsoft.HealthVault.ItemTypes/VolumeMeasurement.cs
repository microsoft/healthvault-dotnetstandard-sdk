// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public VolumeMeasurement() : base()
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
        public VolumeMeasurement(double liters) : base(liters)
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
            Validator.ThrowArgumentOutOfRangeIf(value < 0.0, "value", "VolumeNotPositive");
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
