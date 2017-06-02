// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Record;
using NodaTime;

namespace Microsoft.HealthVault.Person
{
    /// <summary>
    /// Provides information about a person's HealthVault account.
    /// </summary>
    ///
    public class PersonInfo
    {
        /// <summary>
        /// Creates a new instance of the PersonInfo class using
        /// the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the person information.
        /// </param>
        ///
        /// <returns>
        /// A new instance of <see cref="PersonInfo"/> populated with the
        /// person information.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>
        /// </exception>
        ///
        public static PersonInfo CreateFromXml(
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            PersonInfo personInfo = new PersonInfo();
            personInfo.ParseXml(navigator);
            return personInfo;
        }

        /// <summary>
        /// Allows derived classes to construct an instance of themselves given
        /// an instance of the base class.
        /// </summary>
        ///
        /// <param name="personInfo">
        /// Information about the person for constructing the instance.
        /// </param>
        ///
        internal PersonInfo(PersonInfo personInfo)
        {
            _personId = personInfo._personId;
            Name = personInfo.Name;
            _selectedRecordId = personInfo._selectedRecordId;
        }

        /// <summary>
        /// Constructs an new instance of the <see cref="PersonInfo"/> class for testing purposes.
        /// </summary>
        ///
        public PersonInfo()
        {
        }

        #region XMl

        /// <summary>
        /// Populates the class members with data from the specified
        /// person information XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the person information from.
        /// </param>
        ///
        internal virtual void ParseXml(XPathNavigator navigator)
        {
            _personId = new Guid(navigator.SelectSingleNode("person-id").Value);

            Name = navigator.SelectSingleNode("name").Value;

            XPathNavigator navAppSettings = navigator.SelectSingleNode("app-settings");

            if (navAppSettings != null)
            {
                XDocument doc = SDKHelper.SafeLoadXml(navAppSettings.OuterXml);
                ApplicationSettingsDocument = doc;
            }

            XPathNavigator navSelectedRecordId =
                navigator.SelectSingleNode("selected-record-id");

            if (navSelectedRecordId != null)
            {
                _selectedRecordId = new Guid(navSelectedRecordId.Value);
            }

            XPathNavigator navPreferredCulture =
                navigator.SelectSingleNode("preferred-culture[language != '']");
            if (navPreferredCulture != null)
            {
                PreferredCulture = null;
                XPathNavigator navLanguageCode = navPreferredCulture.SelectSingleNode("language");
                {
                    PreferredCulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredCulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        PreferredCulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator navPreferredUICulture =
                navigator.SelectSingleNode("preferred-uiculture[language != '']");
            if (navPreferredUICulture != null)
            {
                PreferredUICulture = null;
                XPathNavigator navLanguageCode = navPreferredUICulture.SelectSingleNode("language");
                {
                    PreferredUICulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredUICulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        PreferredUICulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator locationNav = navigator.SelectSingleNode("location");
            if (locationNav != null)
            {
                Location = new Location();
                Location.ParseXml(locationNav);
            }

            XPathNodeIterator recordsNav = navigator.Select("record");
            foreach (XPathNavigator recordNav in recordsNav)
            {
                // Now see if we can fill in the record information
                HealthRecordInfo record = HealthRecordInfo.CreateFromXml(recordNav);
                if (record != null)
                {
                    AuthorizedRecords.Add(record.Id, record);
                }
            }

            XPathNavigator navMoreRecords = navigator.SelectSingleNode("more-records");
            if (navMoreRecords != null)
            {
                HasMoreRecords = true;
            }

            XPathNavigator navMoreAppSettings = navigator.SelectSingleNode("more-app-settings");
            if (navMoreAppSettings != null)
            {
                HasMoreApplicationSettings = true;
            }
        }

        /// <summary>
        /// Gets the XML representation of the <see cref="PersonInfo"/>.
        /// </summary>
        ///
        /// <returns>
        /// A XML string containing the person information.
        /// </returns>
        ///
        /// <remarks>
        /// This method can be used to get a serialized version of the
        /// <see cref="PersonInfo"/>.
        /// </remarks>
        ///
        public string GetXml()
        {
            StringBuilder personInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(personInfoXml, settings))
            {
                WriteXml("person-info", writer);
                writer.Flush();
            }

            return personInfoXml.ToString();
        }

        private void WriteXml(string nodeName, XmlWriter writer)
        {
            bool writeContainingNode = false;
            if (!string.IsNullOrEmpty(nodeName))
            {
                writeContainingNode = true;
                writer.WriteStartElement(nodeName);
            }

            writer.WriteElementString("person-id", _personId.ToString());
            writer.WriteElementString("name", Name);

            if (ApplicationSettingsDocument != null)
            {
                writer.WriteRaw(ApplicationSettingsDocument.CreateNavigator().OuterXml);
            }

            if (HasMoreApplicationSettings)
            {
                writer.WriteElementString("more-app-settings", string.Empty);
            }

            if (_selectedRecordId != Guid.Empty)
            {
                writer.WriteElementString(
                    "selected-record-id",
                    _selectedRecordId.ToString());
            }

            foreach (HealthRecordInfo record in AuthorizedRecords.Values)
            {
                record.WriteXml("record", writer);
            }

            if (HasMoreRecords)
            {
                writer.WriteElementString("more-records", string.Empty);
            }

            if (!string.IsNullOrEmpty(PreferredCulture))
            {
                WriteCulture("preferred-culture", writer, PreferredCulture);
            }

            if (!string.IsNullOrEmpty(PreferredUICulture))
            {
                WriteCulture("preferred-uiculture", writer, PreferredUICulture);
            }

            Location?.WriteXml(writer, "location");

            if (writeContainingNode)
            {
                writer.WriteEndElement();
            }
        }

        private static void WriteCulture(
            string elementName,
            XmlWriter writer,
            string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                writer.WriteStartElement(elementName);
                writer.WriteElementString("language", culture);
                writer.WriteEndElement();
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the HealthVault unique identifier for the person.
        /// </summary>
        ///
        /// <value>
        /// A GUID that is assigned to the account when it was created.
        /// </value>
        ///
        public Guid PersonId
        {
            get
            {
                return _personId;
            }

            set { _personId = value; }
        }

        private Guid _personId;

        /// <summary>
        /// Gets or sets the person's name.
        /// </summary>
        ///
        /// <value>
        /// The person's full name as it was entered into HealthVault.
        /// </value>
        ///
        /// <remarks>
        /// The person's full name can  be changed only by going to the
        /// HealthVault Shell.
        /// </remarks>
        ///
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the underlying application settings document.
        /// </summary>
        public XDocument ApplicationSettingsDocument { get; set; }

        /// <summary>
        /// Gets or sets the record the person has chosen to use as the default
        /// record for the current application.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthRecordInfo"/> representing the record the person has
        /// chosen to use as the default record for the current application.
        /// </value>
        ///
        /// <remarks>
        /// Note that this property is only applicable to single-record
        /// applications (SRAs). <br />
        /// A value of <b>null</b> indicates that no record has been selected or that
        /// the person does not have sufficient rights to the record to use
        /// it with the current application.
        /// <br/>
        /// Setting the selected record through will only change the
        /// selected record for this instance. Other instances of the
        /// <see cref="PersonInfo"/> for the same person will have the original
        /// selected record. The user can set their selected record through the
        /// HealthVault Shell.
        /// </remarks>
        ///
        public HealthRecordInfo SelectedRecord
        {
            get
            {
                if (_selectedRecord == null)
                {
                    if (_selectedRecordId != Guid.Empty &&
                        AuthorizedRecords.ContainsKey(_selectedRecordId))
                    {
                        _selectedRecord = AuthorizedRecords[_selectedRecordId];
                    }
                }

                return _selectedRecord;
            }

            set
            {
                _selectedRecord = value;

                if (value != null &&
                    value.Id != Guid.Empty)
                {
                    _selectedRecordId = value.Id;
                }

                SelectedRecordChanged?.Invoke(this, new EventArgs());
            }
        }

        private HealthRecordInfo _selectedRecord;
        private Guid _selectedRecordId;

        /// <summary>
        /// Occurs when the <see cref="SelectedRecord"/> setter is called.
        /// </summary>
        ///
        /// <remarks>
        /// This event is not triggered if another instance of the PersonInfo gets updated or
        /// if the value changes in HealthVault.
        /// The sender is the PersonInfo instance that was updated.
        /// The event args are empty.
        /// </remarks>
        ///
        public event EventHandler SelectedRecordChanged;

        /// <summary>
        /// Gets a dictionary of all the authorized records.
        /// </summary>
        public IDictionary<Guid, HealthRecordInfo> AuthorizedRecords { get; set; } = new Dictionary<Guid, HealthRecordInfo>();

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        ///
        /// <remarks>
        /// The preferred culture should be used when formatting date/time, numbers, collating, etc.
        /// </remarks>
        ///
        public string PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets the user's preferred UI culture.
        /// </summary>
        ///
        /// <remarks>
        /// The preferred UI culture should be used when retrieving text from resources for display
        /// to the user.
        /// </remarks>
        ///
        public string PreferredUICulture { get; set; }

        /// <summary>
        /// Gets the location of the user account.
        /// </summary>
        ///
        public Location Location { get; set; }

        public bool HasMoreRecords { get; set; }

        public bool HasMoreApplicationSettings { get; set; }

        #endregion public properties

        /// <summary>
        /// Gets the <see cref="HealthRecordInfo"/> for the first health record
        /// that has a relationship of <see cref="RelationshipType.Self"/> with the
        /// person.
        /// </summary>
        ///
        /// <returns>
        /// The first <see cref="HealthRecordInfo"/> having the
        /// <see cref="RelationshipType.Self"/> relationship with the person.
        /// </returns>
        ///
        /// <remarks>
        /// Since a person may have more than one or no
        /// <see cref="RelationshipType.Self"/> records, this method returns an
        /// error if the person does not have a self record, but it returns
        /// the first self record if they have multiple records. There is no
        /// guarantee that the first record will always be the same one.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The person does not have an authorized record with the
        /// <see cref="RelationshipType.Self"/> relationship that is in the
        /// Active state with the authorization expiration
        /// date anytime in the future.
        /// </exception>
        ///
        public HealthRecordInfo GetSelfRecord()
        {
            HealthRecordInfo selfRecord = null;

            foreach (HealthRecordInfo authRecord in AuthorizedRecords.Values)
            {
                if (
                    authRecord.RelationshipType == RelationshipType.Self && 
                    (authRecord.DateAuthorizationExpires == null || authRecord.DateAuthorizationExpires.Value > SystemClock.Instance.GetCurrentInstant()))
                {
                    selfRecord = authRecord;
                    break;
                }
            }

            if (selfRecord == null)
            {
                throw new HealthRecordNotFoundException(Resources.SelfRecordNotFound);
            }

            return selfRecord;
        }
    }
}
