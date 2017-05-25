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
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a grouping of target glucose zones by name.
    /// </summary>
    ///
    public class TargetGlucoseZoneGroup : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZoneGroup"/>
        /// class with default values.
        /// </summary>
        ///
        public TargetGlucoseZoneGroup()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZoneGroup"/>
        /// class with the specified name.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the target glucose zone grouping.
        /// </param>
        ///
        public TargetGlucoseZoneGroup(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TargetGlucoseZoneGroup"/>
        /// class with the specified name and target zones.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the target glucose zone grouping.
        /// </param>
        ///
        /// <param name="targetZones">
        /// The target glucose zones for the grouping.
        /// </param>
        ///
        public TargetGlucoseZoneGroup(
            string name,
            IEnumerable<TargetGlucoseZone> targetZones)
        {
            Name = name;

            if (targetZones != null)
            {
                foreach (TargetGlucoseZone zone in targetZones)
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
        /// The XML containing the target glucose zone group information.
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
                navigator.Select("target-glucose-zone");
            foreach (XPathNavigator zoneNav in zoneIterator)
            {
                TargetGlucoseZone zone = new TargetGlucoseZone();
                zone.ParseXml(zoneNav);
                _zones.Add(zone);
            }
        }

        /// <summary>
        /// Writes the XML representation of the target glucose zone group into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the target glucose zone group.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the target glucose zone group should be
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(_name))
            {
                writer.WriteAttributeString("name", _name);
            }

            foreach (TargetGlucoseZone zone in _zones)
            {
                zone.WriteXml("target-glucose-zone", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name for the target glucose zone group.
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
        /// Gets the target glucose zones for the zone group.
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
        public Collection<TargetGlucoseZone> TargetZones => _zones;

        private readonly Collection<TargetGlucoseZone> _zones =
            new Collection<TargetGlucoseZone>();
    }
}
