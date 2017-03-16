// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents information about a health record that is not considered personally
    /// identifiable.
    /// </summary>
    ///
    public class Basic : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Basic"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Basic()
            : base(TypeId)
        {
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
            new Guid("bf516a61-5252-4c28-a979-27f45f62f78d");

        /// <summary>
        /// Populates this <see cref="Basic"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the basic data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a basic node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator basicNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("basic");

            Validator.ThrowInvalidIfNull(basicNav, Resources.BasicUnexpectedNode);

            XPathNavigator genderNav =
                basicNav.SelectSingleNode("gender");

            if (genderNav != null)
            {
                string genderString = genderNav.Value;
                if (string.Equals(
                        genderString,
                        "m",
                        StringComparison.Ordinal))
                {
                    this.gender = ItemTypes.Gender.Male;
                }
                else if (
                    string.Equals(
                        genderString,
                        "f",
                        StringComparison.Ordinal))
                {
                    this.gender = ItemTypes.Gender.Female;
                }
                else
                {
                    this.gender = ItemTypes.Gender.Unknown;
                }
            }

            XPathNavigator birthYearNav =
                basicNav.SelectSingleNode("birthyear");

            if (birthYearNav != null)
            {
                this.birthYear = birthYearNav.ValueAsInt;
            }

            XPathNavigator countryNav =
                basicNav.SelectSingleNode("country");

            if (countryNav != null)
            {
                this.country = countryNav.Value;
            }

            XPathNavigator postalNav =
                basicNav.SelectSingleNode("postcode");

            if (postalNav != null)
            {
                this.postalCode = postalNav.Value;
            }

            XPathNavigator cityNav =
                basicNav.SelectSingleNode("city");

            if (cityNav != null)
            {
                this.city = cityNav.Value;
            }

            XPathNavigator stateNav = basicNav.SelectSingleNode("state");
            if (stateNav != null)
            {
                this.stateOrProvince = stateNav.Value;
            }

            XPathNavigator dayOfWeekNav =
                basicNav.SelectSingleNode("firstdow");

            if (dayOfWeekNav != null)
            {
                this.firstDayOfWeek = (DayOfWeek)(dayOfWeekNav.ValueAsInt - 1);
            }

            XPathNodeIterator languageIterator =
                basicNav.Select("language");

            foreach (XPathNavigator languageNav in languageIterator)
            {
                Language newLanguage = new Language();
                newLanguage.ParseXml(languageNav);

                this.languages.Add(newLanguage);
            }
        }

        /// <summary>
        /// Writes the basic data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the basic data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <basic>
            writer.WriteStartElement("basic");

            if (this.gender != null)
            {
                if (this.gender == ItemTypes.Gender.Male)
                {
                    writer.WriteElementString("gender", "m");
                }
                else if (this.gender == ItemTypes.Gender.Female)
                {
                    writer.WriteElementString("gender", "f");
                }
            }

            if (this.birthYear != null)
            {
                writer.WriteElementString(
                    "birthyear",
                    ((int)this.birthYear).ToString(CultureInfo.InvariantCulture));
            }

            if (this.country != null)
            {
                writer.WriteElementString("country", this.country);
            }

            if (this.postalCode != null)
            {
                writer.WriteElementString("postcode", this.postalCode);
            }

            if (this.city != null)
            {
                writer.WriteElementString("city", this.city);
            }

            if (!string.IsNullOrEmpty(this.stateOrProvince))
            {
                writer.WriteElementString("state", this.stateOrProvince);
            }

            if (this.firstDayOfWeek != null)
            {
                // The DayOfWeek enum starts at 0 whereas the XSD starts at
                // 1.

                writer.WriteElementString(
                    "firstdow",
                    ((int)this.firstDayOfWeek + 1).ToString(CultureInfo.InvariantCulture));
            }

            foreach (Language language in this.languages)
            {
                language.WriteXml("language", writer);
            }

            // </basic>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the gender of the person.
        /// </summary>
        ///
        /// <value>
        /// The person's gender.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the gender should not be stored.
        /// </remarks>
        ///
        public Gender? Gender
        {
            get { return this.gender; }
            set { this.gender = value; }
        }

        private Gender? gender;

        /// <summary>
        /// Gets or sets the birth year of the person.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the year.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the birth year should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than 1000 or greater than
        /// 3000 when setting the value.
        /// </exception>
        ///
        public int? BirthYear
        {
            get { return this.birthYear; }

            set
            {
                if (value != null && ((int)value < 1000 || (int)value > 3000))
                {
                    throw new ArgumentOutOfRangeException(nameof(this.BirthYear), Resources.BasicBirthYearOutOfRange);
                }

                this.birthYear = value;
            }
        }

        private int? birthYear;

        /// <summary>
        /// Gets or sets the country of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the two-letter ISO3166-2 code for the
        /// country/region.
        /// </value>
        ///
        public string Country
        {
            get { return this.country; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Country");
                this.country = value;
            }
        }

        private string country;

        /// <summary>
        /// Gets or sets the postal code of the country of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the postal code.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string PostalCode
        {
            get { return this.postalCode; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "PostalCode");
                this.postalCode = value;
            }
        }

        private string postalCode;

        /// <summary>
        /// Gets or sets the city of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the city.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string City
        {
            get { return this.city; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "City");
                this.city = value;
            }
        }

        private string city;

        /// <summary>
        /// Gets or sets the state or province of residence.
        /// </summary>
        ///
        /// <value>
        /// A string representing the state or province.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the state
        /// should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string StateOrProvince
        {
            get { return this.stateOrProvince; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "StateOrProvince");
                this.stateOrProvince = value;
            }
        }

        private string stateOrProvince;

        /// <summary>
        /// Gets or sets the preferred first day of the week.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="DayOfWeek"/> instance representing the day.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the first day of the week
        /// should not be stored.
        /// </remarks>
        ///
        public DayOfWeek? FirstDayOfWeek
        {
            get { return this.firstDayOfWeek; }
            set { this.firstDayOfWeek = value; }
        }

        private DayOfWeek? firstDayOfWeek;

        /// <summary>
        /// Gets the language(s) the person speaks.
        /// </summary>
        ///
        /// <value>
        /// A list of the languages.
        /// </value>
        ///
        public IList<Language> Languages => this.languages;

        private readonly List<Language> languages = new List<Language>();

        /// <summary>
        /// Gets a string representation of the basic item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the basic item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.Gender != null || this.BirthYear != null)
            {
                if (this.Gender != null && this.BirthYear != null)
                {
                    string localizedGenderString = ResourceUtilities.ResourceManager.GetString(this.Gender.ToString());
                    return string.Format(
                        Resources.BasicToStringFormatGenderAndBirthYear,
                        localizedGenderString,
                        this.BirthYear);
                }

                if (this.BirthYear != null)
                {
                    return string.Format(
                        Resources.BasicToStringFormatBirthYear,
                        this.BirthYear);
                }

                return ResourceUtilities.ResourceManager.GetString(this.Gender.ToString());
            }

            if (this.PostalCode != null || this.Country != null)
            {
                if (this.PostalCode != null && this.Country != null)
                {
                    return string.Format(
                        Resources.BasicToStringFormatPostalCodeAndCountry,
                        this.PostalCode,
                        this.Country);
                }

                if (this.Country != null)
                {
                    return this.Country;
                }

                return this.PostalCode;
            }

            if (this.CommonData.Note != null)
            {
                return this.CommonData.Note;
            }

            return Resources.BasicToStringSeeDetails;
        }
    }
}
