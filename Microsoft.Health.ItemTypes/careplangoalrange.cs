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
    /// A range of values indicating a status for a measurement.
    /// </summary>
    ///
    public class CarePlanGoalRange : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoalRange"/> class with default values.
        /// </summary>
        ///
        public CarePlanGoalRange()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoalRange"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// The larger the statusIndicator value, the farther the range is from the target. Multiple 
        /// ranges can have the same statusIndicator. For instance, a range just above the target could have 
        /// statusIndicator equal to one as well as the range just below the target.
        /// </remarks>
        /// 
        /// <param name="statusIndicator">
        /// Status indicator. For ex: '0' indicates the range is the target range.
        /// </param>
        ///
        public CarePlanGoalRange(int statusIndicator)
        {
            StatusIndicator = statusIndicator;
        }
        
        /// <summary>
        /// Populates this <see cref="CarePlanGoalRange"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the CarePlanGoalRange data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _statusIndicator = XPathHelper.GetOptNavValueAsInt(navigator, "status-indicator").Value;
            _minimumValue = XPathHelper.GetOptNavValueAsDouble(navigator, "minimum-value");
            _maximumValue = XPathHelper.GetOptNavValueAsDouble(navigator, "maximum-value");
        }
        
        /// <summary>
        /// Writes the XML representation of the CarePlanGoalRange into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlanGoalRange should be
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
            Validator.ThrowIfStringNullOrEmpty(nodeName, "WriteXmlEmptyNodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement("goal-range");
            {
                writer.WriteElementString("status-indicator", _statusIndicator.ToString(CultureInfo.InvariantCulture));
                XmlWriterHelper.WriteOptDouble(writer, "minimum-value", _minimumValue);
                XmlWriterHelper.WriteOptDouble(writer, "maximum-value", _maximumValue);
            }

            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets status indicator. For ex: '0' indicates the range is the target range.
        /// </summary>
        /// 
        /// <remarks>
        /// The further away from the target range the greater the status-indicator should be. Multiple ranges can have the same status indicator. For example, a range above the target could have the same status indicator as a range below the target.
        /// </remarks>
        ///
        public int StatusIndicator
        {
            get
            {
                return _statusIndicator;
            }
            
            set
            {
                Validator.ThrowArgumentExceptionIf(
                    value < 0,
                    "StatusIndicator",
                    "CarePlanGoalRangeStatusIndicatorNegative");

                _statusIndicator = value;
            }
        }
        
        private int _statusIndicator;
        
        /// <summary>
        /// Gets or sets minimum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        /// For ranges greater than a specified value with no maximum, specify a minimum-value but no maximum-value.
        /// </remarks>
        ///
        public double? MinimumValue
        {
            get
            {
                return _minimumValue;
            }
            
            set
            {
                _minimumValue = value;
            }
        }
        
        private double? _minimumValue;
        
        /// <summary>
        /// Gets or sets maximum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        /// For ranges less than a specified value with no minimum, specify a maximum-value but no minimum-value.
        /// </remarks>
        ///
        public double? MaximumValue
        {
            get
            {
                return _maximumValue;
            }
            
            set
            {
                _maximumValue = value;
            }
        }
        
        private double? _maximumValue;
        
        /// <summary>
        /// Gets a string representation of the CarePlanGoalRange.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the CarePlanGoalRange.
        /// </returns>
        ///
        public override string ToString()
        {
            string rangeValue = String.Empty;

            if (_minimumValue != null &&
                _maximumValue != null)
            {
                rangeValue =
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMinMax"),
                        _minimumValue,
                        _maximumValue);
            }
            else if (_minimumValue != null)
            {
                rangeValue =
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMinOnly"),
                        _minimumValue);
            }
            else if (_maximumValue != null)
            {
                rangeValue =
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMaxOnly"),
                        _maximumValue);
            }

            return
                String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormat"),
                        rangeValue,
                        _statusIndicator);
        }

        /// <summary>
        /// Returns true if the value is in the range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if the value is in range. </returns>
        public bool IsInRange(double value)
        {
            if (!_minimumValue.HasValue &&
                !_maximumValue.HasValue)
            {
                return false;
            }

            if (_minimumValue.HasValue &&
                value < _minimumValue.Value)
            {
                return false;
            }

            if (_maximumValue.HasValue &&
                value > _maximumValue.Value)
            {
                return false;
            }

            return true;
        }
    }
}
