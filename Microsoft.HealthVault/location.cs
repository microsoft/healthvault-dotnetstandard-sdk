// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// A location defined by country and state/province ISO 3166 codes.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Constructs a default instance of a location with no country or state/province.
        /// </summary>
        internal Location()
        {
        }

        /// <summary>
        /// Constructs a location instance with the specified country and state/province codes.
        /// </summary>
        /// 
        /// <param name="country">
        /// An ISO 3166-1 two-letter country code.
        /// </param>
        /// 
        /// <param name="stateProvince">
        /// An ISO 3166-2 state/province code without the country prefix.
        /// </param>
        /// 
        public Location(string country, string stateProvince)
        {
            Country = country;
            StateProvince = stateProvince;
        }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// 
        /// <remarks>
        /// An ISO 3166-1 two letter country code.
        /// </remarks>
        /// 
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Country value cannot be null or white spaces only.", "value");
                }

                _country = value;
            }
        }
        private string _country;

        /// <summary>
        /// Gets or sets the state/province code.
        /// </summary>
        /// 
        /// <remarks>
        /// An ISO 3166-2 state/province code without the country prefix.
        /// </remarks>
        /// 
        public string StateProvince { get; set; }

        /// <summary>
        /// Parses the location XML and populates the instance properties from
        /// the results.
        /// </summary>
        /// 
        /// <param name="location">
        /// XML of the location to be parsed.
        /// </param>
        /// 
        public void ParseXml(XPathNavigator location)
        {
            XPathNavigator countryNav = location.SelectSingleNode("country");
            if (countryNav != null)
            {
                Country = countryNav.Value;
            }

            XPathNavigator stateNav = location.SelectSingleNode("state-province");
            if (stateNav != null)
            {
                StateProvince = stateNav.Value;
            }
        }

        /// <summary>
        /// Writes the location to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the location xml to.
        /// </param>
        /// 
        /// <param name="elementName">
        /// The name of the containing element to use when writing the location. Typically "location".
        /// </param>
        /// 
        public void WriteXml(XmlWriter writer, string elementName)
        {
            if (!String.IsNullOrEmpty(Country))
            {
                writer.WriteStartElement(elementName);

                writer.WriteElementString("country", Country);

                if (!String.IsNullOrEmpty(StateProvince))
                {
                    writer.WriteElementString("state-province", StateProvince);
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Gets a string representation of the location.
        /// </summary>
        /// 
        /// <returns>
        /// <cref name="Country"/>-<cref name="StateProvince"/>
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = String.Empty;

            if (!String.IsNullOrEmpty(Country))
            {
                if (!String.IsNullOrEmpty(StateProvince))
                {
                    result = Country + "-" + StateProvince;
                }
                else
                {
                    result = Country;
                }
            }

            return result;
        }
    }
}

