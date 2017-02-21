// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault
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
    public class HealthRecordAccessor
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
            ApplicationConnection connection,
            XPathNavigator navigator)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "PersonInfoConnectionNull");
            Validator.ThrowIfArgumentNull(navigator, "navigator", "ParseXmlNavNull");

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
            string id = navigator.GetAttribute("id", String.Empty);
            _recordId = new Guid(id);

            string country = navigator.GetAttribute("location-country", String.Empty);
            string state = navigator.GetAttribute("location-state-province", String.Empty);
            if (!String.IsNullOrEmpty(country))
            {
                Location = new Location(country, String.IsNullOrEmpty(state) ? null : state);
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
                WriteXml("record", writer);
                writer.Flush();
            }
            return recordInfoXml.ToString();
        }

        internal virtual void WriteXml(string nodeName, XmlWriter writer)
        {
            writer.WriteStartElement(nodeName);
            WriteXml(writer);
        }

        internal void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("id", _recordId.ToString());

            if (Location != null)
            {
                writer.WriteAttributeString("location-country", Location.Country);
                if (!String.IsNullOrEmpty(Location.StateProvince))
                {
                    writer.WriteAttributeString("location-state-province", Location.StateProvince);
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
            ApplicationConnection connection,
            Guid id)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "CtorServiceArgumentNull");

            Validator.ThrowArgumentExceptionIf(
                id == Guid.Empty,
                "id",
                "CtorIDArgumentEmpty");

            _connection = connection;
            _recordId = id;
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
            ApplicationConnection connection)
        {
            _connection = connection;
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
        public Guid Id
        {
            get { return _recordId; }
        }
        private Guid _recordId;

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
        public ApplicationConnection Connection
        {
            get { return _connection; }
        }
        private ApplicationConnection _connection;

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
            HealthRecordFilter filter = new HealthRecordFilter(typeIds);
            searcher.Filters.Add(filter);

            return searcher;
        }

        /// <summary>
        /// Gets the health record item specified by its ID.
        /// </summary>
        ///
        /// <param name="itemId">
        /// The ID of the health record item to retrieve.
        /// </param>
        ///
        /// <param name="sections">
        /// The data sections of the health record item to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthRecordItem"/> with the specified data sections
        /// filled out.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task<HealthRecordItem> GetItemAsync(
            Guid itemId,
            HealthRecordItemSections sections)
        {
            HealthRecordSearcher searcher = CreateSearcher();
            return await searcher.GetSingleItem(itemId, sections).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the health record item specified by its ID.
        /// </summary>
        ///
        /// <param name="itemId">
        /// The ID of the health record item to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="HealthRecordItem"/> with the default data sections (Core and XML)
        /// filled out.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task<HealthRecordItem> GetItemAsync(
            Guid itemId)
        {
            return await GetItemAsync(itemId, HealthRecordItemSections.Default).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the health record items related to this record filtered on the
        /// specified type.
        /// </summary>
        ///
        /// <param name="typeId">
        /// A unique identifier for the type of health record item to filter
        /// on.
        /// </param>
        ///
        /// <param name="sections">
        /// The data sections of the health record item to retrieve.
        /// </param>
        ///
        /// <returns>
        /// A collection of the health record items related to this record
        /// that match the specified type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task<HealthRecordItemCollection> GetItemsByTypeAsync(
            Guid typeId,
            HealthRecordItemSections sections)
        {
            HealthRecordSearcher searcher = CreateSearcher(typeId);
            searcher.Filters[0].View.Sections = sections;

            ReadOnlyCollection<HealthRecordItemCollection> results =
                await searcher.GetMatchingItems().ConfigureAwait(false);

            // Since we only applied a single filter we should
            // only have a single group

            return results[0];
        }

        /// <summary>
        /// Gets the health record items related to this record filtered on the
        /// specified type.
        /// </summary>
        ///
        /// <param name="typeId">
        /// A unique identifier for the type of health record item to filter
        /// on.
        /// </param>
        ///
        /// <param name="view">
        /// The view to use when retrieving the data.
        /// </param>
        ///
        /// <returns>
        /// A collection of the health record items related to this record
        /// that match the specified type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task<HealthRecordItemCollection> GetItemsByTypeTask(
            Guid typeId,
            HealthRecordView view)
        {
            HealthRecordSearcher searcher = CreateSearcher(typeId);
            searcher.Filters[0].View = view;

            ReadOnlyCollection<HealthRecordItemCollection> results =
                await searcher.GetMatchingItems().ConfigureAwait(false);

            // Since we only applied a single filter we should
            // only have a single group

            return results[0];
        }

        /// <summary>
        /// Gets the health record items related to this record filtered on the
        /// specified type.
        /// </summary>
        ///
        /// <param name="typeId">
        /// A unique identifier for the type of health record item to filter
        /// on.
        /// </param>
        ///
        /// <returns>
        /// A collection of the health record items related to this record
        /// that match the specified type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task<HealthRecordItemCollection> GetItemsByTypeAsync(
            Guid typeId)
        {
            return await GetItemsByTypeAsync(typeId, HealthRecordItemSections.Default).ConfigureAwait(false);
        }

        #endregion Health Record Item search methods

        #region Thing Create/Update methods

        /// <summary>
        /// Creates a new health record item associated with this record in the
        /// HealthVault service.
        /// </summary>
        ///
        /// <param name="item">
        /// The health record item to be created in the HealthVault service.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The HealthRecordItem supplied was null.
        /// </exception>
        ///
        public async Task NewItemAsync(HealthRecordItem item)
        {
            Validator.ThrowIfArgumentNull(item, "item", "NewItemNullItem");

            await NewItemsAsync(new HealthRecordItem[] { item }).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates new health record items associated with the record.
        /// </summary>
        ///
        /// <param name="items">
        /// The health record items from which to create new instances.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur,
        /// no items will have been created.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// At least one HealthRecordItem in the supplied list was null.
        /// </exception>
        ///
        public async Task NewItemsAsync(IList<HealthRecordItem> items)
        {
            await HealthVaultPlatform.NewItemsAsync(Connection, this, items).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified health record item.
        /// </summary>
        ///
        /// <param name="item">
        /// The health record item to be updated.
        /// </param>
        ///
        /// <remarks>
        /// Only new items are updated with the appropriate unique identifier.
        /// All other sections must be updated manually.
        /// <br/><br/>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="item"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="item"/> parameter does not have an ID.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        ///
        public async Task UpdateItemAsync(HealthRecordItem item)
        {
            Validator.ThrowIfArgumentNull(item, "item", "UpdateItemNull");

            await UpdateItemsAsync(new HealthRecordItem[] { item }).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the specified health record items in one batch call to
        /// the service.
        /// </summary>
        ///
        /// <param name="itemsToUpdate">
        /// The health record items to be updated.
        /// </param>
        ///
        /// <remarks>
        /// Only new items are updated with the appropriate unique identifier.
        /// All other sections must be updated manually.
        /// <br/><br/>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="itemsToUpdate"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemsToUpdate"/> contains a <b>null</b> member or
        /// a <see cref="HealthRecordItem"/> instance that does not have an ID.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur,
        /// no items will have been updated.
        /// </exception>
        ///
        public async Task UpdateItemsAsync(
            IList<HealthRecordItem> itemsToUpdate)
        {
            await HealthVaultPlatform.UpdateItemsAsync(Connection, this, itemsToUpdate).ConfigureAwait(false);
        }

        #endregion Item Create/Update methods

        #region Item Removal methods

        /// <summary>
        /// Marks the specified health record item as deleted.
        /// </summary>
        ///
        /// <param name="item">
        /// The health record item to remove.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Health record items are never completely deleted. Instead, they
        /// are marked as deleted and are ignored for most normal operations.
        /// Items can be undeleted by contacting customer service.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="item"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// There are errors that remove the health record item from
        /// the server.
        /// </exception>
        ///
        public async Task RemoveItem(HealthRecordItem item)
        {
            Validator.ThrowIfArgumentNull(item, "item", "RemoveItemNull");

            await RemoveItemsAsync(new HealthRecordItem[] { item }).ConfigureAwait(false);
        }

        /// <summary>
        /// Marks the specified health record item as deleted.
        /// </summary>
        ///
        /// <param name="itemsToRemove">
        /// A list of the health record items to remove.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Health record items are never completely deleted. They are marked
        /// as deleted and are ignored for most normal operations. Items can
        /// be undeleted by contacting customer service.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemsToRemove"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors removed the health record items from the server.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur,
        /// no items will have been removed.
        /// </exception>
        ///
        public async Task RemoveItemsAsync(IList<HealthRecordItem> itemsToRemove)
        {
            Validator.ThrowArgumentExceptionIf(
                itemsToRemove == null || itemsToRemove.Count == 0,
                "itemsToRemove",
                "RemoveItemsListNullOrEmpty");

            List<HealthRecordItemKey> keys = new List<HealthRecordItemKey>();

            foreach (HealthRecordItem item in itemsToRemove)
            {
                if (item == null)
                {
                    continue;
                }

                keys.Add(item.Key);
            }

            await RemoveItemsAsync(keys).ConfigureAwait(false);
        }

        /// <summary>
        /// Marks the specified health record item as deleted.
        /// </summary>
        ///
        /// <param name="itemsToRemove">
        /// The unique item identifiers of the items to remove.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Health record items are never completely deleted. They are marked
        /// as deleted and are ignored for most normal operations. Items can
        /// be undeleted by contacting customer service.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemsToRemove"/> parameter is empty.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors removed the health record items from the server.
        /// The exception's Error property will contain the index of the
        /// item on which the failure occurred in the ErrorInfo property. If any failures occur,
        /// no items will have been removed.
        /// </exception>
        ///
        public async Task RemoveItemsAsync(IList<HealthRecordItemKey> itemsToRemove)
        {
            await HealthVaultPlatform.RemoveItemsAsync(Connection, this, itemsToRemove).ConfigureAwait(false);
        }

        ///
        /// <summary>
        /// Marks the specified health record item as deleted.
        /// </summary>
        ///
        /// <param name="itemId">
        /// The unique item identifier to remove.
        /// </param>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Health record items are never completely deleted. They are marked
        /// as deleted and are ignored for most normal operations. Items can
        /// be undeleted by contacting customer service.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="itemId"/> parameter is Guid.Empty.
        /// </exception>
        ///
        /// <exception cref="HealthServiceException">
        /// Errors removed the health record items from the server.
        /// </exception>
        ///
        public async Task RemoveItem(HealthRecordItemKey itemId)
        {
            Validator.ThrowIfArgumentNull(itemId, "itemId", "RemoveItemNull");

            await RemoveItemsAsync(new HealthRecordItemKey[] { itemId }).ConfigureAwait(false);
        }

        #endregion Item Removal methods

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
            await HealthVaultPlatform.RemoveApplicationAuthorizationAsync(Connection, this).ConfigureAwait(false);
        }

        #endregion Authorization methods

        /// <summary>
        /// Returns a list of <see cref="HealthRecordItemTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </summary>
        ///
        /// <param name="healthRecordItemTypes">
        /// A collection of <see cref="HealthRecordItemTypeDefinition" />
        /// representing the health record item types
        /// for which the permissions are being queried.
        /// </param>
        ///
        /// <returns>
        /// A list of <see cref="HealthRecordItemTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty list is
        /// returned. If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type.
        /// </remarks>
        ///
        public async Task<Collection<HealthRecordItemTypePermission>> QueryPermissions(
            IList<HealthRecordItemTypeDefinition> healthRecordItemTypes)
        {
            Validator.ThrowIfArgumentNull(healthRecordItemTypes, "healthRecordItemTypes", "CtorhealthRecordItemTypesArgumentNull");

            List<Guid> thingTypeIds = new List<Guid>();
            for (int i = 0; i < healthRecordItemTypes.Count; ++i)
            {
                thingTypeIds.Add(healthRecordItemTypes[i].TypeId);
            }
            return await QueryPermissions(thingTypeIds).ConfigureAwait(false);
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
        /// Returns a dictionary of <see cref="HealthRecordItemTypePermission"/>
        /// with health record item types as the keys.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty dictionary is
        /// returned. If for a health record item type, the person has
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
        public async Task<IDictionary<Guid, HealthRecordItemTypePermission>> QueryPermissionsByTypes(
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatform.QueryPermissionsByTypesAsync(Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
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
        /// A list of <see cref="HealthRecordItemTypePermission"/>
        /// objects which represent the permissions that the current
        /// authenticated person has for the HealthRecordItemTypes specified
        /// in the current health record when using the current application.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty list is
        /// returned. If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type.
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
        public async Task<Collection<HealthRecordItemTypePermission>> QueryPermissions(
            IList<Guid> healthRecordItemTypeIds)
        {
            return await HealthVaultPlatform.QueryPermissionsAsync(Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
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
        /// which contains a collection of <see cref="HealthRecordItemTypePermission"/> objects and
        /// other permission settings.
        /// </returns>
        ///
        /// <remarks>
        /// If the list of health record item types is empty, an empty list is
        /// returned for <see cref="HealthRecordPermissions"/> object's ItemTypePermissions property.
        /// If for a health record item type, the person has
        /// neither online access nor offline access permissions,
        /// HealthRecordItemTypePermission object is not returned for that
        /// health record item type.
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
            return await HealthVaultPlatform.QueryRecordPermissionsAsync(Connection, this, healthRecordItemTypeIds).ConfigureAwait(false);
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
        /// last updating application is authorized by the the last updating person to
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
        /// A List of HealthRecordItems representing the valid group memberships.
        /// </returns>
        /// <exception cref="HealthServiceException">
        /// If an error occurs while contacting the HealthVault service.
        /// </exception>
        public async Task<Collection<HealthRecordItem>> GetValidGroupMembership(IList<Guid> applicationIds)
        {
            return await HealthVaultPlatform.GetValidGroupMembershipAsync(Connection, this, applicationIds).ConfigureAwait(false);
        }
    }
}