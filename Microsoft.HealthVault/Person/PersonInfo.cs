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
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Person
{
    /// <summary>
    /// Provides information about a person's HealthVault account.
    /// </summary>
    ///
    public class PersonInfo : IMarshallable
    {
        private bool moreRecords;  // AuthorizedRecords collection does not contain the full set of records...
        private bool moreAppSettings;  // ApplicationSettings does not contain the app settings xml...

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
            this.personId = personInfo.personId;
            this.Name = personInfo.Name;
            this.selectedRecordId = personInfo.selectedRecordId;
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
            this.personId = new Guid(navigator.SelectSingleNode("person-id").Value);

            this.Name = navigator.SelectSingleNode("name").Value;

            XPathNavigator navAppSettings = navigator.SelectSingleNode("app-settings");

            if (navAppSettings != null)
            {
                XDocument doc = SDKHelper.SafeLoadXml(navAppSettings.OuterXml);
                this.ApplicationSettingsDocument = doc;
            }

            XPathNavigator navSelectedRecordId =
                navigator.SelectSingleNode("selected-record-id");

            if (navSelectedRecordId != null)
            {
                this.selectedRecordId = new Guid(navSelectedRecordId.Value);
            }

            XPathNavigator navPreferredCulture =
                navigator.SelectSingleNode("preferred-culture[language != '']");
            if (navPreferredCulture != null)
            {
                this.PreferredCulture = null;
                XPathNavigator navLanguageCode = navPreferredCulture.SelectSingleNode("language");
                {
                    this.PreferredCulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredCulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        this.PreferredCulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator navPreferredUICulture =
                navigator.SelectSingleNode("preferred-uiculture[language != '']");
            if (navPreferredUICulture != null)
            {
                this.PreferredUICulture = null;
                XPathNavigator navLanguageCode = navPreferredUICulture.SelectSingleNode("language");
                {
                    this.PreferredUICulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredUICulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        this.PreferredUICulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator locationNav = navigator.SelectSingleNode("location");
            if (locationNav != null)
            {
                this.Location = new Location();
                this.Location.ParseXml(locationNav);
            }

            XPathNodeIterator recordsNav = navigator.Select("record");
            foreach (XPathNavigator recordNav in recordsNav)
            {
                // Now see if we can fill in the record information
                HealthRecordInfo record = HealthRecordInfo.CreateFromXml(recordNav);
                if (record != null)
                {
                    this.AuthorizedRecords.Add(record.Id, record);
                }
            }

            XPathNavigator navMoreRecords = navigator.SelectSingleNode("more-records");
            if (navMoreRecords != null)
            {
                this.moreRecords = true;
            }

            XPathNavigator navMoreAppSettings = navigator.SelectSingleNode("more-app-settings");
            if (navMoreAppSettings != null)
            {
                this.moreAppSettings = true;
            }
        }

        /// <summary>
        /// Populates the data of the class from the XML in
        /// the specified reader.
        /// </summary>
        ///
        /// <param name="reader">
        /// The reader to get the data for the class instance
        /// from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="reader"/> is <b>null</b>.
        /// </exception>
        ///
        public void Unmarshal(XmlReader reader)
        {
            Validator.ThrowIfArgumentNull(reader, nameof(reader), Resources.XmlNullReader);

            this.ParseXml(new XPathDocument(reader).CreateNavigator());
        }

        /// <summary>
        /// Writes the person information into the specified writer as XML.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XMLWriter receiving the XML.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public void Marshal(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            this.WriteXml("person-info", writer, MarshalOptions.Default);
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
            return this.GetXml(MarshalOptions.Default);
        }

        /// <summary>
        /// Gets the XML representation of the <see cref="PersonInfo"/>
        /// that is appropriate for storing.
        /// </summary>
        /// 
        /// <returns>
        /// A XML string containing the person information.
        /// </returns>
        ///
        internal string GetXml(MarshalOptions marshalOptions)
        {
            StringBuilder personInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(personInfoXml, settings))
            {
                this.WriteXml("person-info", writer, marshalOptions);
                writer.Flush();
            }

            return personInfoXml.ToString();
        }

        private void WriteXml(string nodeName, XmlWriter writer, MarshalOptions marshalOptions)
        {
            bool writeContainingNode = false;
            if (!string.IsNullOrEmpty(nodeName))
            {
                writeContainingNode = true;
                writer.WriteStartElement(nodeName);
            }

            writer.WriteElementString("person-id", this.personId.ToString());
            writer.WriteElementString("name", this.Name);

            if (this.ApplicationSettingsDocument != null)
            {
                if ((marshalOptions & MarshalOptions.MinimizeApplicationSettings) == 0)
                {
                    writer.WriteRaw(this.ApplicationSettingsDocument.CreateNavigator().OuterXml);
                }
                else
                {
                    writer.WriteElementString("more-app-settings", string.Empty);
                }
            }
            else if (this.moreAppSettings)
            {
                writer.WriteElementString("more-app-settings", string.Empty);
            }

            if (this.selectedRecordId != Guid.Empty)
            {
                writer.WriteElementString(
                    "selected-record-id",
                    this.selectedRecordId.ToString());
            }
            
            // If we are removing records because they make the serialized xml too big, we remove all except
            // the currently-selected record...
            bool skippedRecords = false;
            foreach (HealthRecordInfo record in this.AuthorizedRecords.Values)
            {
                if ((marshalOptions & MarshalOptions.MinimizeRecords) == 0)
                {
                    record.WriteXml("record", writer);
                }
                else
                {
                    if (this.selectedRecordId != Guid.Empty &&
                        record.Id == this.selectedRecordId)
                    {
                        record.WriteXml("record", writer);
                    }
                    else
                    {
                        skippedRecords = true;
                    }
                }
            }

            if (skippedRecords || this.moreRecords)
            {
                writer.WriteElementString("more-records", string.Empty);
            }

            if (!string.IsNullOrEmpty(this.PreferredCulture))
            {
                WriteCulture("preferred-culture", writer, this.PreferredCulture);
            }

            if (!string.IsNullOrEmpty(this.PreferredUICulture))
            {
                WriteCulture("preferred-uiculture", writer, this.PreferredUICulture);
            }

            this.Location?.WriteXml(writer, "location");

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
                return this.personId;
            }

            set { this.personId = value; }
        }

        private Guid personId;

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
        /// <remarks>
        /// This property should only be used for testing.
        /// </remarks>
        protected XDocument ApplicationSettingsDocument { get; set; }

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
                if (this.selectedRecord == null)
                {
                    if (this.selectedRecordId != Guid.Empty &&
                        this.AuthorizedRecords.ContainsKey(this.selectedRecordId))
                    {
                        this.selectedRecord = this.AuthorizedRecords[this.selectedRecordId];
                    }
                }

                return this.selectedRecord;
            }

            set
            {
                this.selectedRecord = value;

                if (value != null &&
                    value.Id != Guid.Empty)
                {
                    this.selectedRecordId = value.Id;
                }

                this.SelectedRecordChanged?.Invoke(this, new EventArgs());
            }
        }

        private HealthRecordInfo selectedRecord;
        private Guid selectedRecordId;

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
        public IDictionary<Guid, HealthRecordInfo> AuthorizedRecords { get; } = new Dictionary<Guid, HealthRecordInfo>();

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

            foreach (HealthRecordInfo authRecord in this.AuthorizedRecords.Values)
            {
                if (authRecord.RelationshipType == RelationshipType.Self
                    && authRecord.DateAuthorizationExpires > DateTime.UtcNow)
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

        [Flags]
        internal enum MarshalOptions
        {
            Default = 0,
            MinimizeRecords = 1,
            MinimizeApplicationSettings = 2
        }
    }
}
