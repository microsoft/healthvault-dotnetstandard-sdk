// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Authentication;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Extensions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.PlatformInformation;
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
        /// <param name="connection">
        /// An <see cref="Connection"/> for the current user. The
        /// connection can be optionally supplied, but it is overwritten if
        /// the connection information is in the XML.
        /// </param>
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
        /// The <paramref name="navigator"/> parameter is <b>null</b> or
        /// <paramref name="connection"/> is <b>null</b> and the XML does not
        /// contain the connection information.
        /// </exception>
        ///
        public static PersonInfo CreateFromXml(
            IConnectionInternal connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            if (connection == null)
            {
                throw new ArgumentException("Connection argument is expected", nameof(connection));
            }

            PersonInfo personInfo = new PersonInfo(connection);
            personInfo.ParseXml(navigator);
            return personInfo;
        }

        /// <summary>
        /// Creates a new instance of the PersonInfo class using
        /// the specified XML.
        /// </summary>
        ///
        /// <param name="connection">
        /// An <see cref="Connection"/> for the current user. The
        /// connection can be optionally supplied, but it is overwritten if
        /// the connection information is in the XML.
        /// </param>
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
        /// The <paramref name="navigator"/> parameter is <b>null</b> or
        /// <paramref name="connection"/> is <b>null</b> and the XML does not
        /// contain the connection information.
        /// </exception>
        ///
        internal static async Task<PersonInfo> CreateFromXmlExcludeUrl(
            IConnectionInternal connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            PersonInfo personInfo = new PersonInfo(connection);
            personInfo.ParseXmlExcludeUrl(navigator);

            await personInfo.SetServiceInfoConnectionAsync().ConfigureAwait(false);

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
            this.Connection = personInfo.Connection;
            this.personId = personInfo.personId;
            this.Name = personInfo.Name;
            this.selectedRecordId = personInfo.selectedRecordId;
        }

        /// <summary>
        /// Constructs an empty <see cref="PersonInfo"/> object which can be used to
        /// deserialize person info XML.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection the <see cref="PersonInfo"/> object should use for operations
        /// once it has been deserialized.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        internal PersonInfo(IConnectionInternal connection)
        {
            this.Connection = connection;
        }

        /// <summary>
        /// Constructs an new instance of the <see cref="PersonInfo"/> class for testing purposes.
        /// </summary>
        ///
        protected PersonInfo()
        {
        }

        /// <summary>
        /// Populates the class members with data from the specified
        /// person information XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the person information from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <see cref="HealthVault.Connection"/> is <b>null</b> and the XML does not contain
        /// connection information.
        /// </exception>
        ///
        internal virtual void ParseXml(XPathNavigator navigator)
        {
            this.ParseXml(navigator, true);
        }

        /// <summary>
        /// Populates the class members with data from the specified
        /// person information XML excluding the embedded platform url.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the person information from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The <see cref="HealthVault.Connection"/> is <b>null</b>, and the XML does not contain
        /// valid connection information.
        /// </exception>
        ///
        internal void ParseXmlExcludeUrl(XPathNavigator navigator)
        {
            this.ParseXml(navigator, false);
        }

        private void ParseXml(XPathNavigator navigator, bool includeUrl)
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

            XPathNavigator connectionNav =
                navigator.SelectSingleNode("connection");
            if (connectionNav != null)
            {
                IConnectionInternal connection = Ioc.Get<IConnectionInternal>();

                this.appId = new Guid(connectionNav.SelectSingleNode("app-id").Value);

                var instanceIdNav = connectionNav.SelectSingleNode("instance-id");
                if (instanceIdNav != null)
                {
                    this.instanceId = instanceIdNav.Value;
                }

                if (includeUrl)
                {
                    this.healthServiceUri =
                        new Uri(connectionNav.SelectSingleNode("wildcat-url").Value);
                }

                XPathNavigator credNav = connectionNav.SelectSingleNode("credential");

                if (credNav != null)
                {
                    connection.SetSessionCredentialFromCookieXml(credNav);
                    this.credential = connection.SessionCredential;
                }

                XPathNavigator compressionNode =
                    connectionNav.SelectSingleNode("request-compression-method");

                if (compressionNode != null)
                {
                    this.compressionMethod = compressionNode.Value;
                }
            }
            else
            {
                Validator.ThrowInvalidIfNull(this.Connection, "PersonInfoConnectionNull");
            }

            XPathNodeIterator recordsNav = navigator.Select("record");
            foreach (XPathNavigator recordNav in recordsNav)
            {
                // Now see if we can fill in the record information
                HealthRecordInfo record = HealthRecordInfo.CreateFromXml(recordNav);
                if (record != null)
                {
                    this.authorizedRecords.Add(record.Id, record);
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

        private async Task SetServiceInfoConnectionAsync()
        {
            if (!string.IsNullOrEmpty(this.instanceId))
            {
                ServiceInfo info = new ServiceInfo();

                var serviceInfo = await info.GetServiceInfoAsync();

                HealthServiceInstance serviceInstance = serviceInfo.ServiceInstances[this.instanceId];

                // connection.DoDirtyWork(this.appId, serviceInstance, this.credential);

                // TODO: IConnection-ify this.
                // this.ApplicationConnection = new AuthenticatedConnection(this.appId, serviceInstance, this.credential);
            }
            else
            {
                this.healthServiceUri = HealthApplicationConfiguration.Current.GetHealthVaultMethodUrl();

                // TODO: IConnection-ify this.
                // this.ApplicationConnection = new AuthenticatedConnection(this.appId, this.healthServiceUri, this.credential);
            }

            if (!string.IsNullOrEmpty(this.compressionMethod))
            {
                // TODO: IConnection-ify this.
                // this.ApplicationConnection.RequestCompressionMethod = this.compressionMethod;
            }

            if (this.authorizedRecords != null)
            {
                foreach (var authorizedRecord in this.authorizedRecords)
                {
                    authorizedRecord.Value.Connection = this.Connection;
                }
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
            Validator.ThrowIfArgumentNull(reader, "reader", "XmlNullReader");

            this.ParseXml(new XPathDocument(reader).CreateNavigator());
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
            return this.GetXml(CookieOptions.IncludeUrl);
        }

        /// <summary>
        /// Gets the XML representation of the <see cref="PersonInfo"/>
        /// that is appropriate for storing in the cookie.
        /// </summary>
        ///
        /// <returns>
        /// A XML string containing the person information.
        /// </returns>
        ///
        /// <remarks>
        /// Embedded url will be overridden with the configuration value.
        /// </remarks>
        ///
        internal string GetXmlForCookie(CookieOptions cookieOptions)
        {
            return this.GetXml(cookieOptions);
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

            this.WriteXml("person-info", writer, CookieOptions.IncludeUrl);
        }

        [Flags]
        internal enum CookieOptions
        {
            Default = 0,
            MinimizeRecords = 1,
            MinimizeApplicationSettings = 2,
            IncludeUrl = 4
        }

        private string GetXml(CookieOptions cookieOptions)
        {
            StringBuilder personInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(personInfoXml, settings))
            {
                this.WriteXml("person-info", writer, cookieOptions);
                writer.Flush();
            }

            return personInfoXml.ToString();
        }

        private void WriteXml(string nodeName, XmlWriter writer, CookieOptions cookieOptions)
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
                if ((cookieOptions & CookieOptions.MinimizeApplicationSettings) == 0)
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

            writer.WriteStartElement("connection");

            writer.WriteElementString(
                "app-id",
                this.Connection.ApplicationConfiguration.ApplicationId.ToString());

            if (this.Connection.ServiceInstance != null)
            {
                writer.WriteElementString(
                    "instance-id",
                    this.Connection.ServiceInstance.Id);
            }

            if ((cookieOptions & CookieOptions.IncludeUrl) != 0)
            {
                writer.WriteElementString(
                    "wildcat-url",
                    this.Connection.ApplicationConfiguration.HealthVaultUrl.ToString());
            }

            if (this.Connection?.SessionCredential != null)
            {
                writer.WriteStartElement("credential");

                this.Connection.StoreSessionCredentialInCookieXml(writer);
                writer.WriteEndElement();
            }

            // TODO: comment this for now
            // if (!string.IsNullOrEmpty(this.Connection.RequestCompressionMethod))
            // {
            //    writer.WriteElementString(
            //        "request-compression-method",
            //        this.Connection.RequestCompressionMethod);
            // }

            writer.WriteEndElement();

            // If we are removing records because they make the cookie too big, we remove all except
            // the currently-selected record...
            bool skippedRecords = false;
            foreach (HealthRecordInfo record in this.authorizedRecords.Values)
            {
                if ((cookieOptions & CookieOptions.MinimizeRecords) == 0)
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

            protected set { this.personId = value; }
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
        public string Name { get; protected set; }

        private async Task FetchApplicationSettingsAndAuthorizedRecordsAsync()
        {
            PersonInfo personInfo = await HealthVaultPlatform.GetPersonInfoAsync(this.Connection);
            this.ApplicationSettingsDocument = personInfo.ApplicationSettingsDocument;
            this.authorizedRecords = personInfo.authorizedRecords;

            this.moreAppSettings = false;
            this.moreRecords = false;
        }

        /// <summary>
        /// Gets or sets the application settings for the current application and
        /// person.
        /// </summary>
        ///
        /// <remarks>
        /// This can be <b>null</b> if no application settings have been stored
        /// for the application or user.
        /// </remarks>
        ///
        public async Task<IXPathNavigable> GetApplicationSettings()
        {
            if (this.moreAppSettings)
            {
                await this.FetchApplicationSettingsAndAuthorizedRecordsAsync().ConfigureAwait(false);
            }

            return this.ApplicationSettingsDocument.CreateNavigator();
        }

        /// <summary>
        /// Gets or sets the underlying application settings document.
        /// </summary>
        /// <remarks>
        /// This property should only be used for testing.
        /// </remarks>
        protected XDocument ApplicationSettingsDocument { get; set; }

        /// <summary>
        /// Sets the application settings in the web service for this person.
        /// </summary>
        ///
        /// <param name="applicationSettings">
        /// The application specific settings for this person.
        /// </param>
        ///
        /// <remarks>
        /// This method makes a network call to the web service.
        /// <br/><br/>
        /// The XML provided by <paramref name="applicationSettings"/> must
        /// have the outer node "&lt;app-settings&gt;" or the request will
        /// fail.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// An error is returned from the server when making the request.
        /// </exception>
        public async Task SetApplicationSettings(IXPathNavigable applicationSettings)
        {
            string requestParameters
                = HealthVaultPlatformPerson.GetSetApplicationSettingsParameters(applicationSettings);

            await HealthVaultPlatformPerson
                .Current
                .SetApplicationSettingsAsync(this.Connection, requestParameters)
                .ConfigureAwait(false);

            this.ApplicationSettingsDocument = SDKHelper.SafeLoadXml(requestParameters);

            if (this.ApplicationSettingsChanged != null)
            {
                EventArgs e = new EventArgs();
                this.ApplicationSettingsChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when <see cref="SetApplicationSettings"/> changes the application settings in
        /// HealthVault.
        /// </summary>
        ///
        /// <remarks>
        /// This event is not triggered if another instance of the PersonInfo gets updated or
        /// if the value changes in HealthVault.
        /// The sender is the PersonInfo instance that was updated.
        /// The event args are empty.
        /// </remarks>
        ///
        public event EventHandler ApplicationSettingsChanged;

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
                        this.authorizedRecords.ContainsKey(this.selectedRecordId))
                    {
                        this.selectedRecord = this.authorizedRecords[this.selectedRecordId];
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
        /// Gets or sets the authorized record for the person.
        /// </summary>
        ///
        /// <value>
        /// The records that the person is authorized to access.
        /// </value>
        ///
        /// <remarks>
        /// A person can access their own health record,
        /// health records that they have created for other people, or
        /// health records that other people have shared with them. The
        /// contents of this collection will be all the health records,
        /// including those that have been deleted or suspended. A person can
        /// interact only with active records.
        /// <br/><br/>
        /// Shortcuts are provided to get access to the person's own
        /// health record using <see cref="GetSelfRecord"/> and specific records
        /// using <see cref="HealthVault.Connection.ApplicationConnection.GetAuthorizedRecords(System.Collections.Generic.IList{System.Guid})"/> by
        /// ID.
        /// </remarks>
        ///
        public async Task<Dictionary<Guid, HealthRecordInfo>> GetAuthorizedRecordsAsync()
        {
            if (this.moreRecords)
            {
                await this.FetchApplicationSettingsAndAuthorizedRecordsAsync().ConfigureAwait(false);
            }

            return this.authorizedRecords;
        }

        private Dictionary<Guid, HealthRecordInfo> authorizedRecords =
            new Dictionary<Guid, HealthRecordInfo>();

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        ///
        /// <remarks>
        /// The preferred culture should be used when formatting date/time, numbers, collating, etc.
        /// </remarks>
        ///
        public string PreferredCulture { get; protected set; }

        /// <summary>
        /// Gets or sets the user's preferred UI culture.
        /// </summary>
        ///
        /// <remarks>
        /// The preferred UI culture should be used when retrieving text from resources for display
        /// to the user.
        /// </remarks>
        ///
        public string PreferredUICulture { get; protected set; }

        /// <summary>
        /// Gets the location of the user account.
        /// </summary>
        ///
        public Location Location { get; private set; }

        private string instanceId;
        private SessionCredential credential;
        private Guid appId;
        private Uri healthServiceUri;
        private string compressionMethod;

        #endregion public properties

        /// <summary>
        /// Gets a reference to the HealthVault connection instance that was used to create this
        /// <see cref="PersonInfo"/>.
        /// </summary>
        ///
        public IConnectionInternal Connection { get; private set; }

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

            foreach (HealthRecordInfo authRecord in this.authorizedRecords.Values)
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
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = ResourceRetriever.GetResourceString(
                        "SelfRecordNotFound")
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.RecordNotFound,
                        error);
                throw e;
            }

            return selfRecord;
        }
    }
}
