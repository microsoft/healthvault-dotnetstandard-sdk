// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.Authentication;
using Microsoft.Health.PlatformPrimitives;

namespace Microsoft.Health
{
    /// <summary>
    /// Provides information about a person's HealthVault account.
    /// </summary>
    /// 
    public class PersonInfo : IMarshallable
    {
        private bool _moreRecords;  // AuthorizedRecords collection does not contain the full set of records...
        private bool _moreAppSettings;  // ApplicationSettings does not contain the app settings xml...

        /// <summary>
        /// Creates a new instance of the PersonInfo class using
        /// the specified XML.
        /// </summary>
        /// 
        /// <param name="connection">
        /// An <see cref="ApplicationConnection"/> for the current user. The 
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
            ApplicationConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

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
        /// An <see cref="ApplicationConnection"/> for the current user. The 
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
        internal static PersonInfo CreateFromXmlExcludeUrl(
            ApplicationConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            PersonInfo personInfo = new PersonInfo(connection);
            personInfo.ParseXmlExcludeUrl(navigator);
            return personInfo;
        }

        /// <summary>
        /// Look up the person and record that were
        /// previously associated with this alternate id.
        /// </summary>
        /// 
        /// <remarks>
        /// To obtain the record info only, use <see cref="HealthRecordInfo.GetFromAlternateId"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// A new instance of <see cref="PersonInfo"/> containing information
        /// about the associated person and record.
        /// </returns>
        ///
        /// <param name="connection">The application connection to use.</param>
        /// <param name="alternateId">The alternateId to look up.</param>
        /// <returns>A <see cref="PersonInfo"/> with information 
        /// about the person and record.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The connection, alternateId parameters are null.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The alternateId parameter is empty, all whitespace, or more than 255 characters in length.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// If the alternate Id is not associated with a person and record id, the ErrorCode property
        /// will be set to AlternateIdNotFound.
        /// </exception>
        /// 
        public static PersonInfo GetFromAlternateId(
            ApplicationConnection connection,
            string alternateId)
        {
            return HealthVaultPlatform.GetPersonAndRecordForAlternateId(connection, alternateId);
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
            _connection = personInfo._connection;
            _personId = personInfo._personId;
            _name = personInfo._name;
            _selectedRecordId = personInfo._selectedRecordId;
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
        internal PersonInfo(ApplicationConnection connection)
        {
            _connection = connection;
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
        /// The <see cref="Connection"/> is <b>null</b> and the XML does not contain
        /// connection information.
        /// </exception>
        /// 
        internal virtual void ParseXml(XPathNavigator navigator)
        {
            ParseXml(navigator, true);
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
        /// The <see cref="Connection"/> is <b>null</b>, and the XML does not contain
        /// valid connection information.
        /// </exception>
        /// 
        internal void ParseXmlExcludeUrl(XPathNavigator navigator)
        {
            ParseXml(navigator, false);
        }

        private void ParseXml(XPathNavigator navigator, bool includeUrl)
        {
            _personId = new Guid(navigator.SelectSingleNode("person-id").Value);

            _name = navigator.SelectSingleNode("name").Value;

            XPathNavigator navAppSettings = navigator.SelectSingleNode("app-settings");

            if (navAppSettings != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.SafeLoadXml(navAppSettings.OuterXml);

                _appSettings = doc;
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
                _preferredCulture = null;
                XPathNavigator navLanguageCode = navPreferredCulture.SelectSingleNode("language");
                {
                    _preferredCulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredCulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        _preferredCulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator navPreferredUICulture =
                navigator.SelectSingleNode("preferred-uiculture[language != '']");
            if (navPreferredUICulture != null)
            {
                _preferredUICulture = null;
                XPathNavigator navLanguageCode = navPreferredUICulture.SelectSingleNode("language");
                {
                    _preferredUICulture = navLanguageCode.Value;

                    // Country code only matters if the language code is present.
                    XPathNavigator navCountryCode = navPreferredUICulture.SelectSingleNode("country");
                    if (navCountryCode != null)
                    {
                        _preferredUICulture += "-" + navCountryCode.Value;
                    }
                }
            }

            XPathNavigator locationNav = navigator.SelectSingleNode("location");
            if (locationNav != null)
            {
                Location = new Location();
                Location.ParseXml(locationNav);
            }

            XPathNavigator connectionNav =
                navigator.SelectSingleNode("connection");
            if (connectionNav != null)
            {
                Guid appId = new Guid(connectionNav.SelectSingleNode("app-id").Value);

                var instanceIdNav = connectionNav.SelectSingleNode("instance-id");
                HealthServiceInstance serviceInstance = null;
                if (instanceIdNav != null)
                {
                    string instanceId = instanceIdNav.Value;

                    if (!ServiceInfo.Current.ServiceInstances.ContainsKey(instanceId))
                    {
                        throw Validator.InvalidOperationException("InstanceIdNotFound");
                    }

                    serviceInstance = ServiceInfo.Current.ServiceInstances[instanceId];
                }

                Uri healthServiceUri = null;
                if (includeUrl)
                {
                    healthServiceUri =
                        new Uri(connectionNav.SelectSingleNode("wildcat-url").Value);
                }
                else if (serviceInstance == null)
                {
                    healthServiceUri =
                        HealthApplicationConfiguration.Current.HealthVaultMethodUrl;
                }

                XPathNavigator credNav = connectionNav.SelectSingleNode("credential");

                if (credNav != null)
                {
                    Credential cred = Credential.CreateFromCookieXml(credNav);

                    _connection =
                        serviceInstance != null
                        ? new AuthenticatedConnection(appId, serviceInstance, cred)
                        : new AuthenticatedConnection(appId, healthServiceUri, cred);
                }

                XPathNavigator compressionNode =
                    connectionNav.SelectSingleNode("request-compression-method");

                if (compressionNode != null)
                {
                    _connection.RequestCompressionMethod = compressionNode.Value;
                }
            }
            else
            {
                Validator.ThrowInvalidIfNull(_connection, "PersonInfoConnectionNull");
            }

            XPathNodeIterator recordsNav = navigator.Select("record");
            foreach (XPathNavigator recordNav in recordsNav)
            {
                // Now see if we can fill in the record information
                HealthRecordInfo record = HealthRecordInfo.CreateFromXml(
                                                ApplicationConnection, recordNav);

                if (record != null)
                {
                    _authorizedRecords.Add(record.Id, record);
                }
            }

            XPathNavigator navMoreRecords = navigator.SelectSingleNode("more-records");
            if (navMoreRecords != null)
            {
                _moreRecords = true;
            }

            XPathNavigator navMoreAppSettings = navigator.SelectSingleNode("more-app-settings");
            if (navMoreAppSettings != null)
            {
                _moreAppSettings = true;
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

            ParseXml(new XPathDocument(reader).CreateNavigator());
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
            return GetXml(CookieOptions.IncludeUrl);
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
            return GetXml(cookieOptions);
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

            WriteXml("person-info", writer, CookieOptions.IncludeUrl);
        }

        [Flags]
        internal enum CookieOptions
        {
            Default = 0,
            MinimizeRecords = 1,
            MinimizeApplicationSettings = 2,
            IncludeUrl = 4,
        }

        private string GetXml(CookieOptions cookieOptions)
        {
            StringBuilder personInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(personInfoXml, settings))
            {
                WriteXml("person-info", writer, cookieOptions);
                writer.Flush();
            }
            return personInfoXml.ToString();
        }

        private void WriteXml(string nodeName, XmlWriter writer, CookieOptions cookieOptions)
        {
            bool writeContainingNode = false;
            if (!String.IsNullOrEmpty(nodeName))
            {
                writeContainingNode = true;
                writer.WriteStartElement(nodeName);
            }

            writer.WriteElementString("person-id", _personId.ToString());
            writer.WriteElementString("name", _name);

            if (_appSettings != null)
            {
                if ((cookieOptions & CookieOptions.MinimizeApplicationSettings) == 0)
                {
                    writer.WriteRaw(_appSettings.CreateNavigator().OuterXml);
                }
                else
                {
                    writer.WriteElementString("more-app-settings", String.Empty);
                }
            }
            else if (_moreAppSettings)
            {
                writer.WriteElementString("more-app-settings", String.Empty);
            }

            if (_selectedRecordId != Guid.Empty)
            {
                writer.WriteElementString(
                    "selected-record-id",
                    _selectedRecordId.ToString());
            }

            writer.WriteStartElement("connection");

            writer.WriteElementString(
                "app-id",
                _connection.ApplicationId.ToString());

            if (_connection.ServiceInstance != null)
            {
                writer.WriteElementString(
                    "instance-id",
                    _connection.ServiceInstance.Id);
            }

            if ((cookieOptions & CookieOptions.IncludeUrl) != 0)
            {
                writer.WriteElementString(
                    "wildcat-url",
                    _connection.RequestUrl.ToString());
            }

            AuthenticatedConnection authConnection = _connection as AuthenticatedConnection;
            if (authConnection != null && authConnection.Credential != null)
            {
                writer.WriteStartElement("credential");
                authConnection.Credential.WriteCookieXml(writer);
                writer.WriteEndElement();
            }

            if (!String.IsNullOrEmpty(_connection.RequestCompressionMethod))
            {
                writer.WriteElementString(
                    "request-compression-method",
                    _connection.RequestCompressionMethod);
            }

            writer.WriteEndElement();

            // If we are removing records because they make the cookie too big, we remove all except
            // the currently-selected record...
            bool skippedRecords = false;
            foreach (HealthRecordInfo record in AuthorizedRecords.Values)
            {
                if ((cookieOptions & CookieOptions.MinimizeRecords) == 0)
                {
                    record.WriteXml("record", writer);
                }
                else
                {
                    if (_selectedRecordId != Guid.Empty &&
                        record.Id == _selectedRecordId)
                    {
                        record.WriteXml("record", writer);
                    }
                    else
                    {
                        skippedRecords = true;
                    }
                }
            }

            if (skippedRecords || _moreRecords)
            {
                writer.WriteElementString("more-records", String.Empty);
            }

            if (!String.IsNullOrEmpty(_preferredCulture))
            {
                WriteCulture("preferred-culture", writer, _preferredCulture);
            }

            if (!String.IsNullOrEmpty(_preferredUICulture))
            {
                WriteCulture("preferred-uiculture", writer, _preferredUICulture);
            }

            if (Location != null)
            {
                Location.WriteXml(writer, "location");
            }

            if (writeContainingNode)
            {
                writer.WriteEndElement();
            }
        }

        private static void WriteCulture(
            String elementName,
            XmlWriter writer,
            String culture)
        {
            if (!String.IsNullOrEmpty(culture))
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
        /// <remarks>
        /// </remarks>
        /// 
        public Guid PersonId
        {
            get
            {
                return _personId;
            }
            protected set { _personId = value; }
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
        public string Name
        {
            get
            {
                return _name;
            }
            protected set { _name = value; }
        }
        private string _name;

        private void FetchApplicationSettingsAndAuthorizedRecords()
        {
            PersonInfo personInfo = HealthVaultPlatform.GetPersonInfo(_connection);
            _appSettings = personInfo._appSettings;
            _authorizedRecords = personInfo._authorizedRecords;

            _moreAppSettings = false;
            _moreRecords = false;
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
        public IXPathNavigable ApplicationSettings
        {
            get
            {
                if (_moreAppSettings)
                {
                    FetchApplicationSettingsAndAuthorizedRecords();
                }

                return _appSettings;
            }
        }
        private XmlDocument _appSettings;

        /// <summary>
        /// Gets or sets the underlying application settings document.
        /// </summary>
        /// <remarks>
        /// This property should only be used for testing. 
        /// </remarks>
        protected XmlDocument ApplicationSettingsDocument
        {
            get { return _appSettings; }
            set { _appSettings = value; }
        }

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
        public void SetApplicationSettings(IXPathNavigable applicationSettings)
        {
            string requestParameters
                = HealthVaultPlatformPerson.GetSetApplicationSettingsParameters(applicationSettings);
            HealthVaultPlatformPerson.Current.SetApplicationSettings(ApplicationConnection, requestParameters);
            _appSettings = new XmlDocument();
            _appSettings.XmlResolver = null;
            _appSettings.SafeLoadXml(requestParameters);

            if (ApplicationSettingsChanged != null)
            {
                EventArgs e = new EventArgs();
                ApplicationSettingsChanged(this, e);
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
                if (_selectedRecord == null)
                {
                    if (_selectedRecordId != Guid.Empty &&
                        _authorizedRecords.ContainsKey(_selectedRecordId))
                    {
                        _selectedRecord = _authorizedRecords[_selectedRecordId];
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

                if (SelectedRecordChanged != null)
                {
                    SelectedRecordChanged(this, new EventArgs());
                }
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
        /// using <see cref="Microsoft.Health.ApplicationConnection.GetAuthorizedRecords(IList{Guid})"/> by
        /// ID.
        /// </remarks>
        /// 
        public Dictionary<Guid, HealthRecordInfo> AuthorizedRecords
        {
            get
            {
                if (_moreRecords)
                {
                    FetchApplicationSettingsAndAuthorizedRecords();
                }
                return _authorizedRecords;
            }
            protected set { _authorizedRecords = value; }
        }
        private Dictionary<Guid, HealthRecordInfo> _authorizedRecords =
            new Dictionary<Guid, HealthRecordInfo>();

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        /// 
        /// <remarks>
        /// The preferred culture should be used when formatting date/time, numbers, collating, etc.
        /// </remarks>
        /// 
        public string PreferredCulture
        {
            get { return _preferredCulture; }
            protected set { _preferredCulture = value; }
        }
        private string _preferredCulture;

        /// <summary>
        /// Gets or sets the user's preferred UI culture.
        /// </summary>
        /// 
        /// <remarks>
        /// The preferred UI culture should be used when retrieving text from resources for display
        /// to the user.
        /// </remarks>
        /// 
        public string PreferredUICulture
        {
            get { return _preferredUICulture; }
            protected set { _preferredUICulture = value; }
        }
        private string _preferredUICulture;

        /// <summary>
        /// Gets the location of the user account.
        /// </summary>
        /// 
        public Location Location { get; private set; }

        #endregion public properties

        /// <summary>
        /// Gets a reference to the HealthVault service that
        /// created this <see cref="PersonInfo"/> or null if the connection used was an
        /// <see cref="Microsoft.Health.Web.OfflineWebApplicationConnection"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This may return null if the <see cref="PersonInfo"/> was retrieved using an
        /// <see cref="Microsoft.Health.ApplicationConnection"/>.
        /// It is preferred that <see cref="ApplicationConnection"/> is used instead.
        /// </remarks>
        /// 
        public AuthenticatedConnection Connection
        {
            get { return _connection as AuthenticatedConnection; }
        }
        private ApplicationConnection _connection;

        /// <summary>
        /// Gets a reference to the HealthVault connection instance that was used to create this
        /// <see cref="PersonInfo"/>.
        /// </summary>
        /// 
        public ApplicationConnection ApplicationConnection
        {
            get { return _connection; }
        }

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
                if (authRecord.RelationshipType == RelationshipType.Self
                    && authRecord.DateAuthorizationExpires > DateTime.UtcNow)
                {
                    selfRecord = authRecord;
                    break;
                }
            }

            if (selfRecord == null)
            {
                HealthServiceResponseError error = new HealthServiceResponseError();
                error.Message =
                    ResourceRetriever.GetResourceString(
                        "SelfRecordNotFound");

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
