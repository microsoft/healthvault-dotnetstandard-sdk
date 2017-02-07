// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a physical mailing address.
    /// </summary>
    /// 
    public class Address : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Address"/> class with default values.
        /// </summary>
        /// 
        public Address()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Address"/> class with 
        /// values for the mandatory properties.
        /// </summary>
        /// 
        /// <param name="city">
        /// The name of the city.
        /// </param>
        /// 
        /// <param name="country">
        /// The name of the country.
        /// </param>
        /// 
        /// <param name="postalCode">
        /// The postal code.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="city"/>, <paramref name="country"/> or
        /// <paramref name="postalCode"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public Address(string city, string country, string postalCode)
        {
            Validator.ThrowIfStringNullOrEmpty(city, "city");
            Validator.ThrowIfStringNullOrEmpty(country, "country");
            Validator.ThrowIfStringNullOrEmpty(postalCode, "postalCode");

            _city = city;
            _country = country;
            _postalCode = postalCode;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="Address"/> class with 
        /// values for the mandatory properties.
        /// </summary>
        /// 
        /// <param name="street">
        /// The street address.
        /// </param>
        /// 
        /// <param name="city">
        /// The name of the city.
        /// </param>
        /// 
        /// <param name="country">
        /// The name of the country.
        /// </param>
        /// 
        /// <param name="postalCode">
        /// The postal code.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="street"/>, <paramref name="city"/>, <paramref name="country"/> or
        /// <paramref name="postalCode"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        public Address(string city, string country, string postalCode, IEnumerable<string> street)
        {
            Validator.ThrowIfArgumentNull(street, "street", "AddressStreetMandatory");
            Validator.ThrowIfStringNullOrEmpty(city, "city");
            Validator.ThrowIfStringNullOrEmpty(country, "country");
            Validator.ThrowIfStringNullOrEmpty(postalCode, "postalCode");

            _city = city;
            _country = country;
            _postalCode = postalCode;

            foreach (string streetString in street)
            {
                _street.Add(streetString);
            }

            Validator.ThrowArgumentExceptionIf(
                _street.Count == 0,
                "street", 
                "AddressStreetMandatory");
        }
 
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the address information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);
            
            XPathNavigator descNav =
                navigator.SelectSingleNode("description");

            if (descNav != null)
            {
                _description = descNav.Value;
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");

            if (isPrimaryNav != null)
            {
                _isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            XPathNodeIterator streetIterator =
                navigator.Select("street");

            foreach (XPathNavigator streetNav in streetIterator)
            {
                _street.Add(streetNav.Value);
            }

            _city = navigator.SelectSingleNode("city").Value;

            _state = XPathHelper.GetOptNavValue(navigator, "state");

            _postalCode = navigator.SelectSingleNode("postcode").Value;
            _country = navigator.SelectSingleNode("country").Value;

            _county = XPathHelper.GetOptNavValue(navigator, "county");
        }

        /// <summary>
        /// Writes the XML representation of the address into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the address.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the address should be 
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
        /// The <see cref="Street"/> property is empty or <see cref="City"/>, 
        /// <see cref="Country"/>, or <see cref="PostalCode"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            Validator.ThrowSerializationIf(
                _street.Count == 0,
                "AddressStreetNotSet");

            Validator.ThrowSerializationIfNull(_city, "AddressCityNotSet");
            Validator.ThrowSerializationIfNull(_country, "AddressCountryNotSet");
            Validator.ThrowSerializationIfNull(_postalCode, "AddressPostalCodeNotSet");

            writer.WriteStartElement(nodeName);

            if (!String.IsNullOrEmpty(_description))
            {
                writer.WriteElementString("description", _description);
            }

            if (_isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary", 
                    SDKHelper.XmlFromBool((bool)_isPrimary));
            }

            foreach (string street in _street)
            {
                writer.WriteElementString("street", street);
            }

            writer.WriteElementString("city", _city);
            if (!String.IsNullOrEmpty(_state))
            {
                writer.WriteElementString("state", _state);
            }

            writer.WriteElementString("postcode", _postalCode);
            writer.WriteElementString("country", _country);

            XmlWriterHelper.WriteOptString(writer, "county", _county);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description for the address.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the description.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the description should not be stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string Description
        {
            get { return _description; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }
        private string _description;

        /// <summary>
        /// Gets or sets a value indicating whether the address is the primary 
        /// address for the person.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the address is the primary one; otherwise, <b>false</b>.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if <see cref="IsPrimary"/> should not be stored.
        /// </remarks>
        /// 
        public bool? IsPrimary
        {
            get { return _isPrimary; }
            set { _isPrimary = value; }
        }
        private bool? _isPrimary;

        /// <summary>
        /// Gets the street number, name, apartment, and so on.
        /// </summary>
        /// 
        /// <value>
        /// A string collection of address information.
        /// </value>
        /// 
        public Collection<string> Street
        {
            get { return _street; }
        }
        private Collection<string> _street = new Collection<string>();

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the city name.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        /// 
        public string City
        {
            get { return _city; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "City", "AddressCityMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "City");
                _city = value;
            }
        }
        private string _city;

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the state name.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the state should not be stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string State
        {
            get { return _state; }
            set 
            {
                Validator.ThrowIfStringIsWhitespace(value, "State");
                _state = value;
            }
        }
        private string _state;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the country name.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only 
        /// whitespace during set.
        /// </exception>
        /// 
        public string Country
        {
            get { return _country; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Country", "AddressCountryMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "Country");
                _country = value;
            }
        }
        private string _country;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the postal code.
        /// </value>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        /// 
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "PostalCode", "AddressPostalCodeMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "PostalCode");
                _postalCode = value;
            }
        }
        private string _postalCode;

        /// <summary>
        /// Gets or sets the county.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the county name.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the county should not be stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string County
        {
            get { return _county; }
            set
            {
                if (value != null)
                {
                    Validator.ThrowIfStringNullOrEmpty(value, "County");
                    Validator.ThrowIfStringIsWhitespace(value, "County");
                }
                _county = value;
            }
        }

        private string _county;

        /// <summary>
        /// Gets a string representation of the address.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the address with commas separating the "lines"
        /// of the address.
        /// </returns>
        /// 
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            string listFormat = ResourceRetriever.GetResourceString("ListFormat");

            if (IsPrimary != null && IsPrimary.Value)
            {
                result.Append(ResourceRetriever.GetResourceString("IsPrimary"));
            }

            foreach (string street in Street)
            {
                result.Append(street);
                result.Append(ResourceRetriever.GetResourceString("ListSeparator"));
            }

            result.Append(City);

            if (!String.IsNullOrEmpty(County))
            {
                result.AppendFormat(listFormat, County);
            }

            if (!String.IsNullOrEmpty(State))
            {
                result.AppendFormat(listFormat, State);
            }

            result.AppendFormat(listFormat, PostalCode);

            if (!String.IsNullOrEmpty(Country))
            {
                result.AppendFormat(listFormat, Country);
            }

            if (!String.IsNullOrEmpty(Description))
            {
                result.AppendFormat(listFormat, Description);
            }

            return result.ToString();
        }
    }
}
