// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's
    /// aerobic profile at a single point in time.
    /// </summary>
    ///
    public class AerobicProfile : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AerobicProfile"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public AerobicProfile()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AerobicProfile"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the aerobic profile was take.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public AerobicProfile(HealthServiceDateTime when)
            : base(TypeId)
        {
            When = when;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("7b2ea78c-4b78-4f75-a6a7-5396fe38b09a");

        /// <summary>
        /// Populates this <see cref="AerobicProfile"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the aerobic profile data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an aerobic-session node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator aerobicProfileNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "aerobic-profile");

            Validator.ThrowInvalidIfNull(aerobicProfileNav, Resources.AerobicProfileUnexpectedNode);

            _when = new HealthServiceDateTime();
            _when.ParseXml(aerobicProfileNav.SelectSingleNode("when"));

            XPathNavigator maxHrNav =
                aerobicProfileNav.SelectSingleNode("max-heartrate");

            if (maxHrNav != null)
            {
                _maxHr = maxHrNav.ValueAsInt;
            }

            XPathNavigator restingHrNav =
                aerobicProfileNav.SelectSingleNode("resting-heartrate");

            if (restingHrNav != null)
            {
                _restingHr = restingHrNav.ValueAsInt;
            }

            XPathNavigator atNav =
                aerobicProfileNav.SelectSingleNode("anaerobic-threshold");

            if (atNav != null)
            {
                _anaerobicThreshold = atNav.ValueAsInt;
            }

            XPathNavigator vo2MaxNav =
                aerobicProfileNav.SelectSingleNode("VO2-max");

            if (vo2MaxNav != null)
            {
                XPathNavigator vo2AbsNav =
                    vo2MaxNav.SelectSingleNode("absolute");
                if (vo2AbsNav != null)
                {
                    _vo2Absolute = vo2AbsNav.ValueAsDouble;
                }

                XPathNavigator vo2RelNav =
                    vo2MaxNav.SelectSingleNode("relative");
                if (vo2RelNav != null)
                {
                    _vo2Relative = vo2RelNav.ValueAsDouble;
                }
            }

            XPathNodeIterator zoneGroupIterator =
                aerobicProfileNav.Select("heartrate-zone-group");

            foreach (XPathNavigator groupNav in zoneGroupIterator)
            {
                HeartRateZoneGroup zoneGroup = new HeartRateZoneGroup();
                zoneGroup.ParseXml(groupNav);
                _zoneGroups.Add(zoneGroup);
            }
        }

        /// <summary>
        /// Writes the aerobic profile data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the aerobic profile data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            // <aerobic-profile>
            writer.WriteStartElement("aerobic-profile");

            // <when>
            _when.WriteXml("when", writer);

            // <max-heartrate>
            if (_maxHr != null)
            {
                writer.WriteElementString(
                    "max-heartrate",
                    _maxHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <resting-heartrate>
            if (_restingHr != null)
            {
                writer.WriteElementString(
                    "resting-heartrate",
                    _restingHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <anaerobic-threshold>
            if (_anaerobicThreshold != null)
            {
                writer.WriteElementString(
                    "anaerobic-threshold",
                    _anaerobicThreshold.Value.ToString(
                        CultureInfo.InvariantCulture));
            }

            if (_vo2Absolute != null || _vo2Relative != null)
            {
                writer.WriteStartElement("VO2-max");

                if (_vo2Absolute != null)
                {
                    XmlWriterHelper.WriteOptDouble(
                        writer,
                        "absolute",
                        _vo2Absolute.Value);
                }

                if (_vo2Relative != null)
                {
                    XmlWriterHelper.WriteOptDouble(
                        writer,
                        "relative",
                        _vo2Relative.Value);
                }

                writer.WriteEndElement();
            }

            foreach (HeartRateZoneGroup group in _zoneGroups)
            {
                group.WriteXml("heartrate-zone-group", writer);
            }

            // </aerobic-profile>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the aerobic profile was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's maximum heart rate.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the rate.
        /// </value>
        ///
        /// <remarks>
        /// If the maximum heart rate is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        ///
        public int? MaximumHeartRate
        {
            get { return _maxHr; }
            set { _maxHr = value; }
        }

        private int? _maxHr;

        /// <summary>
        /// Gets or sets the person's resting heart rate.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the rate.
        /// </value>
        ///
        /// <remarks>
        /// If the resting heart rate is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        ///
        public int? RestingHeartRate
        {
            get { return _restingHr; }
            set { _restingHr = value; }
        }

        private int? _restingHr;

        /// <summary>
        /// Gets or sets the person's anaerobic threshold in beats per minute
        /// (BPM).
        /// </summary>
        ///
        /// <value>
        /// An integer representing the threshold.
        /// </value>
        ///
        /// <remarks>
        /// If the anaerobic threshold is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        ///
        public int? AnaerobicThreshold
        {
            get { return _anaerobicThreshold; }
            set { _anaerobicThreshold = value; }
        }

        private int? _anaerobicThreshold;

        /// <summary>
        /// Gets or sets the relative VO2 max for the person in mL/kg/min.
        /// </summary>
        ///
        /// <value>
        /// A number representing the relative VO2 max.
        /// </value>
        ///
        /// <remarks>
        /// If the relative VO2 max is not known, the value can be set to <b>null</b>.
        /// </remarks>
        ///
        public double? RelativeVO2Max
        {
            get { return _vo2Relative; }
            set { _vo2Relative = value; }
        }

        private double? _vo2Relative;

        /// <summary>
        /// Gets or sets the absolute V02 max for the person in mL/min.
        /// </summary>
        ///
        /// <value>
        /// A number representing the absolute VO2 max.
        /// </value>
        ///
        /// <remarks>
        /// If the absolute V02 max is not known, the value can be set to <b>null</b>.
        /// </remarks>
        ///
        public double? AbsoluteVO2Max
        {
            get { return _vo2Absolute; }
            set { _vo2Absolute = value; }
        }

        private double? _vo2Absolute;

        /// <summary>
        /// Gets the target heart rate zone groups.
        /// </summary>
        ///
        /// <value>
        /// A collection of zone groups.
        /// </value>
        ///
        /// <remarks>
        /// Target heart rate zones are grouped to allow different sets of
        /// target zones based on activity, exercise theories, and so on.
        /// To add a group of heart rate zones, call the Add method on the
        /// returned collection.
        /// </remarks>
        ///
        public Collection<HeartRateZoneGroup> TargetHeartRateZoneGroups => _zoneGroups;

        private readonly Collection<HeartRateZoneGroup> _zoneGroups =
            new Collection<HeartRateZoneGroup>();

        /// <summary>
        /// Gets a string representation of the aerobic profile item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation for the aerobic profile item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(256);

            if (MaximumHeartRate != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileMaxHRFormat,
                    MaximumHeartRate);
            }

            if (RestingHeartRate != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileRestingHRFormat,
                    RestingHeartRate);
            }

            if (AnaerobicThreshold != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileAnaerobicThresholdFormat,
                    AnaerobicThreshold);
            }

            if (RelativeVO2Max != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileRelativeVO2MaxFormat,
                    RelativeVO2Max);
            }

            return result.ToString();
        }
    }
}
