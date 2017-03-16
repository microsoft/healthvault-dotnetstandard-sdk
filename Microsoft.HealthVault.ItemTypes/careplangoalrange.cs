// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

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
            this.StatusIndicator = statusIndicator;
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

            this.statusIndicator = XPathHelper.GetOptNavValueAsInt(navigator, "status-indicator").Value;
            this.minimumValue = XPathHelper.GetOptNavValueAsDouble(navigator, "minimum-value");
            this.maximumValue = XPathHelper.GetOptNavValueAsDouble(navigator, "maximum-value");
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
                writer.WriteElementString("status-indicator", this.statusIndicator.ToString(CultureInfo.InvariantCulture));
                XmlWriterHelper.WriteOptDouble(writer, "minimum-value", this.minimumValue);
                XmlWriterHelper.WriteOptDouble(writer, "maximum-value", this.maximumValue);
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
                return this.statusIndicator;
            }

            set
            {
                Validator.ThrowArgumentExceptionIf(
                    value < 0,
                    "StatusIndicator",
                    "CarePlanGoalRangeStatusIndicatorNegative");

                this.statusIndicator = value;
            }
        }

        private int statusIndicator;

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
                return this.minimumValue;
            }

            set
            {
                this.minimumValue = value;
            }
        }

        private double? minimumValue;

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
                return this.maximumValue;
            }

            set
            {
                this.maximumValue = value;
            }
        }

        private double? maximumValue;

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

            if (this.minimumValue != null &&
                this.maximumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMinMax"),
                        this.minimumValue,
                        this.maximumValue);
            }
            else if (this.minimumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMinOnly"),
                        this.minimumValue);
            }
            else if (this.maximumValue != null)
            {
                rangeValue =
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormatMaxOnly"),
                        this.maximumValue);
            }

            return
                string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalRangeFormat"),
                        rangeValue,
                        this.statusIndicator);
        }

        /// <summary>
        /// Returns true if the value is in the range.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if the value is in range. </returns>
        public bool IsInRange(double value)
        {
            if (!this.minimumValue.HasValue &&
                !this.maximumValue.HasValue)
            {
                return false;
            }

            if (this.minimumValue.HasValue &&
                value < this.minimumValue.Value)
            {
                return false;
            }

            if (this.maximumValue.HasValue &&
                value > this.maximumValue.Value)
            {
                return false;
            }

            return true;
        }
    }
}
