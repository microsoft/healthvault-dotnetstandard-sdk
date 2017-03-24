// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
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
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/>
        /// method is called.
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
            this.When = when;
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

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(aerobicProfileNav.SelectSingleNode("when"));

            XPathNavigator maxHrNav =
                aerobicProfileNav.SelectSingleNode("max-heartrate");

            if (maxHrNav != null)
            {
                this.maxHr = maxHrNav.ValueAsInt;
            }

            XPathNavigator restingHrNav =
                aerobicProfileNav.SelectSingleNode("resting-heartrate");

            if (restingHrNav != null)
            {
                this.restingHr = restingHrNav.ValueAsInt;
            }

            XPathNavigator atNav =
                aerobicProfileNav.SelectSingleNode("anaerobic-threshold");

            if (atNav != null)
            {
                this.anaerobicThreshold = atNav.ValueAsInt;
            }

            XPathNavigator vo2MaxNav =
                aerobicProfileNav.SelectSingleNode("VO2-max");

            if (vo2MaxNav != null)
            {
                XPathNavigator vo2AbsNav =
                    vo2MaxNav.SelectSingleNode("absolute");
                if (vo2AbsNav != null)
                {
                    this.vo2Absolute = vo2AbsNav.ValueAsDouble;
                }

                XPathNavigator vo2RelNav =
                    vo2MaxNav.SelectSingleNode("relative");
                if (vo2RelNav != null)
                {
                    this.vo2Relative = vo2RelNav.ValueAsDouble;
                }
            }

            XPathNodeIterator zoneGroupIterator =
                aerobicProfileNav.Select("heartrate-zone-group");

            foreach (XPathNavigator groupNav in zoneGroupIterator)
            {
                HeartRateZoneGroup zoneGroup = new HeartRateZoneGroup();
                zoneGroup.ParseXml(groupNav);
                this.zoneGroups.Add(zoneGroup);
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
            this.when.WriteXml("when", writer);

            // <max-heartrate>
            if (this.maxHr != null)
            {
                writer.WriteElementString(
                    "max-heartrate",
                    this.maxHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <resting-heartrate>
            if (this.restingHr != null)
            {
                writer.WriteElementString(
                    "resting-heartrate",
                    this.restingHr.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <anaerobic-threshold>
            if (this.anaerobicThreshold != null)
            {
                writer.WriteElementString(
                    "anaerobic-threshold",
                    this.anaerobicThreshold.Value.ToString(
                        CultureInfo.InvariantCulture));
            }

            if (this.vo2Absolute != null || this.vo2Relative != null)
            {
                writer.WriteStartElement("VO2-max");

                if (this.vo2Absolute != null)
                {
                    XmlWriterHelper.WriteOptDouble(
                        writer,
                        "absolute",
                        this.vo2Absolute.Value);
                }

                if (this.vo2Relative != null)
                {
                    XmlWriterHelper.WriteOptDouble(
                        writer,
                        "relative",
                        this.vo2Relative.Value);
                }

                writer.WriteEndElement();
            }

            foreach (HeartRateZoneGroup group in this.zoneGroups)
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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

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
            get { return this.maxHr; }
            set { this.maxHr = value; }
        }

        private int? maxHr;

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
            get { return this.restingHr; }
            set { this.restingHr = value; }
        }

        private int? restingHr;

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
            get { return this.anaerobicThreshold; }
            set { this.anaerobicThreshold = value; }
        }

        private int? anaerobicThreshold;

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
            get { return this.vo2Relative; }
            set { this.vo2Relative = value; }
        }

        private double? vo2Relative;

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
            get { return this.vo2Absolute; }
            set { this.vo2Absolute = value; }
        }

        private double? vo2Absolute;

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
        public Collection<HeartRateZoneGroup> TargetHeartRateZoneGroups => this.zoneGroups;

        private readonly Collection<HeartRateZoneGroup> zoneGroups =
            new Collection<HeartRateZoneGroup>();

        /// <summary>
        /// Gets a string representation of the aerobic profile item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation fo the aerobic profile item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(256);

            if (this.MaximumHeartRate != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileMaxHRFormat,
                    this.MaximumHeartRate);
            }

            if (this.RestingHeartRate != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileRestingHRFormat,
                    this.RestingHeartRate);
            }

            if (this.AnaerobicThreshold != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileAnaerobicThresholdFormat,
                    this.AnaerobicThreshold);
            }

            if (this.RelativeVO2Max != null)
            {
                result.AppendFormat(
                    Resources.AerobicProfileRelativeVO2MaxFormat,
                    this.RelativeVO2Max);
            }

            return result.ToString();
        }
    }
}
