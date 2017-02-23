// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
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

            this.city = city;
            this.country = country;
            this.postalCode = postalCode;
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

            this.city = city;
            this.country = country;
            this.postalCode = postalCode;

            foreach (string streetString in street)
            {
                this.street.Add(streetString);
            }

            Validator.ThrowArgumentExceptionIf(
                this.street.Count == 0,
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
                this.description = descNav.Value;
            }

            XPathNavigator isPrimaryNav =
                navigator.SelectSingleNode("is-primary");

            if (isPrimaryNav != null)
            {
                this.isPrimary = isPrimaryNav.ValueAsBoolean;
            }

            XPathNodeIterator streetIterator =
                navigator.Select("street");

            foreach (XPathNavigator streetNav in streetIterator)
            {
                this.street.Add(streetNav.Value);
            }

            this.city = navigator.SelectSingleNode("city").Value;

            this.state = XPathHelper.GetOptNavValue(navigator, "state");

            this.postalCode = navigator.SelectSingleNode("postcode").Value;
            this.country = navigator.SelectSingleNode("country").Value;

            this.county = XPathHelper.GetOptNavValue(navigator, "county");
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
                this.street.Count == 0,
                "AddressStreetNotSet");

            Validator.ThrowSerializationIfNull(this.city, "AddressCityNotSet");
            Validator.ThrowSerializationIfNull(this.country, "AddressCountryNotSet");
            Validator.ThrowSerializationIfNull(this.postalCode, "AddressPostalCodeNotSet");

            writer.WriteStartElement(nodeName);

            if (!string.IsNullOrEmpty(this.description))
            {
                writer.WriteElementString("description", this.description);
            }

            if (this.isPrimary != null)
            {
                writer.WriteElementString(
                    "is-primary",
                    SDKHelper.XmlFromBool((bool)this.isPrimary));
            }

            foreach (string street in this.street)
            {
                writer.WriteElementString("street", street);
            }

            writer.WriteElementString("city", this.city);
            if (!string.IsNullOrEmpty(this.state))
            {
                writer.WriteElementString("state", this.state);
            }

            writer.WriteElementString("postcode", this.postalCode);
            writer.WriteElementString("country", this.country);

            XmlWriterHelper.WriteOptString(writer, "county", this.county);

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
            get { return this.description; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

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
            get { return this.isPrimary; }
            set { this.isPrimary = value; }
        }

        private bool? isPrimary;

        /// <summary>
        /// Gets the street number, name, apartment, and so on.
        /// </summary>
        ///
        /// <value>
        /// A string collection of address information.
        /// </value>
        ///
        public Collection<string> Street => this.street;

        private readonly Collection<string> street = new Collection<string>();

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
            get { return this.city; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "City", "AddressCityMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "City");
                this.city = value;
            }
        }

        private string city;

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
            get { return this.state; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "State");
                this.state = value;
            }
        }

        private string state;

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
            get { return this.country; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Country", "AddressCountryMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "Country");
                this.country = value;
            }
        }

        private string country;

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
            get { return this.postalCode; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "PostalCode", "AddressPostalCodeMandatory");
                Validator.ThrowIfStringIsWhitespace(value, "PostalCode");
                this.postalCode = value;
            }
        }

        private string postalCode;

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
            get { return this.county; }

            set
            {
                if (value != null)
                {
                    Validator.ThrowIfStringNullOrEmpty(value, "County");
                    Validator.ThrowIfStringIsWhitespace(value, "County");
                }

                this.county = value;
            }
        }

        private string county;

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

            if (this.IsPrimary != null && this.IsPrimary.Value)
            {
                result.Append(ResourceRetriever.GetResourceString("IsPrimary"));
            }

            foreach (string street in this.Street)
            {
                result.Append(street);
                result.Append(ResourceRetriever.GetResourceString("ListSeparator"));
            }

            result.Append(this.City);

            if (!string.IsNullOrEmpty(this.County))
            {
                result.AppendFormat(listFormat, this.County);
            }

            if (!string.IsNullOrEmpty(this.State))
            {
                result.AppendFormat(listFormat, this.State);
            }

            result.AppendFormat(listFormat, this.PostalCode);

            if (!string.IsNullOrEmpty(this.Country))
            {
                result.AppendFormat(listFormat, this.Country);
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                result.AppendFormat(listFormat, this.Description);
            }

            return result.ToString();
        }
    }
}
