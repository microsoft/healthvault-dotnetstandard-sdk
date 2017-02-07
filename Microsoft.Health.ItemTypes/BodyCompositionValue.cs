// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// A single measurement of body composition.
    /// </summary>
    /// 
    public class BodyCompositionValue : HealthRecordItemData
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="BodyCompositionValue"/> class 
        /// with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The mass-value element is used to store mass values, and the percentage-value is 
        /// used to store precentages. An application should set one or the other. When both 
        /// values are available, they should be stored in separate instance. 
        /// </remarks>
        /// 
        public BodyCompositionValue()
        {
        }

        /// <summary>
        /// Populates this <see cref="BodyCompositionValue"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML to get the body composition value data from.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> is <b> null </b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            // mass-value (t:weigth-value)
            _massValue = XPathHelper.GetOptNavValue<WeightValue>(navigator, "mass-value");

            // percent-value (t:percentage)
            XPathNavigator percentValueNav = navigator.SelectSingleNode("percent-value");
            if (percentValueNav != null)
            {
                _percentValue = percentValueNav.ValueAsDouble;
            }
        }

        /// <summary>
        /// Writes the body composition value data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the body composition value item.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the body composition value data to.
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            // <body-composition-value>
            writer.WriteStartElement(nodeName);

            // mass-value
            XmlWriterHelper.WriteOpt<WeightValue>(
                writer,
                "mass-value",
                _massValue);

            // percent-value
            XmlWriterHelper.WriteOptDouble(
                writer,
                "percent-value",
                _percentValue);

            // </body-composition-value>
            writer.WriteEndElement();

        }

        /// <summary>
        /// Gets or sets a body composition measurement stored as a mass.
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: 45 Kg, 15 lbs. 
        /// </remarks>
        /// 
        public WeightValue MassValue
        {
            get { return _massValue; }
            set { _massValue = value; }
        }
        private WeightValue _massValue;

        /// <summary>
        /// Gets or sets a body composition measurement stored as a percentage.  
        /// </summary>
        /// 
        /// <remarks>
        /// Examples: 0.37 (37%), 0.075 (7.5%). 
        /// </remarks>
        /// 
        public double? PercentValue
        {
            get { return _percentValue; }
            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    (value > 1.0) || (value < 0.0),
                    "PercentValue",
                    "BodyCompositionValuePercentValueOutOfRange");
                _percentValue = value;
            }
        }
        private double? _percentValue;

        /// <summary>
        /// Gets a string representation of BodyCompositionValue. 
        /// </summary> 
        /// 
        /// <returns>
        /// A string representation of the BodyCompositionValue.
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = String.Empty;
            if (_massValue != null && _percentValue != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "BodyCompositionValueToStringFormatMassAndPercent"),
                        _massValue.ToString(),
                        _percentValue * 100);
            }
            else if (_massValue != null)
            {
                result = _massValue.ToString();
            }
            else if (_percentValue != null)
            {
                result = 
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "Percent"),
                        _percentValue * 100);
            }
            return result.ToString();
        }
    }
}
