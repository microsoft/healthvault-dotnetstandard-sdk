// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Application;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Record;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents the API set used to access a health record for an individual.
    /// </summary>
    ///
    /// <remarks>
    /// A HealthRecordAccessor represents a person's view of a health record.
    /// This view can vary based upon the access rights the person has to the
    /// record. More than one person might have access to the same record but have
    /// different views. For instance, a husband might have a HealthRecordAccessor
    /// instance for himself and another for his wife's health record to which
    /// she granted him access.
    /// </remarks>
    ///
    internal class HealthRecordAccessor
    {
        /// <summary>
        /// Creates an instance of a HealthRecordAccessor object using
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
        /// A new instance of a HealthRecordAccessor object containing the
        /// record information.
        /// </returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> or <paramref name="navigator"/>
        /// parameter is <b>null</b>.
        /// </exception>
        ///
        public static HealthRecordAccessor CreateFromXml(
            IConnectionInternal connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.PersonInfoConnectionNull);
            Validator.ThrowIfArgumentNull(navigator, nameof(navigator), Resources.ParseXmlNavNull);

            HealthRecordAccessor recordInfo =
                new HealthRecordAccessor(connection);

            recordInfo.ParseXml(navigator);
            return recordInfo;
        }

        /// <summary>
        /// Parses HealthRecordAccessor member data from the specified XPathNavigator.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the record information.
        /// </param>
        ///
        internal virtual void ParseXml(XPathNavigator navigator)
        {
            string id = navigator.GetAttribute("id", string.Empty);
            this.Id = new Guid(id);

            string country = navigator.GetAttribute("location-country", string.Empty);
            string state = navigator.GetAttribute("location-state-province", string.Empty);
            if (!string.IsNullOrEmpty(country))
            {
                this.Location = new Location(country, string.IsNullOrEmpty(state) ? null : state);
            }
        }

        /// <summary>
        /// Retrieves the XML representation of the <see cref="HealthRecordAccessor"/>.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the XML representation of the
        /// <see cref="HealthRecordAccessor"/>.
        /// </returns>
        ///
        public virtual string GetXml()
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

        internal virtual void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);
            this.WriteXml(writer);
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", this.Id.ToString());

            if (this.Location != null)
            {
                writer.WriteAttributeString("location-country", this.Location.Country);
                if (!string.IsNullOrEmpty(this.Location.StateProvince))
                {
                    writer.WriteAttributeString("location-state-province", this.Location.StateProvince);
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordAccessor"/>
        /// class.
        /// </summary>
        ///
        /// <param name="connection">
        /// An instance of a connection to which the record
        /// operations will be directed.
        /// </param>
        ///
        /// <param name="id">
        /// The unique identifier for the record.
        /// </param>
        ///
        /// <remarks>
        /// This constructor creates a view of a personal health record.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="id"/> parameter is Guid.Empty.
        /// </exception>
        ///
        public HealthRecordAccessor(
            IHealthVaultConnection connection,
            Guid id)
        {
            Validator.ThrowIfArgumentNull(connection, nameof(connection), Resources.CtorServiceArgumentNull);

            if (id == Guid.Empty)
            {
                throw new ArgumentException(Resources.CtorIDArgumentEmpty, nameof(id));
            }

            this.Connection = connection;
            this.Id = id;
        }

        /// <summary>
        /// Constructs a HealthRecordAccessor for deserialization purposes.
        /// </summary>
        ///
        /// <param name="connection">
        /// An instance of a connection to which the record
        /// operations will be directed.
        /// </param>
        ///
        /// <remarks>
        /// This constructor is useful only if ParseXml is called.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        ///
        internal HealthRecordAccessor(
            IHealthVaultConnection connection)
        {
            this.Connection = connection;
        }

        internal HealthRecordAccessor()
        {
        }

        #region Public properties

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
        /// Gets a reference to the HealthVault service that
        /// created this <see cref="HealthRecordAccessor"/>.
        /// </summary>
        ///
        public IHealthVaultConnection Connection { get; internal set; }

        #endregion Public properties

        #region Things search methods

        /// <summary>
        /// Creates a new HealthRecordSearcher for this record.
        /// </summary>
        ///
        /// <returns>
        /// A HealthRecordSearcher that searches for items associated
        /// with this record.
        /// </returns>
        ///
        /// <remarks>
        /// You can also call the <see cref="HealthRecordSearcher"/> constructor
        /// directly and pass in a reference to this
        /// <see cref="HealthRecordAccessor"/>.
        /// </remarks>
        ///
        public HealthRecordSearcher CreateSearcher()
        {
            return new HealthRecordSearcher(this);
        }

        /// <summary>
        /// Creates a new HealthRecordSearcher for a list of specific types.
        /// </summary>
        ///
        /// <returns>
        /// A <see cref="HealthRecordSearcher"/> that searches for items with specific type IDs
        /// within this record.
        /// </returns>
        ///
        /// <remarks>
        /// The method adds a filter to the <see cref="HealthRecordSearcher"/> that only returns
        /// items of the specified type IDs. That filter may be accessed through the
        /// returned searcher using searcher.Filters[0].
        ///
        /// You can also call the <see cref="HealthRecordSearcher"/> constructor
        /// directly and pass in a reference to this
        /// <see cref="HealthRecordAccessor"/>.
        /// </remarks>
        ///
        /// <param name="typeIds">
        /// A list of unique type ids to filter on.
        /// </param>
        ///
        public HealthRecordSearcher CreateSearcher(params Guid[] typeIds)
        {
            HealthRecordSearcher searcher = new HealthRecordSearcher(this);
            ThingQuery query = new ThingQuery(typeIds);
            searcher.Filters.Add(query);

            return searcher;
        }

        #region Authorization methods

        /// <summary>
        /// Releases the authorization of the application on the health record.
        /// </summary>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors during the authorization release.
        /// </exception>
        ///
        /// <remarks>
        /// Once the application releases the authorization to the health record,
        /// calling any methods of this <see cref="HealthRecordAccessor"/> will result
        /// in a <see cref="HealthServiceAccessDeniedException"/>."
        /// </remarks>
        public async Task RemoveApplicationAuthorizationAsync()
        {
            await this.Connection.ExecuteAsync(HealthVaultMethods.RemoveApplicationRecordAuthorization, 1).ConfigureAwait(false);
        }

        #endregion Authorization methods

        /// <summary>
        /// Returns a list of <see cref="ThingTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </summary>
        ///
        /// <param name="healthRecordItemTypes">
        /// A collection of <see cref="ThingTypeDefinition" />
        /// representing the thing types
        /// for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A list of <see cref="ThingTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
        /// </remarks>
        ///
        public async Task<Collection<ThingTypePermission>> QueryPermissions(
            IList<ThingTypeDefinition> healthRecordItemTypes)
        {
            Validator.ThrowIfArgumentNull(healthRecordItemTypes, nameof(healthRecordItemTypes), Resources.CtorhealthRecordItemTypesArgumentNull);

            List<Guid> thingTypeIds = new List<Guid>();
            foreach (ThingTypeDefinition definition in healthRecordItemTypes)
            {
                thingTypeIds.Add(definition.TypeId);
            }

            return await this.QueryPermissions(thingTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this  record.
        /// </summary>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of unique identifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// Returns a dictionary of <see cref="ThingTypePermission"/>
        /// with thing types as the keys.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty dictionary is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// <b> null </b> will be returned for that type in the dictionary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public async Task<IDictionary<Guid, ThingTypePermission>> QueryPermissionsByTypes(
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatform.QueryPermissionsByTypesAsync(this.Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record.
        /// </summary>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A list of <see cref="ThingTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned. If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// If there is an exception during executing the request to HealthVault.
        /// </exception>
        ///
        public async Task<Collection<ThingTypePermission>> QueryPermissions(
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatform.QueryPermissionsAsync(this.Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the permissions which the authenticated person
        /// has when using the calling application for the specified item types
        /// in this health record as well as the other permission settings such as <see cref="HealthRecordPermissions.MeaningfulUseOptIn">MeaningfulUseOptIn</see>.
        /// </summary>
        ///
        /// <param name="healthRecordItemTypeIds">
        /// A collection of uniqueidentifiers to identify the health record
        /// item types, for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthRecordPermissions"/> object
        /// which contains a collection of <see cref="ThingTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of thing types is empty, an empty list is
        /// returned for <see cref="HealthRecordPermissions"/> object's ItemTypePermissions property.
        /// If for a thing type, the person has
        /// neither online access nor offline access permissions,
        /// ThingTypePermission object is not returned for that
        /// thing type.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="healthRecordItemTypeIds"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There is an error in the server request.
        /// </exception>
        ///
        public async Task<HealthRecordPermissions> QueryRecordPermissions(
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatform.QueryRecordPermissionsAsync(this.Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets valid group memberships for a record.
        /// </summary>
        ///
        /// <remarks>
        /// Group membership thing types allow an application to signify that the
        /// record belongs to an application defined group.  A record in the group may be
        /// eligible for special programs offered by other applications, for example.
        /// Applications then need a away to query for valid group memberships.
        /// <br/>
        /// Valid group memberships are those memberships which are not expired, and whose
        /// last updating application is authorized by the last updating person to
        /// read and delete the membership.
        /// </remarks>
        ///
        /// <param name="applicationIds">
        /// A collection of unique application identifiers for which to
        /// search for group memberships.  For a null or empty application identifier
        /// list, return all valid group memberships for the record.  Otherwise,
        /// return only those group memberships last updated by one of the
        /// supplied application identifiers.
        /// </param>
        ///
        /// <returns>
        /// A List of things representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public async Task<Collection<ThingBase>> GetValidGroupMembership(IList<Guid> applicationIds)
        {
            return await HealthVaultPlatform.GetValidGroupMembershipAsync(this.Connection, this, applicationIds).ConfigureAwait(false);
        }

        #endregion
    }
}