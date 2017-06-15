// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// A location defined by country and state/province ISO 3166 codes.
    /// </summary>
    internal class LocationCollection : Collection<Location>
    {
        /// <summary>
        /// Gets or sets if location collection supports all locations.
        /// </summary>
        public bool AllLocations { get; set; }

        /// <summary>
        /// Parses the location collection XML and populates the instance properties from
        /// the results.
        /// </summary>
        ///
        /// <param name="locationCollection">
        /// XML of the location collection to be parsed.
        /// </param>
        ///
        public void ParseXml(XPathNavigator locationCollection)
        {
            if (locationCollection == null)
            {
                throw new ArgumentNullException(nameof(locationCollection));
            }

            XPathNavigator allLocationsIterator = locationCollection.SelectSingleNode("supported-record-locations/all-locations");

            AllLocations = allLocationsIterator != null;

            XPathNodeIterator supportedRecordLocationsIterator = locationCollection.Select("supported-record-locations/location");
            foreach (XPathNavigator supportedLocationNav in supportedRecordLocationsIterator)
            {
                Location location = new Location();
                location.ParseXml(supportedLocationNav);

                Add(location);
            }
        }

        /// <summary>
        /// Writes the location collection to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the location collection xml to.
        /// </param>
        ///
        /// <param name="elementName">
        /// The name of the containing element to use when writing the location collection.
        /// </param>
        ///
        public void WriteXml(XmlWriter writer, string elementName)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (AllLocations || Count > 0)
            {
                writer.WriteStartElement(elementName);

                foreach (Location location in this)
                {
                    location.WriteXml(writer, "location");
                }

                if (AllLocations)
                {
                    writer.WriteElementString("all-locations", "true");
                }

                writer.WriteEndElement();
            }
        }
    }
}