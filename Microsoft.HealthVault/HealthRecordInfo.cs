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
using Microsoft.HealthVault.Web;

namespace Microsoft.HealthVault
{

    /// <summary>
    /// Represents the APIs and information about a health record for an individual.
    /// </summary>
    /// 
    /// <remarks>
    /// A HealthRecordInfo represents a person's view of a health record and 
    /// information about the health record such as the state, name, date of
    /// expiration, and so on. This view may vary based upon the access rights the 
    /// person has to the record and multiple people may have access to the 
    /// same record but have different views. For instance, a husband may 
    /// have a HealthRecordInfo instance for himself and another for his 
    /// wife's health record which she shared with him.
    /// </remarks>
    /// 
    public class HealthRecordInfo : HealthRecordAccessor, IMarshallable
    {
        /// <summary>
        /// Creates an instance of a HealthRecordInfo object using
        /// the specified XML.
        /// </summary>
        /// 
        /// <param name="connection">
        /// A connection for the current user.
        /// </param>
        /// 
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        /// 
        /// <returns>
        /// A new instance of a HealthRecordInfo object populated with the
        /// record information.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> or <paramref name="navigator"/> 
        /// parameter is <b>null</b>.
        /// </exception>
        /// 
        public new static HealthRecordInfo CreateFromXml(
            ApplicationConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "PersonInfoConnectionNull");
            Validator.ThrowIfArgumentNull(navigator, "navigator", "ParseXmlNavNull");

            HealthRecordInfo recordInfo = new HealthRecordInfo(connection);
            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        /// <summary>
        /// Look up the record that were
        /// previously associated with this alternate id.
        /// </summary>
        /// 
        /// <remarks>
        /// To obtain the person and record info, use <see cref="PersonInfo.GetFromAlternateId"/>.
        /// </remarks>
        /// 
        /// <returns>
        /// A new instance of <see cref="HealthRecordInfo"/> that can be used to access the
        /// record.
        /// </returns>
        ///
        /// <param name="connection">The application connection to use.</param>
        /// <param name="alternateId">The alternateId to look up.</param>
        /// <returns>A HealthRecordInfo that can be used to access the record.</returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The alternateId parameter is null.
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
        /// /// 
        public static HealthRecordInfo GetFromAlternateId(
            ApplicationConnection connection,
            string alternateId)
        {
            PersonInfo personInfo = HealthVaultPlatform.GetPersonAndRecordForAlternateId(connection, alternateId);

            if (personInfo.AuthorizedRecords != null && personInfo.AuthorizedRecords.Count == 1)
            {
                List<HealthRecordInfo> infos = new List<HealthRecordInfo>(personInfo.AuthorizedRecords.Values);
                return infos[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parses HealthRecordInfo member data from the specified XPathNavigator.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        /// 
        internal override void ParseXml(XPathNavigator navigator)
        {
            base.ParseXml(navigator);

            _custodian = XPathHelper.ParseAttributeAsBoolean(navigator, "record-custodian", false);

            long? relationshipNumber = null;

            relationshipNumber = XPathHelper.ParseAttributeAsLong(navigator, "rel-type", 0);
            if (relationshipNumber.HasValue &&
               (relationshipNumber <= (int)RelationshipType.Daughter))
            {
                _relationshipType = (RelationshipType)relationshipNumber;
            }

            _relationshipName = navigator.GetAttribute("rel-name", String.Empty);

            _dateAuthorizationExpires = XPathHelper.ParseAttributeAsDateTime(navigator, "auth-expires", DateTime.MinValue);

            _authExpired = XPathHelper.ParseAttributeAsBoolean(navigator, "auth-expired", false);

            _name = navigator.Value;

            _displayName = navigator.GetAttribute("display-name", String.Empty);

            _state = XPathHelper.ParseAttributeAsEnum<HealthRecordState>(navigator, "state", HealthRecordState.Unknown);
            if (_state > HealthRecordState.Deleted)
            {
                _state = HealthRecordState.Unknown;
            }

            _dateCreated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-created", DateTime.MinValue);
            _dateUpdated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-updated", DateTime.MinValue);

            _quotaInBytes = XPathHelper.ParseAttributeAsLong(navigator, "max-size-bytes", null);
            _quotaUsedInBytes = XPathHelper.ParseAttributeAsLong(navigator, "size-bytes", null);

            _authorizationStatus = XPathHelper.ParseAttributeAsEnum<HealthRecordAuthorizationStatus>(
                    navigator,
                    "app-record-auth-action",
                    HealthRecordAuthorizationStatus.Unknown);

            _applicationSpecificRecordId = navigator.GetAttribute("app-specific-record-id", String.Empty);

            LatestOperationSequenceNumber = XPathHelper.ParseAttributeAsLong(navigator, "latest-operation-sequence-number", 0).Value;

            RecordAppAuthCreatedDate = XPathHelper.ParseAttributeAsDateTime(navigator, "record-app-auth-created-date", DateTime.MinValue);

            _updated = true;
        }

        /// <summary>
        /// Populates the data of the class from the XML in
        /// the specified reader.
        /// </summary>
        /// 
        /// <param name="reader">
        /// The reader from which to get the data for the class instance.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="reader"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public void Unmarshal(XmlReader reader)
        {
            Validator.ThrowIfArgumentNull(reader, "reader", "XmlNullReader");

            XPathDocument healthRecordInfoDoc = new XPathDocument(reader);
            ParseXml(healthRecordInfoDoc.CreateNavigator());
        }

        /// <summary>
        /// Gets the XML representation of the HealthRecordInfo.
        /// </summary>
        /// 
        /// <returns>
        /// A string containing the XML representation of the HealthRecordInfo.
        /// </returns>
        /// 
        public override string GetXml()
        {
            StringBuilder recordInfoXml = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(recordInfoXml, settings))
            {
                WriteXml("record", writer);
                writer.Flush();
            }
            return recordInfoXml.ToString();
        }

        /// <summary>
        /// Writes the record information into the specified writer as XML.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The writer that receives the record information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        public void Marshal(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            WriteXml("record", writer);
        }

        internal override void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);

            base.WriteXml(writer);

            writer.WriteAttributeString(
                "record-custodian",
                XmlConvert.ToString(_custodian));

            writer.WriteAttributeString(
                "rel-type",
                XmlConvert.ToString((int)_relationshipType));

            if (!String.IsNullOrEmpty(_relationshipName))
            {
                writer.WriteAttributeString(
                    "rel-name",
                    _relationshipName);
            }

            writer.WriteAttributeString(
                "auth-expires",
                SDKHelper.XmlFromDateTime(_dateAuthorizationExpires));

            writer.WriteAttributeString(
                "auth-expired",
                SDKHelper.XmlFromBool(_authExpired));

            if (!String.IsNullOrEmpty(_displayName))
            {
                writer.WriteAttributeString(
                    "display-name",
                    _displayName);
            }

            writer.WriteAttributeString(
                "state",
                _state.ToString());

            writer.WriteAttributeString(
                "date-created",
                SDKHelper.XmlFromDateTime(_dateCreated));

            if (_quotaInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "max-size-bytes",
                    XmlConvert.ToString(_quotaInBytes.Value));
            }

            if (_quotaUsedInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "size-bytes",
                    XmlConvert.ToString(_quotaUsedInBytes.Value));
            }

            writer.WriteAttributeString(
                "app-record-auth-action",
                _authorizationStatus.ToString());

            writer.WriteAttributeString(
                "app-specific-record-id",
                _applicationSpecificRecordId);

            writer.WriteAttributeString(
                "date-updated",
                SDKHelper.XmlFromDateTime(_dateUpdated));

            writer.WriteValue(_name);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// 
        /// <param name="recordInfo">
        /// The record info object which is to be used as the source 
        /// for the data.
        /// </param>
        /// 
        internal HealthRecordInfo(HealthRecordInfo recordInfo)
            : this(
                recordInfo.Connection as AuthenticatedConnection,
                recordInfo.Id)
        {
            if (recordInfo.IsUpdated)
            {
                _custodian = recordInfo.IsCustodian;
                _dateAuthorizationExpires = recordInfo.DateAuthorizationExpires;
                _name = recordInfo.Name;
                _relationshipType = recordInfo.RelationshipType;
                _relationshipName = recordInfo.RelationshipName;
                _displayName = recordInfo.DisplayName;

                if (recordInfo.Location != null)
                {
                    Location = new Location(recordInfo.Location.Country, recordInfo.Location.StateProvince);
                }

                _updated = true;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordInfo"/> class, 
        /// providing a new view of a personal health record.
        /// </summary>
        /// 
        /// <param name="connection">
        /// An instance of a <see cref="ApplicationConnection"/> 
        /// to which the record operations are directed.
        /// </param>
        /// 
        /// <param name="id">
        /// The unique identifier for the record.
        /// </param>
        /// 
        /// <remarks>
        /// With this constructor, none of the data held in the properties
        /// is valid except the 
        /// <see cref="HealthRecordAccessor.Id"/> 
        /// property. The ID is not validated with the service and the data
        /// is not retrieved until 
        /// <see cref="HealthRecordInfo.Refresh"/>
        /// is called. However, any of the methods can be called without 
        /// Update being called.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="id"/> parameter is Guid.Empty.
        /// </exception>
        /// 
        public HealthRecordInfo(
            ApplicationConnection connection,
            Guid id)
            : base(connection, id)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordInfo"/> class 
        /// for deserialization purposes.
        /// </summary>
        /// 
        /// <param name="connection">
        /// An instance of a <see cref="ApplicationConnection"/> 
        /// to which the record operations are directed.
        /// </param>
        /// 
        /// <remarks>
        /// This constructor is only useful if ParseXml is called.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        internal HealthRecordInfo(ApplicationConnection connection)
            : base(connection)
        {
        }

        #region Public properties

        /// <summary>
        /// Gets a value indicating whether or not the person is a custodian 
        /// of the record.
        /// </summary>
        /// 
        /// <value>
        /// <b>true</b> if the person is a custodian of the record; otherwise, 
        /// <b>false</b>.
        /// </value>
        /// 
        /// <remarks>
        /// A person is considered a custodian if they have been given 
        /// ownership of the record. The owner can give ownership to another 
        /// as an explicit action when sharing the record.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public bool IsCustodian
        {
            get
            {
                VerifyUpdated();
                return _custodian;
            }
            protected set
            {
                _custodian = value;
                _updated = true;
            }
        }
        private bool _custodian;

        /// <summary>
        /// Gets the date/time that the authorization for the record expires.
        /// </summary>
        /// 
        /// <value>
        /// A DateTime in UTC indicating when the record is no longer
        /// accessible to the user.
        /// </value>
        /// 
        /// <remarks>
        /// When a person shares their record with another HealthVault account,
        /// they can specify the date when that sharing is revoked (if ever).
        /// This property indicates that date. If the person tries to access
        /// the record after the indicated date, they receive a
        /// <see cref="HealthServiceAccessDeniedException"/>.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public DateTime DateAuthorizationExpires
        {
            get
            {
                VerifyUpdated();
                return _dateAuthorizationExpires;
            }
            protected set
            {
                _dateAuthorizationExpires = value;
                _updated = true;
            }
        }
        private DateTime _dateAuthorizationExpires;

        /// <summary>
        /// <b>true</b> if the authorization of the authenticated person has
        /// expired for this record; otherwise, <b>false</b>.
        /// </summary>
        /// 
        public bool HasAuthorizationExpired
        {
            get
            {
                VerifyUpdated();
                return _authExpired;
            }
            protected set
            {
                _authExpired = value;
                _updated = true;
            }
        }
        private bool _authExpired;

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        /// 
        /// <value>
        /// A string indicating the name of the record.
        /// </value>
        /// 
        /// <remarks>
        /// The name defaults to the name of the person to whom the record
        /// belongs. See <see cref="DisplayName"/> for how to override the 
        /// name to customize the view for a person authorized to view the 
        /// record.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public string Name
        {
            get
            {
                VerifyUpdated();
                return _name;
            }
            protected set
            {
                _name = value;
                _updated = true;
            }
        }
        private string _name;

        /// <summary>
        /// Gets the relationship the person authorized to view this record
        /// has with the "owner" of the record.
        /// </summary>
        /// 
        /// <value>
        /// An enumeration value indicating the relationship between the 
        /// record owner and the person authorized to use the record.
        /// </value>
        /// 
        /// <remarks>
        /// See <see cref="RelationshipType"/> for more information on the
        /// relationships and what they mean.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public RelationshipType RelationshipType
        {
            get
            {
                VerifyUpdated();
                return _relationshipType;
            }
            protected set
            {
                _relationshipType = value;
                _updated = true;
            }
        }
        private RelationshipType _relationshipType = RelationshipType.Unknown;

        /// <summary>
        /// Gets the localized string representing the relationship between
        /// the person authorized to view this record and the owner of the
        /// record.
        /// </summary>
        /// 
        /// <value>
        /// A string representation of the enumeration value indicating the 
        /// relationship between the record owner and the person authorized 
        /// to use the record.
        /// </value>
        /// 
        /// <remarks>
        /// See <see cref="RelationshipType"/> for more information on the
        /// relationships and what they mean.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public string RelationshipName
        {
            get
            {
                VerifyUpdated();
                return _relationshipName;
            }
            protected set
            {
                _relationshipName = value;
                _updated = true;
            }
        }
        private string _relationshipName;

        /// <summary>
        /// Gets the display name of the record.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the name of the record as seen by the
        /// current user.
        /// </value>
        /// 
        /// <remarks>
        /// A record has a name that defaults to the name of the owner of
        /// the record. A nickname can override the record name for each 
        /// person authorized to use the record. If the nickname is specified,
        /// it is returned in this property. If the nickname is not specified,
        /// the record name is returned.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The record was constructed using the record ID and 
        /// <see cref="Refresh"/> has not been called.
        /// </exception>
        /// 
        public string DisplayName
        {
            get
            {
                VerifyUpdated();
                return _displayName;
            }
            protected set
            {
                _displayName = value;
                _updated = true;
            }
        }
        private string _displayName;

        /// <summary>
        /// Gets the state of a <see cref="HealthRecordInfo"/>.
        /// </summary>
        ///
        public HealthRecordState State
        {
            get
            {
                return _state;
            }

            internal protected set
            {
                _state = value;
            }
        }
        private HealthRecordState _state;

        /// <summary>
        /// Gets the date the record was created, in UTC.
        /// </summary>
        /// 
        public DateTime DateCreated
        {
            get
            {
                return _dateCreated;
            }
            protected set
            {
                _dateCreated = value;
            }
        }
        private DateTime _dateCreated;

        /// <summary>
        /// Gets the date the record was updated, in UTC.
        /// </summary>
        /// 
        public DateTime DateUpdated
        {
            get
            {
                return _dateUpdated;
            }
            protected set
            {
                _dateUpdated = value;
            }
        }
        private DateTime _dateUpdated;

        /// <summary>
        /// Gets the maximum total size in bytes that the <see cref="HealthRecordItem" />s in 
        /// the <see cref="HealthRecordInfo" /> can occupy.
        /// </summary>
        /// 
        /// <remarks> 
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is 
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        /// 
        public Int64? QuotaInBytes
        {
            get { return _quotaInBytes; }
            protected set { _quotaInBytes = value; }
        }
        private Int64? _quotaInBytes;

        /// <summary>
        /// Gets the total size in bytes that the <see cref="HealthRecordItem" />s in 
        /// the <see cref="HealthRecordInfo" /> currently occupy.
        /// </summary>
        /// 
        /// <remarks> 
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is 
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        /// 
        public Int64? QuotaUsedInBytes
        {
            get { return _quotaUsedInBytes; }
            protected set { _quotaUsedInBytes = value; }
        }
        private Int64? _quotaUsedInBytes;

        /// <summary>
        /// Gets the record's latest operation sequence number.
        /// </summary>
        /// 
        /// <remarks>
        /// The record's operation sequence number is used when sync'ing data from a 
        /// record. Anytime an operation is performed against a health record item HealthVault
        /// stamps it with the next increment of the operation sequence number for the record.
        /// For example, the first item added to the record would be stamped with the sequence
        /// number 1, the next operation would stamp the thing with 2, etc. Applications can
        /// determine all operations that have occurred since a known point by calling 
        /// GetRecordOperations and passing the sequence number of the known point.
        /// </remarks>
        /// 
        public long LatestOperationSequenceNumber { get; protected set; }

        /// <summary>
        /// Gets the <see cref="HealthRecordAuthorizationStatus"/> for the record.
        /// </summary>
        /// <remarks>
        /// The status indicates whether, at the time of retrieval, the application 
        /// is able to access the record.  Any status other than NoActionRequired 
        /// requires user intervention in HealthVault before the application may 
        /// successfully access the record.  
        /// </remarks>
        public HealthRecordAuthorizationStatus HealthRecordAuthorizationStatus
        {
            get { return _authorizationStatus; }
            protected set { _authorizationStatus = value; }
        }
        private HealthRecordAuthorizationStatus _authorizationStatus;

        /// <summary>
        /// Gets the application specific record id for the specified 
        /// record and application.
        /// </summary>
        /// 
        public string ApplicationSpecificRecordId
        {
            get
            {
                return _applicationSpecificRecordId;
            }
            protected set
            {
                _applicationSpecificRecordId = value;
            }
        }
        private string _applicationSpecificRecordId;

        /// <summary>
        /// Gets the date when the user authorized the application to the record, in UTC.
        /// </summary>
        public DateTime RecordAppAuthCreatedDate { get; protected set; }

        #region Update

        /// <summary>
        /// Updates the <see cref="HealthRecordInfo"/> instance with data from 
        /// the server using the <see cref="HealthRecordAccessor.Id"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// This method is called and the 
        /// <see cref="HealthRecordAccessor.Connection"/> 
        /// object of the <see cref="HealthRecordInfo"/> is not an 
        /// <see cref="AuthenticatedConnection"/>.
        /// </exception>
        /// 
        public void Refresh()
        {
            AuthenticatedConnection connection =
                Connection as AuthenticatedConnection;
            if (connection == null)
            {
                OfflineWebApplicationConnection offlineAuthConnection =
                    Connection as OfflineWebApplicationConnection;

                Validator.ThrowInvalidIfNull(offlineAuthConnection, "ConnectionIsNeitherAuthenticatedNorOffline");
            }

            Collection<HealthRecordInfo> records =
                HealthVaultPlatform.GetAuthorizedRecords(Connection, new Guid[] { this.Id });

            if (records.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError();
                error.Message =
                    ResourceRetriever.FormatResourceString(
                        "RecordNotFoundException",
                        this.Id);

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.RecordNotFound,
                        error);
                throw e;
            }

            HealthRecordInfo thisRecord = records[0];
            this._custodian = thisRecord.IsCustodian;
            this._name = thisRecord.Name;
            this._relationshipName = thisRecord.RelationshipName;
            this._relationshipType = thisRecord.RelationshipType;
            this._dateAuthorizationExpires = thisRecord.DateAuthorizationExpires;
            this._quotaInBytes = thisRecord.QuotaInBytes;
            this._quotaUsedInBytes = thisRecord.QuotaUsedInBytes;
            this._state = thisRecord.State;
            this._dateCreated = thisRecord.DateCreated;
            this._displayName = thisRecord.DisplayName;
            this._authExpired = thisRecord.HasAuthorizationExpired;

            this._updated = true;
        }

        #endregion Update

        #endregion Public properties

        /// <summary>
        /// Gets the name of the record.
        /// </summary>
        /// 
        /// <value>
        /// The name of the record.
        /// </value>
        /// 
        public override string ToString()
        {
            return Name;
        }

        private void VerifyUpdated()
        {
            Validator.ThrowInvalidIf(
                !_updated,
                "HealthRecordNotUpdated");
        }

        internal bool IsUpdated
        {
            get { return _updated; }
        }
        private bool _updated;
    }
}
