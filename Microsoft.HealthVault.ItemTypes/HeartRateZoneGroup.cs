// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a grouping of heart rate zones by name.
    /// </summary>
    ///
    public class HeartRateZoneGroup : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZoneGroup"/> class with
        /// default values.
        /// </summary>
        ///
        public HeartRateZoneGroup()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZoneGroup"/> class
        /// with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the heart rate zone grouping.
        /// </param>
        ///
        public HeartRateZoneGroup(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HeartRateZoneGroup"/> class
        /// with the specified name and target zones.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the heart rate zone grouping.
        /// </param>
        ///
        /// <param name="targetZones">
        /// The target heart rate zones for the grouping.
        /// </param>
        ///
        public HeartRateZoneGroup(
            string name,
            IEnumerable<HeartRateZone> targetZones)
        {
            this.Name = name;

            if (targetZones != null)
            {
                foreach (HeartRateZone zone in targetZones)
                {
                    if (zone != null)
                    {
                        this.zones.Add(zone);
                    }
                }
            }
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the heart rate zone group information.
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

            XPathNodeIterator zoneIterator =
                navigator.Select("heartrate-zone");
            foreach (XPathNavigator zoneNav in zoneIterator)
            {
                HeartRateZone zone = new HeartRateZone();
                zone.ParseXml(zoneNav);
                this.zones.Add(zone);
            }
        }

        /// <summary>
        /// Writes the XML representation of the heart rate zone group into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the heart rate zone group.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the heart rate zone group should be
        /// written.
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="TargetZones"/> property contains no zones.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIf(
                this.zones.Count < 1,
                "HeartRateZoneGroupNoZones");

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(this.name))
            {
                writer.WriteAttributeString("name", this.name);
            }

            foreach (HeartRateZone zone in this.zones)
            {
                zone.WriteXml("heartrate-zone", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name for the heart rate zone group.
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
        /// Gets the target heart rate zones for the zone group.
        /// </summary>
        ///
        /// <value>
        /// A collection of zones.
        /// </value>
        ///
        /// <remarks>
        /// To add a zone to the group, call Add on the returned collection.
        /// </remarks>
        ///
        public Collection<HeartRateZone> TargetZones => this.zones;

        private readonly Collection<HeartRateZone> zones =
            new Collection<HeartRateZone>();
    }
}
