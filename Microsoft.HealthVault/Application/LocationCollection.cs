// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// A location defined by country and state/province ISO 3166 codes.
    /// </summary>
    public class LocationCollection : Collection<Location>
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

            this.AllLocations = allLocationsIterator != null;

            XPathNodeIterator supportedRecordLocationsIterator = locationCollection.Select("supported-record-locations/location");
            foreach (XPathNavigator supportedLocationNav in supportedRecordLocationsIterator)
            {
                Location location = new Location();
                location.ParseXml(supportedLocationNav);

                this.Add(location);
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

            if (this.AllLocations || this.Count > 0)
            {
                writer.WriteStartElement(elementName);

                foreach (Location location in this)
                {
                    location.WriteXml(writer, "location");
                }

                if (this.AllLocations)
                {
                    writer.WriteElementString("all-locations", "true");
                }

                writer.WriteEndElement();
            }
        }
    }
}