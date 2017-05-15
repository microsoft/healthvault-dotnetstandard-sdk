// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Thing;

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
    public class HealthRecordInfo
    {
        #region Constructors

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
        {
            Id = recordInfo.Id;
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
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordInfo"/> class
        /// for deserialization purposes.
        /// </summary>
        /// <remarks>
        /// This constructor is only useful if ParseXml is called.
        /// </remarks>
        public HealthRecordInfo()
        {
        }

        #endregion

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
        public static HealthRecordInfo CreateFromXml(
            IHealthVaultConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.PersonInfoConnectionNull);
            Validator.ThrowIfArgumentNull(navigator, nameof(navigator), Resources.ParseXmlNavNull);

            HealthRecordInfo recordInfo = new HealthRecordInfo();
            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        /// <summary>
        /// Gets the record identifier.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier (GUID) for the record.
        /// </value>
        ///
        /// <remarks>
        /// The record identifier is issued when the record is created. Creating
        /// the account automatically creates a self record as well.
        /// </remarks>
        ///
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the location of the person that this record is for.
        /// </summary>
        ///
        public Location Location { get; protected set; }

        /// <summary>
        /// Creates an instance of a HealthRecordInfo object using  the specified XML.
        /// </summary>
        /// <param name="navigator">The navigator.</param>
        /// <returns>HealthRecordInfo</returns>
        public static HealthRecordInfo CreateFromXml(XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(navigator, nameof(navigator), Resources.ParseXmlNavNull);

            HealthRecordInfo recordInfo = new HealthRecordInfo();
            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        #region Xml

        /// <summary>
        /// Parses HealthRecordInfo member data from the specified XPathNavigator.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        ///
        internal void ParseXml(XPathNavigator navigator)
        {
            string id = navigator.GetAttribute("id", string.Empty);
            Id = new Guid(id);

            string country = navigator.GetAttribute("location-country", string.Empty);
            string state = navigator.GetAttribute("location-state-province", string.Empty);
            if (!string.IsNullOrEmpty(country))
            {
                Location = new Location(country, string.IsNullOrEmpty(state) ? null : state);
            }

            _custodian = XPathHelper.ParseAttributeAsBoolean(navigator, "record-custodian", false);

            long? relationshipNumber;

            relationshipNumber = XPathHelper.ParseAttributeAsLong(navigator, "rel-type", 0);
            if (relationshipNumber.HasValue &&
               (relationshipNumber <= (int)RelationshipType.Daughter))
            {
                _relationshipType = (RelationshipType)relationshipNumber;
            }

            _relationshipName = navigator.GetAttribute("rel-name", string.Empty);

            _dateAuthorizationExpires = XPathHelper.ParseAttributeAsDateTime(navigator, "auth-expires", DateTime.MinValue);

            _authExpired = XPathHelper.ParseAttributeAsBoolean(navigator, "auth-expired", false);

            _name = navigator.Value;

            _displayName = navigator.GetAttribute("display-name", string.Empty);

            State = XPathHelper.ParseAttributeAsEnum(navigator, "state", HealthRecordState.Unknown);
            if (State > HealthRecordState.Deleted)
            {
                State = HealthRecordState.Unknown;
            }

            DateCreated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-created", DateTime.MinValue);
            DateUpdated = XPathHelper.ParseAttributeAsDateTime(navigator, "date-updated", DateTime.MinValue);

            QuotaInBytes = XPathHelper.ParseAttributeAsLong(navigator, "max-size-bytes", null);
            QuotaUsedInBytes = XPathHelper.ParseAttributeAsLong(navigator, "size-bytes", null);

            HealthRecordAuthorizationStatus = XPathHelper.ParseAttributeAsEnum(
                    navigator,
                    "app-record-auth-action",
                    HealthRecordAuthorizationStatus.Unknown);

            ApplicationSpecificRecordId = navigator.GetAttribute("app-specific-record-id", string.Empty);

            LatestOperationSequenceNumber = XPathHelper.ParseAttributeAsLong(navigator, "latest-operation-sequence-number", 0).Value;

            RecordAppAuthCreatedDate = XPathHelper.ParseAttributeAsDateTime(navigator, "record-app-auth-created-date", DateTime.MinValue);
        }

        /// <summary>
        /// Gets the XML representation of the HealthRecordInfo.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the HealthRecordInfo.
        /// </returns>
        ///
        public string GetXml()
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

        internal void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);

            writer.WriteAttributeString("id", Id.ToString());

            if (Location != null)
            {
                writer.WriteAttributeString("location-country", Location.Country);
                if (!string.IsNullOrEmpty(Location.StateProvince))
                {
                    writer.WriteAttributeString("location-state-province", Location.StateProvince);
                }
            }

            writer.WriteAttributeString(
                "record-custodian",
                XmlConvert.ToString(_custodian));

            writer.WriteAttributeString(
                "rel-type",
                XmlConvert.ToString((int)_relationshipType));

            if (!string.IsNullOrEmpty(_relationshipName))
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

            if (!string.IsNullOrEmpty(_displayName))
            {
                writer.WriteAttributeString(
                    "display-name",
                    _displayName);
            }

            writer.WriteAttributeString(
                "state",
                State.ToString());

            writer.WriteAttributeString(
                "date-created",
                SDKHelper.XmlFromDateTime(DateCreated));

            if (QuotaInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "max-size-bytes",
                    XmlConvert.ToString(QuotaInBytes.Value));
            }

            if (QuotaUsedInBytes.HasValue)
            {
                writer.WriteAttributeString(
                    "size-bytes",
                    XmlConvert.ToString(QuotaUsedInBytes.Value));
            }

            writer.WriteAttributeString(
                "app-record-auth-action",
                HealthRecordAuthorizationStatus.ToString());

            writer.WriteAttributeString(
                "app-specific-record-id",
                ApplicationSpecificRecordId);

            writer.WriteAttributeString(
                "date-updated",
                SDKHelper.XmlFromDateTime(DateUpdated));

            writer.WriteValue(_name);

            writer.WriteEndElement();
        }

        #endregion

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
        public bool IsCustodian
        {
            get
            {
                return _custodian;
            }

            set
            {
                _custodian = value;
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
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public DateTime DateAuthorizationExpires
        {
            get
            {
                return _dateAuthorizationExpires;
            }

            set
            {
                _dateAuthorizationExpires = value;
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
                return _authExpired;
            }

            set
            {
                _authExpired = value;
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
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
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
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public RelationshipType RelationshipType
        {
            get
            {
                return _relationshipType;
            }

            set
            {
                _relationshipType = value;
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
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string RelationshipName
        {
            get
            {
                return _relationshipName;
            }

            set
            {
                _relationshipName = value;
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
        /// The record was constructed using the record ID.
        /// </exception>
        ///
        public string DisplayName
        {
            get
            {
                return _displayName;
            }

            set
            {
                _displayName = value;
            }
        }

        private string _displayName;

        /// <summary>
        /// Gets the state of a <see cref="HealthRecordInfo"/>.
        /// </summary>
        ///
        public HealthRecordState State { get; protected internal set; }

        /// <summary>
        /// Gets the date the record was created, in UTC.
        /// </summary>
        ///
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets the date the record was updated, in UTC.
        /// </summary>
        ///
        public DateTime DateUpdated { get; set; }

        /// <summary>
        /// Gets the maximum total size in bytes that the <see cref="ThingBase" />s in
        /// the <see cref="HealthRecordInfo" /> can occupy.
        /// </summary>
        ///
        /// <remarks>
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        ///
        public long? QuotaInBytes { get; set; }

        /// <summary>
        /// Gets the total size in bytes that the <see cref="ThingBase" />s in
        /// the <see cref="HealthRecordInfo" /> currently occupy.
        /// </summary>
        ///
        /// <remarks>
        /// This data value is only available when the <see cref="HealthRecordInfo"/> object is
        /// fetched from the HealthVault platform as opposed to created on the fly.
        /// </remarks>
        ///
        public long? QuotaUsedInBytes { get; set; }

        /// <summary>
        /// Gets the record's latest operation sequence number.
        /// </summary>
        ///
        /// <remarks>
        /// The record's operation sequence number is used when syncing data from a
        /// record. Anytime an operation is performed against a thing HealthVault
        /// stamps it with the next increment of the operation sequence number for the record.
        /// For example, the first item added to the record would be stamped with the sequence
        /// number 1, the next operation would stamp the thing with 2, etc. Applications can
        /// determine all operations that have occurred since a known point by calling
        /// GetRecordOperations and passing the sequence number of the known point.
        /// </remarks>
        ///
        public long LatestOperationSequenceNumber { get; set; }

        /// <summary>
        /// Gets the <see cref="HealthRecordAuthorizationStatus"/> for the record.
        /// </summary>
        /// <remarks>
        /// The status indicates whether, at the time of retrieval, the application
        /// is able to access the record.  Any status other than NoActionRequired
        /// requires user intervention in HealthVault before the application may
        /// successfully access the record.
        /// </remarks>
        public HealthRecordAuthorizationStatus HealthRecordAuthorizationStatus { get; set; }

        /// <summary>
        /// Gets the application specific record id for the specified
        /// record and application.
        /// </summary>
        ///
        public string ApplicationSpecificRecordId { get; set; }

        /// <summary>
        /// Gets the date when the user authorized the application to the record, in UTC.
        /// </summary>
        public DateTime RecordAppAuthCreatedDate { get; set; }

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
    }
}
