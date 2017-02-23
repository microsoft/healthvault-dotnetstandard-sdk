// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a target glucose zone.
    /// </summary>
    ///
    public class TargetGlucoseZone : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZone"/> class with default
        /// values.
        /// </summary>
        ///
        public TargetGlucoseZone()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZone"/> class
        /// with the specified absolute glucose value boundaries and name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the zone.
        /// </param>
        ///
        /// <param name="lowerBoundaryAbsoluteGlucose">
        /// The glucose value in millimoles per liter (mmol/L) for the lower
        /// boundary of the zone.
        /// </param>
        ///
        /// <param name="upperBoundaryAbsoluteGlucose">
        /// The glucose value in millimoles per liter (mmol/L) for the upper
        /// boundary of the zone.
        /// </param>
        ///
        public TargetGlucoseZone(
            string name,
            BloodGlucoseMeasurement lowerBoundaryAbsoluteGlucose,
            BloodGlucoseMeasurement upperBoundaryAbsoluteGlucose)
        {
            this.name = name;
            this.lowAbsolute = lowerBoundaryAbsoluteGlucose;
            this.upperAbsolute = upperBoundaryAbsoluteGlucose;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZone"/> class
        /// with the specified relative glucose value boundaries and name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the zone.
        /// </param>
        ///
        /// <param name="lowerBoundaryRelativeGlucose">
        /// The lower glucose boundary as a percentage of the person's
        /// maximum blood glucose level.
        /// </param>
        ///
        /// <param name="upperBoundaryRelativeGlucose">
        /// The upper glucose boundary as a percentage of the person's
        /// maximum blood glucose level.
        /// </param>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="lowerBoundaryRelativeGlucose"/> or
        /// <paramref name="upperBoundaryRelativeGlucose"/> parameter
        /// is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public TargetGlucoseZone(
            string name,
            double lowerBoundaryRelativeGlucose,
            double upperBoundaryRelativeGlucose)
        {
            this.name = name;
            this.RelativeLowerBoundary = lowerBoundaryRelativeGlucose;
            this.RelativeUpperBoundary = upperBoundaryRelativeGlucose;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the glucose zone information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is null.
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
                    lowNav.SelectSingleNode("absolute-glucose");
                if (absoluteNav != null)
                {
                    this.lowAbsolute = new BloodGlucoseMeasurement();
                    this.lowAbsolute.ParseXml(absoluteNav);
                }
                else
                {
                    XPathNavigator relativeNav =
                        lowNav.SelectSingleNode("percent-max-glucose");
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
                    upperNav.SelectSingleNode("absolute-glucose");
                if (absoluteNav != null)
                {
                    this.upperAbsolute = new BloodGlucoseMeasurement();
                    this.upperAbsolute.ParseXml(absoluteNav);
                }
                else
                {
                    XPathNavigator relativeNav =
                        upperNav.SelectSingleNode("percent-max-glucose");
                    if (relativeNav != null)
                    {
                        this.upperRelative = relativeNav.ValueAsDouble;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the XML representation of the glucose zone into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the heart rate zone.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the glucose zone should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is null or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is null.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="RelativeLowerBoundary"/>, <see cref="AbsoluteLowerBoundary"/>,
        /// <see cref="RelativeUpperBoundary"/>, or <see cref="AbsoluteUpperBoundary"/>
        /// parameter is not set.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIf(
                this.lowAbsolute == null && this.lowRelative == null,
                "GlucoseZoneNoLowerBoundary");

            Validator.ThrowSerializationIf(
                this.upperAbsolute == null && this.upperRelative == null,
                "GlucoseZoneNoUpperBoundary");

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(this.name))
            {
                writer.WriteAttributeString("name", this.name);
            }

            writer.WriteStartElement("lower-bound");
            if (this.lowAbsolute != null)
            {
                this.lowAbsolute.WriteXml("absolute-glucose", writer);
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-glucose",
                    this.lowRelative.Value.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();

            writer.WriteStartElement("upper-bound");
            if (this.upperAbsolute != null)
            {
                this.upperAbsolute.WriteXml("absolute-glucose", writer);
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-glucose",
                    this.upperRelative.Value.ToString(CultureInfo.InvariantCulture));
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name for the blood glucose zone.
        /// </summary>
        ///
        /// <value>
        /// A string representing the name.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the name should not be stored.
        /// </remarks>
        ///
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string name;

        /// <summary>
        /// Gets or sets the lower boundary of the blood glucose zone as a
        /// percentage of the person's maximum blood glucose level.
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
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public double? RelativeLowerBoundary
        {
            get { return this.lowRelative; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && (value < 0.0 || value > 1.0),
                    "RelativeLowerBoundary",
                    "GlucoseZoneRelativeBoundaryRange");
                this.lowRelative = value;
            }
        }

        private double? lowRelative;

        /// <summary>
        /// Gets or sets the lower boundary of the blood glucose zone as a
        /// specific blood glucose level.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="BloodGlucoseMeasurement"/> instance representing the
        /// boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeLowerBoundary"/> or
        /// <see cref="AbsoluteLowerBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteLowerBoundary"/> is stored.
        /// </remarks>
        ///
        public BloodGlucoseMeasurement AbsoluteLowerBoundary
        {
            get { return this.lowAbsolute; }
            set { this.lowAbsolute = value; }
        }

        private BloodGlucoseMeasurement lowAbsolute;

        /// <summary>
        /// Gets or sets the upper boundary of the blood glucose zone as a
        /// percentage of the person's maximum blood glucose level.
        /// </summary>
        ///
        /// <value>
        /// A number representing the boundary.
        /// </value>
        ///
        /// <remarks>
        /// Only <see cref="RelativeUpperBoundary"/> or
        /// <see cref="AbsoluteUpperBoundary"/> may be stored. If both are
        /// set, only <see cref="AbsoluteUpperBoundary"/> will be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 0.0 or greater than 1.0.
        /// </exception>
        ///
        public double? RelativeUpperBoundary
        {
            get { return this.upperRelative; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value != null && (value < 0.0 || value > 1.0),
                    "RelativeUpperBoundary",
                    "GlucoseZoneRelativeBoundaryRange");
                this.upperRelative = value;
            }
        }

        private double? upperRelative;

        /// <summary>
        /// Gets or sets the upper boundary of the blood glucose zone as a
        /// specific blood glucose level.
        /// </summary>
        ///
        /// <remarks>
        /// Only <see cref="RelativeUpperBoundary"/> or
        /// <see cref="AbsoluteUpperBoundary"/> can be stored. If both are
        /// set, only <see cref="AbsoluteUpperBoundary"/> is stored.
        /// </remarks>
        ///
        public BloodGlucoseMeasurement AbsoluteUpperBoundary
        {
            get { return this.upperAbsolute; }
            set { this.upperAbsolute = value; }
        }

        private BloodGlucoseMeasurement upperAbsolute;
    }
}
