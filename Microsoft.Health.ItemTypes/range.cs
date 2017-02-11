// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a data range based on a generic type.
    /// </summary>
    /// 
    /// <remarks>
    /// A range consists of a minimum range value and a maximum range value of the specific
    /// generic type.
    /// </remarks>
    /// 
    /// <typeparam name="RangeType">
    /// The type of the minimum and maximum values for the range.
    /// </typeparam>
    /// 
    public abstract class Range<RangeType> : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the Range class with 
        /// default values.
        /// </summary>
        /// 
        public Range() : base()
        {
            _minRange = DefaultMinValue;
            _maxRange = DefaultMaxValue;
        }

        /// <summary>
        /// Constructs a new instance of the Range class with the 
        /// specified min and max range values.
        /// </summary>
        /// 
        /// <param name="minRange">
        /// The minimum value for the range.
        /// </param>
        /// 
        /// <param name="maxRange">
        /// The maximum value for the range.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="minRange"/> or <paramref name="maxRange"/> would cause the value to
        /// be "unset".
        /// </exception>
        /// 
        public Range(RangeType minRange, RangeType maxRange)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        /// <summary> 
        /// Populates the data for the range from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the range.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator minRangeNav =
                navigator.SelectSingleNode("minimum-range");

            _minRange = ReadRangeValue(minRangeNav);

            XPathNavigator maxRangeNav =
                navigator.SelectSingleNode("maximum-range");

            _maxRange = ReadRangeValue(maxRangeNav);
        }

        /// <summary>
        /// Reads the value from the specified <see cref="XPathNavigator"/> as a 
        /// <typeparamref name="RangeType"/>.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The <see cref="XPathNavigator"/> to read the value from.
        /// </param>
        /// 
        /// <returns>
        /// The value as a <typeparamref name="RangeType"/>.
        /// </returns>
        /// 
        /// <remarks>
        /// Derived classes must override and read the value from the XML and convert it to the
        /// type for the range.
        /// </remarks>
        /// 
        protected abstract RangeType ReadRangeValue(XPathNavigator navigator);

        /// <summary> 
        /// Writes the range data to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer element for the range data.
        /// </param>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the range data to.
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

            WriteRangeValue("minimum-range", MinRange, writer);
            WriteRangeValue("maximum-range", MaxRange, writer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the specified range value to the specified writer with the specified node name.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the node to use when writing the value.
        /// </param>
        /// 
        /// <param name="value">
        /// The value to be written.
        /// </param>
        /// 
        /// <param name="writer">
        /// The writer to write the value element to.
        /// </param>
        /// 
        /// <remarks>
        /// Derived classes must override to provide the implementation of writing the value to
        /// the XML.
        /// </remarks>
        /// 
        protected abstract void WriteRangeValue(string nodeName, RangeType value, XmlWriter writer);

        /// <summary>
        /// Initializes the minimum range value to its default value.
        /// </summary>
        /// 
        /// <returns>
        /// The default value for the minimum range value.
        /// </returns>
        /// 
        protected abstract RangeType DefaultMinValue { get; }

        /// <summary>
        /// Gets the maximum range value to its default value.
        /// </summary>
        /// 
        /// <returns>
        /// The default value for the maximum range value.
        /// </returns>
        /// 
        protected abstract RangeType DefaultMaxValue { get; }


        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        /// 
        /// <value>
        /// A value of <typeparamref name="RangeType"/> that represents the minimum value of the range.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> would cause the value to be "unset".
        /// </exception>
        /// 
        public RangeType MinRange
        {
            get { return _minRange; }
            set 
            {
                VerifyRangeValue(value);
                _minRange = value; 
            }
        }
        private RangeType _minRange;

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        /// 
        /// <value>
        /// A value of <typeparamref name="RangeType"/> that represents the maximum value of the range.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> would cause the value to be "unset".
        /// </exception>
        /// 
        public RangeType MaxRange
        {
            get { return _maxRange; }
            set 
            {
                VerifyRangeValue(value);
                _maxRange = value; 
            }
        }
        private RangeType _maxRange;

        /// <summary>
        /// Verifies that the specified range value is an appropriate value for the range.
        /// </summary>
        /// 
        /// <param name="value">
        /// The value to verify.
        /// </param>
        /// 
        /// <remarks>
        /// Since both the maximum and minimum values for the range must be set, this method is used
        /// to verify that the value is not "unset". For example, the value is not <b>null</b> for
        /// a reference type. Derived classes should override and throw an 
        /// <see cref="ArgumentException"/> or appropriate derived exception if the 
        /// <paramref name="value"/> would cause the range value to be "unset".
        /// The default implementation does nothing.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> would cause the value to be "unset".
        /// </exception>
        /// 
        protected virtual void VerifyRangeValue(RangeType value)
        {
        }
    }
}
