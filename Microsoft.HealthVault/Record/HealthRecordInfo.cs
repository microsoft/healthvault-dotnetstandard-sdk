// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Record
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
        public static new HealthRecordInfo CreateFromXml(
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
        /// Creates an instance of a HealthRecordInfo object using  the specified XML.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <returns>HealthRecordInfo</returns>
        public static HealthRecordInfo CreateFromXml(XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(navigator, "navigator", "ParseXmlNavNull");

            HealthRecordInfo recordInfo = new HealthRecordInfo();
            recordInfo.ParseXml(navigator);
            return recordInfo;
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

            this.custodian = XPathHelper.ParseAttributeAsBoolean(navigator, "record-custodian", false);

            long? relationshipNumber;

            relationshipNumber = XPathHelper.ParseAttributeAsLong(navigator, "rel-type", 0);
            if (relationshipNumber.HasValue &&
               (relationshipNumber <= (int)RelationshipType.Daughter))
            {
                this.relationshipType = (RelationshipType)relationshipNumber;
            }

            this.relationshipName = navigator.GetAttribute("rel-name", string.Empty);

            this.dateAuthorizationExpires = XPathHelper.ParseAttributeAsDateTime(navigator, "auth-expires", DateTime.MinValue);

            this.authExpired = XPathHelper.ParseAttributeAsBoolean(navigator, "auth-expired", false);

            this.name = navigator.Value;

            this.displayName = navigator.GetAttribute("display-name", string.Empty);

            this.State = XPathHelper.ParseAttributeAsEnum(navigator, "state", HealthRecordState.Unknown);
            if (this.State > HealthRecordState.Deleted)
            {
                this.State = HealthRecordState.Unknown;
            }

            this.DateCreated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-created", DateTime.MinValue);
            this.DateUpdated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-updated", DateTime.MinValue);

            this.QuotaInBytes = XPathHelper.ParseAttributeAsLong(navigator, "max-size-bytes", null);
            this.QuotaUsedInBytes = XPathHelper.ParseAttributeAsLong(navigator, "size-bytes", null);

            this.HealthRecordAuthorizationStatus = XPathHelper.ParseAttributeAsEnum(
                    navigator,
                    "app-record-auth-action",
                    HealthRecordAuthorizationStatus.Unknown);

            this.ApplicationSpecificRecordId = navigator.GetAttribute("app-specific-record-id", string.Empty);

            this.LatestOperationSequenceNumber = XPathHelper.ParseAttributeAsLong(navigator, "latest-operation-sequence-number", 0).Value;

            this.RecordAppAuthCreatedDate = XPathHelper.ParseAttributeAsDateTime(navigator, "record-app-auth-created-date", DateTime.MinValue);

            this.IsUpdated = true;
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
            this.ParseXml(healthRecordInfoDoc.CreateNavigator());
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
                this.WriteXml("record", writer);
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

            this.WriteXml("record", writer);
        }

        internal override void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);

            this.WriteXml(writer);

            writer.WriteAttributeString(
                "record-custodian",
                XmlConvert.ToString(this.custodian));

            writer.WriteAttributeString(
                "rel-type",
                XmlConvert.ToString((int)this.relationshipType));

            if (!string.IsNullOrEmpty(this.relationshipName))
            {
                writer.WriteAttributeString(
                    "rel-name",
                    this.relationshipName);
            }

            writer.WriteAttributeString(
                "auth-expires",
                SDKHelper.XmlFromDateTime(this.dateAuthorizationExpires));

            writer.WriteAttributeString(
                "auth-expired",
                SDKHelper.XmlFromBool(this.authExpired));

            if (!string.IsNullOrEmpty(this.displayName))
            {
                writer.WriteAttributeString(
                    "display-name",
                    this.displayName);
            }

            writer.WriteAttributeString(
                "state",
                this.State.ToString());

            writer.WriteAttributeString(
                "date-created",
                SDKHelper.XmlFromDateTime(this.DateCreated));

            if (this.QuotaInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "max-size-bytes",
                    XmlConvert.ToString(this.QuotaInBytes.Value));
            }

            if (this.QuotaUsedInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "size-bytes",
                    XmlConvert.ToString(this.QuotaUsedInBytes.Value));
            }

            writer.WriteAttributeString(
                "app-record-auth-action",
                this.HealthRecordAuthorizationStatus.ToString());

            writer.WriteAttributeString(
                "app-specific-record-id",
                this.ApplicationSpecificRecordId);

            writer.WriteAttributeString(
                "date-updated",
                SDKHelper.XmlFromDateTime(this.DateUpdated));

            writer.WriteValue(this.name);

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
                this.custodian = recordInfo.IsCustodian;
                this.dateAuthorizationExpires = recordInfo.DateAuthorizationExpires;
                this.name = recordInfo.Name;
                this.relationshipType = recordInfo.RelationshipType;
                this.relationshipName = recordInfo.RelationshipName;
                this.displayName = recordInfo.DisplayName;

                if (recordInfo.Location != null)
                {
                    this.Location = new Location(recordInfo.Location.Country, recordInfo.Location.StateProvince);
                }

                this.IsUpdated = true;
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

        internal HealthRecordInfo()
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
                this.VerifyUpdated();
                return this.custodian;
            }

            protected set
            {
                this.custodian = value;
                this.IsUpdated = true;
            }
        }

        private bool custodian;

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
                this.VerifyUpdated();
                return this.dateAuthorizationExpires;
            }

            protected set
            {
                this.dateAuthorizationExpires = value;
                this.IsUpdated = true;
            }
        }

        private DateTime dateAuthorizationExpires;

        /// <summary>
        /// <b>true</b> if the authorization of the authenticated person has
        /// expired for this record; otherwise, <b>false</b>.
        /// </summary>
        ///
        public bool HasAuthorizationExpired
        {
            get
            {
                this.VerifyUpdated();
                return this.authExpired;
            }

            protected set
            {
                this.authExpired = value;
                this.IsUpdated = true;
            }
        }

        private bool authExpired;

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
                this.VerifyUpdated();
                return this.name;
            }

            protected set
            {
                this.name = value;
                this.IsUpdated = true;
            }
        }

        private string name;

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
                this.VerifyUpdated();
                return this.relationshipType;
            }

            protected set
            {
                this.relationshipType = value;
                this.IsUpdated = true;
            }
        }

        private RelationshipType relationshipType = RelationshipType.Unknown;

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
                this.VerifyUpdated();
                return this.relationshipName;
            }

            protected set
            {
                this.relationshipName = value;
                this.IsUpdated = true;
            }
        }

        private string relationshipName;

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
                this.VerifyUpdated();
                return this.displayName;
            }

            protected set
            {
                this.displayName = value;
                this.IsUpdated = true;
            }
        }

        private string displayName;

        /// <summary>
        /// Gets the state of a <see cref="HealthRecordInfo"/>.
        /// </summary>
        ///
        public HealthRecordState State { get; protected internal set; }

        /// <summary>
        /// Gets the date the record was created, in UTC.
        /// </summary>
        ///
        public DateTime DateCreated { get; protected set; }

        /// <summary>
        /// Gets the date the record was updated, in UTC.
        /// </summary>
        ///
        public DateTime DateUpdated { get; protected set; }

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
        public long? QuotaInBytes { get; protected set; }

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
        public long? QuotaUsedInBytes { get; protected set; }

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
        public HealthRecordAuthorizationStatus HealthRecordAuthorizationStatus { get; protected set; }

        /// <summary>
        /// Gets the application specific record id for the specified
        /// record and application.
        /// </summary>
        ///
        public string ApplicationSpecificRecordId { get; protected set; }

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
        public async Task Refresh()
        {
            AuthenticatedConnection connection =
                this.Connection as AuthenticatedConnection;
            if (connection == null)
            {
                OfflineWebApplicationConnection offlineAuthConnection =
                    this.Connection as OfflineWebApplicationConnection;

                Validator.ThrowInvalidIfNull(offlineAuthConnection, "ConnectionIsNeitherAuthenticatedNorOffline");
            }

            Collection<HealthRecordInfo> records =
                await HealthVaultPlatform.GetAuthorizedRecordsAsync(this.Connection, new[] { this.Id }).ConfigureAwait(false);

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
            this.custodian = thisRecord.IsCustodian;
            this.name = thisRecord.Name;
            this.relationshipName = thisRecord.RelationshipName;
            this.relationshipType = thisRecord.RelationshipType;
            this.dateAuthorizationExpires = thisRecord.DateAuthorizationExpires;
            this.QuotaInBytes = thisRecord.QuotaInBytes;
            this.QuotaUsedInBytes = thisRecord.QuotaUsedInBytes;
            this.State = thisRecord.State;
            this.DateCreated = thisRecord.DateCreated;
            this.displayName = thisRecord.DisplayName;
            this.authExpired = thisRecord.HasAuthorizationExpired;

            this.IsUpdated = true;
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
            return this.Name;
        }

        private void VerifyUpdated()
        {
            Validator.ThrowInvalidIf(
                !this.IsUpdated,
                "HealthRecordNotUpdated");
        }

        internal bool IsUpdated { get; private set; }
    }
}
