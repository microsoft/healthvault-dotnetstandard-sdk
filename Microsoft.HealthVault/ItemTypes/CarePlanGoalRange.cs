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
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A range of values indicating a status for a measurement.
    /// </summary>
    ///
    public class CarePlanGoalRange : ItemBase
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
                if (value < 0)
                {
                    throw new ArgumentException(Resources.CarePlanGoalRangeStatusIndicatorNegative, nameof(StatusIndicator));
                }

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
            string rangeValue = string.Empty;

            if (_minimumValue != null &&
                _maximumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.CarePlanGoalRangeFormatMinMax,
                        _minimumValue,
                        _maximumValue);
            }
            else if (_minimumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.CarePlanGoalRangeFormatMinOnly,
                        _minimumValue);
            }
            else if (_maximumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.CarePlanGoalRangeFormatMaxOnly,
                        _maximumValue);
            }

            return
                string.Format(
                        CultureInfo.CurrentUICulture,
                        Resources.CarePlanGoalRangeFormat,
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
