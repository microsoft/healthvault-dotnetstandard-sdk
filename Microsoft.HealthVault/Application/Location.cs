// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Application
{
    /// <summary>
    /// A location defined by country/region and state/province ISO 3166 codes.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Constructs a default instance of a location with no country/region or state/province.
        /// </summary>
        public Location()
        {
        }

        /// <summary>
        /// Constructs a location instance with the specified country/region and state/province codes.
        /// </summary>
        ///
        /// <param name="country">
        /// An ISO 3166-1 two-letter country/region code.
        /// </param>
        ///
        /// <param name="stateProvince">
        /// An ISO 3166-2 state/province code without the country/region prefix.
        /// </param>
        ///
        public Location(string country, string stateProvince)
        {
            Country = country;
            StateProvince = stateProvince;
        }

        /// <summary>
        /// Gets or sets the country/region code.
        /// </summary>
        ///
        /// <remarks>
        /// An ISO 3166-1 two letter country/region code.
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Country value cannot be null or white spaces only.", nameof(value));
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
        /// An ISO 3166-2 state/province code without the country/region prefix.
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
            if (!string.IsNullOrEmpty(Country))
            {
                writer.WriteStartElement(elementName);

                writer.WriteElementString("country", Country);

                if (!string.IsNullOrEmpty(StateProvince))
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
            string result = string.Empty;

            if (!string.IsNullOrEmpty(Country))
            {
                if (!string.IsNullOrEmpty(StateProvince))
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
