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
            _name = name;
            _lowAbsolute = lowerBoundaryHeartRate;
            _upperAbsolute = upperBoundaryHeartRate;
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
            _name = name;
            _lowRelative = lowerBoundaryPercentage;
            _upperRelative = upperBoundaryPercentage;
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

            string name = navigator.GetAttribute("name", String.Empty);
            if (name.Length != 0)
            {
                _name = name;
            }

            XPathNavigator lowNav =
                navigator.SelectSingleNode("lower-bound");
            if (lowNav != null)
            {
                XPathNavigator absoluteNav =
                    lowNav.SelectSingleNode("absolute-heartrate");
                if (absoluteNav != null)
                {
                    _lowAbsolute = absoluteNav.ValueAsInt;
                }
                else
                {
                    XPathNavigator relativeNav =
                        lowNav.SelectSingleNode("percent-max-heartrate");
                    if (relativeNav != null)
                    {
                        _lowRelative = relativeNav.ValueAsDouble;
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
                    _upperAbsolute = absoluteNav.ValueAsInt;
                }
                else
                {
                    XPathNavigator relativeNav =
                        upperNav.SelectSingleNode("percent-max-heartrate");
                    if (relativeNav != null)
                    {
                        _upperRelative = relativeNav.ValueAsDouble;
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
                _lowAbsolute == null && _lowRelative == null,
                "HeartRateZoneNoLowerBoundary");

            Validator.ThrowSerializationIf(
                _upperAbsolute == null && _upperRelative == null,
                "HeartRateZoneNoUpperBoundary");

            writer.WriteStartElement(nodeName);

            if (!String.IsNullOrEmpty(_name))
            {
                writer.WriteAttributeString("name", _name);
            }

            writer.WriteStartElement("lower-bound");
            if (_lowAbsolute != null)
            {
                writer.WriteElementString(
                    "absolute-heartrate", 
                    _lowAbsolute.Value.ToString(CultureInfo.InvariantCulture));    
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-heartrate",
                    _lowRelative.Value.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();

            writer.WriteStartElement("upper-bound");
            if (_upperAbsolute != null)
            {
                writer.WriteElementString(
                    "absolute-heartrate",
                    _upperAbsolute.Value.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteElementString(
                    "percent-max-heartrate",
                    _upperRelative.Value.ToString(CultureInfo.InvariantCulture));
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
            get { return _name; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }
        private string _name;

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
            get { return _lowRelative; }
            set { _lowRelative = value; }
        }
        private double? _lowRelative;

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
            get { return _lowAbsolute; }
            set { _lowAbsolute = value; }
        }
        private int? _lowAbsolute;

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
            get { return _upperRelative; }
            set { _upperRelative = value; }
        }
        private double? _upperRelative;

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
            get { return _upperAbsolute; }
            set { _upperAbsolute = value; }
        }
        private int? _upperAbsolute;

    }

}
