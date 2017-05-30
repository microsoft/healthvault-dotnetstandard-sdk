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
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
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
            Name = name;
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
            Name = name;

            if (targetZones != null)
            {
                foreach (HeartRateZone zone in targetZones)
                {
                    if (zone != null)
                    {
                        _zones.Add(zone);
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
                _name = name;
            }

            XPathNodeIterator zoneIterator =
                navigator.Select("heartrate-zone");
            foreach (XPathNavigator zoneNav in zoneIterator)
            {
                HeartRateZone zone = new HeartRateZone();
                zone.ParseXml(zoneNav);
                _zones.Add(zone);
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="TargetZones"/> property contains no zones.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            if (_zones.Count < 1)
            {
                throw new ThingSerializationException(Resources.HeartRateZoneGroupNoZones);
            }

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(_name))
            {
                writer.WriteAttributeString("name", _name);
            }

            foreach (HeartRateZone zone in _zones)
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
            get { return _name; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Name");
                _name = value;
            }
        }

        private string _name;

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
        public Collection<HeartRateZone> TargetZones => _zones;

        private readonly Collection<HeartRateZone> _zones =
            new Collection<HeartRateZone>();
    }
}
