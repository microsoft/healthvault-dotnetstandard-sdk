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
    /// Represents a target heart rate zone.
    /// </summary>
    ///
    public class HeartRateZone : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZone"/> class with default values.
        /// </summary>
        ///
        public HeartRateZone()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZone"/> class with
        /// the specified zone name and absolute heart rate boundaries.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the zone.
        /// </param>
        ///
        /// <param name="lowerBoundaryHeartRate">
        /// The heart rate in Beats Per Minute (BPM) for the lower boundary
        /// of the zone.
        /// </param>
        ///
        /// <param name="upperBoundaryHeartRate">
        /// The heart rate in Beats Per Minute (BPM) for the upper boundary
        /// of the zone.
        /// </param>
        ///
        public HeartRateZone(
            string name,
            int lowerBoundaryHeartRate,
            int upperBoundaryHeartRate)
        {
            this.name = name;
            this.lowAbsolute = lowerBoundaryHeartRate;
            this.upperAbsolute = upperBoundaryHeartRate;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZone"/> class with
        /// the specified relative heart rate boundaries.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the zone.
        /// </param>
        ///
        /// <param name="lowerBoundaryPercentage">
        /// The heart rate as a percentage of max heart rate for the lower
        /// boundary of the zone.
        /// </param>
        ///
        /// <param name="upperBoundaryPercentage">
        /// The heart rate as a percentage of max heart rate for the upper
        /// boundary of the zone.
        /// </param>
        ///
        public HeartRateZone(
            string name,
            double lowerBoundaryPercentage,
            double upperBoundaryPercentage)
        {
            this.name = name;
            this.lowRelative = lowerBoundaryPercentage;
            this.upperRelative = upperBoundaryPercentage;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the heart rate zone information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            string name = navigator.GetAttribute("name", string.Empty);
            if (name.Length != 0)
            {
                this.name = name;
            }

            XPathNavigator lowNav =
                navigator.SelectSingleNode("lower-bound");
            if (lowNav != null)
            {
                XPathNavigator absoluteNav =
                    lowNav.SelectSingleNode("absolute-heartrate");
                if (absoluteNav != null)
                {
                    this.lowAbsolute = absoluteNav.ValueAsInt;
                }
                else
                {
                    XPathNavigator relativeNav =
                        lowNav.SelectSingleNode("percent-max-heartrate");
                    if (relativeNav != null)
                    {
                        this.lowRelative = relativeNav.ValueAsDouble;
                    }
                }
            }

            XPathNavigator upperNav =
                navigator.SelectSingleNode("upper-bound");
            if (upperNav != null)
            {
                XPathNavigator absoluteNav =
                    upperNav.SelectSingleNode("absolute-heartrate");
                if (absoluteNav != null)
                {
                    this.upperAbsolute = absoluteNav.ValueAsInt;
                }
                else
                {
                    XPathNavigator relativeNav =
                        upperNav.SelectSingleNode("percent-max-heartrate");
                    if (relativeNav != null)
                    {
                        this.upperRelative = relativeNav.ValueAsDouble;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the XML representation of the heart rate zone into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the heart rate zone.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the heart rate zone should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If the <see cref="RelativeLowerBoundary"/>,
        /// <see cref="AbsoluteLowerBoundary"/>,
        /// or <see cref="RelativeUpperBoundary"/>, or
        /// <see cref="AbsoluteUpperBoundary"/> property is not set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIf(
                this.lowAbsolute == null && this.lowRelative == null,
                "HeartRateZoneNoLowerBoundary");

            Validator.ThrowSerializationIf(
                this.upperAbsolute == null && this.upperRelative == null,
                "HeartRateZoneNoUpperBoundary");

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(this.name))
            {
                writer.WriteAttributeString("name", this.name);
            }

            writer.WriteStartElement("lower-bound");
            if (this.lowAbsolute != null)
            {
                writer.WriteElementString(
                    "absolute-heartrate",
                    this.lowAbsolute.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-heartrate",
                    this.lowRelative.Value.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();

            writer.WriteStartElement("upper-bound");
            if (this.upperAbsolute != null)
            {
                writer.WriteElementString(
                    "absolute-heartrate",
                    this.upperAbsolute.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-heartrate",
                    this.upperRelative.Value.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name for the heart rate zone.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the name should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                this.name = value;
            }
        }

        private string name;

        /// <summary>
        /// Gets or sets the lower boundary of the heart rate zone as a
        /// percentage of the person's maximum heart rate.
        /// </summary>
        ///
        /// <value>
        /// A number representing the boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeLowerBoundary"/> or
        /// <see cref="AbsoluteLowerBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteLowerBoundary"/> is stored.
        /// </remarks>
        ///
        public double? RelativeLowerBoundary
        {
            get { return this.lowRelative; }
            set { this.lowRelative = value; }
        }

        private double? lowRelative;

        /// <summary>
        /// Gets or sets the lower boundary of the heart rate zone as a
        /// specific heart rate.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeLowerBoundary"/> or
        /// <see cref="AbsoluteLowerBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteLowerBoundary"/> is stored.
        /// </remarks>
        ///
        public int? AbsoluteLowerBoundary
        {
            get { return this.lowAbsolute; }
            set { this.lowAbsolute = value; }
        }

        private int? lowAbsolute;

        /// <summary>
        /// Gets or sets the upper boundary of the heart rate zone as a
        /// percentage of the person's maximum heart rate.
        /// </summary>
        ///
        /// <value>
        /// A number representing the boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeUpperBoundary"/> or
        /// <see cref="AbsoluteUpperBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteUpperBoundary"/> is stored.
        /// </remarks>
        ///
        public double? RelativeUpperBoundary
        {
            get { return this.upperRelative; }
            set { this.upperRelative = value; }
        }

        private double? upperRelative;

        /// <summary>
        /// Gets or sets the upper boundary of the heart rate zone as a
        /// specific heart rate.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeUpperBoundary"/> or
        /// <see cref="AbsoluteUpperBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteUpperBoundary"/> is stored.
        /// </remarks>
        ///
        public int? AbsoluteUpperBoundary
        {
            get { return this.upperAbsolute; }
            set { this.upperAbsolute = value; }
        }

        private int? upperAbsolute;
    }
}
