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

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a length value and display. This class is abstract.
    /// </summary>
    /// 
    /// <remarks>
    /// HealthVault measurements are composed of the value and 
    /// a display value. All values are stored in a base unit for that type
    /// of measurement. An application can take a
    /// value using any scale the application chooses and can store the user- 
    /// entered value as the display value, but the measurement value must be 
    /// converted to the appropriate base unit to be stored in HealthVault.
    /// This abstract base class defines an interface from which all measurements 
    /// derive. The <see cref="Value"/> property's type varies according 
    /// to the derived class definition and affects the base unit for that
    /// type. See the documentation for the derived class for more information
    /// on the base unit type for that measurement.
    /// </remarks>
    /// 
    public abstract class Measurement<MeasurementType> : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the Measurement class 
        /// with no value.
        /// </summary>
        /// 
        public Measurement()
        {
        }

        /// <summary>
        /// Creates a new instance of the Measurement class with 
        /// the specified value.
        /// </summary>
        /// 
        /// <param name="value">
        /// An instance of MeasurementType representing the value.
        /// </param>
        /// 
        public Measurement(MeasurementType value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a new instance of the Measurement class with 
        /// the specified value and optional display value.
        /// </summary>
        /// 
        /// <param name="value">
        /// An instance of MeasurementType representing the value.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the measurement. This should contain the
        /// exact measurement as entered by the user even if it uses a 
        /// unit of measure other than the base unit of measure for the 
        /// type. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public Measurement(MeasurementType value, DisplayValue displayValue)
        {
            this.Value = value;
            this.DisplayValue = displayValue;
        }

        /// <summary> 
        /// Populates the data for the measurement from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the measurement.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            ParseValueXml(navigator);

            XPathNavigator displayNav = navigator.SelectSingleNode("display");
            if (displayNav != null)
            {
                _displayValue = new DisplayValue();
                _displayValue.ParseXml(displayNav);
            }
        }

        /// <summary>
        /// Parses the measurement specific value from the XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML node representing the measurement.
        /// </param>
        /// 
        protected abstract void ParseValueXml(XPathNavigator navigator);

        /// <summary> 
        /// Writes the measurement to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the measurement.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the measurement to.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            WriteValueXml(writer);

            if (_displayValue != null)
            {
                _displayValue.WriteXml("display", writer);
            }
            writer.WriteEndElement();
        }

        /// <summary> 
        /// Writes the measurement value to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the measurement value to.
        /// </param>
        /// 
        protected abstract void WriteValueXml(XmlWriter writer);

        /// <summary>
        /// Gets or sets the value of the measurement.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of MeasurementType representing the value.
        /// </returns>
        /// 
        /// <remarks>
        /// The value of the measurement must be within the tolerances for the
        /// specific type of measurement and must be in the base unit of 
        /// measure. See the derived classes documentation for the
        /// <see cref="AssertMeasurementValue"/> method to determine which 
        /// exceptions can be thrown.
        /// </remarks>
        /// 
        public MeasurementType Value
        {
            get { return _value; }
            set 
            {
                AssertMeasurementValue(value);
                _value = value; 
            }
        }
        private MeasurementType _value;

        /// <summary>
        /// Verifies the value is in the appropriate base unit of measure and
        /// is a legal value for the type.
        /// </summary>
        /// 
        /// <param name="value">
        /// The value to be verified.
        /// </param>
        /// 
        /// <remarks>
        /// Derived class must override this method and ensure that value uses
        /// the correct base unit of measurement and is a legal value for the
        /// type. If not, an appropriate exception should be thrown.
        /// </remarks>
        /// 
        protected abstract void AssertMeasurementValue(MeasurementType value);



        /// <summary>
        /// Gets or sets the display value of the HealthVault dictionary item.
        /// </summary>
        /// 
        /// <returns>
        /// An instance of <see cref="DisplayValue"/>.
        /// </returns>
        /// 
        public DisplayValue DisplayValue
        {
            get { return _displayValue; }
            set { _displayValue = value; }
        }
        private DisplayValue _displayValue;

        /// <summary>
        /// Retrieves a string representation of the measurement using the 
        /// <see cref="DisplayValue"/> property.
        /// </summary>
        /// 
        /// <returns>
        /// The measurement as a string including units if available.
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = null;
            if (_displayValue != null)
            {
                result = _displayValue.ToString();
            }
            else
            {
                result = GetValueString(_value);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the string representation of the value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The measurement value to get the string representation of.
        /// </param>
        /// 
        /// <returns>
        /// A string representing the measurement value.
        /// </returns>
        /// 
        protected abstract string GetValueString(MeasurementType value);
    }
}
