// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.DesktopWeb.Common
{
    /// <summary>
    /// Represents a data table that populates itself with HealthVault data.
    /// </summary>
    ///
    [Serializable]
    public class ThingDataTable : DataTable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ThingDataTable"/>
        /// class with the specified table view and filter.
        /// </summary>
        ///
        /// <param name="view">
        /// The view that the data table should take on the data.
        /// </param>
        ///
        /// <param name="query">
        /// The filter used to gather things from the HealthVault
        /// service.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="query"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="view"/> parameter is
        /// <see cref="ThingDataTableView.SingleTypeTable"/> and
        /// the <paramref name="query"/> parameter contains more than one type
        /// identifier.
        /// </exception>
        public ThingDataTable(
            ThingDataTableView view,
            ThingQuery query)
        {
            Validator.ThrowIfArgumentNull(query, "filter", "DataTableFilterNull");

            Validator.ThrowArgumentExceptionIf(
                view == ThingDataTableView.SingleTypeTable &&
                query.TypeIds.Count > 1,
                "view",
                "DataTableViewInvalid");

            this.query = query;
            _view = view;
        }

        /// <summary>
        /// Fills in the data table with data from a list of ThingBase.
        /// </summary>
        /// <param name="record"/>
        /// <param name="items"/>
        /// <param name="startIndex"/>
        /// <param name="count"/>
        public async Task GetDataAsync(HealthRecordAccessor record, IList<ThingBase> items, int startIndex, int count)
        {
            ThingDataTableView effectiveView =
                await this.ApplyEffectiveViewAsync(record.Connection).ConfigureAwait(false);

            IDictionary<Guid, ThingTypeDefinition> typeDefDict =
                await ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(query.TypeIds,
                    record.Connection).ConfigureAwait(false);
            ThingTypeDefinition sttTypeDef =
                typeDefDict.Count == 1 ? typeDefDict[query.TypeIds[0]] : null;

            bool firstRow = true;
            string transformName =
                (effectiveView == ThingDataTableView.SingleTypeTable) ? "stt" : "mtt";

            for (int i = startIndex; i < items.Count && i < count; ++i)
            {
                ThingBase item = items[i];

                XPathNavigator itemTransformNav;
                IDictionary<string, XDocument> transformedXmlData = item.TransformedXmlData;
                if (transformedXmlData.ContainsKey(transformName))
                {
                    itemTransformNav =
                        transformedXmlData[transformName].CreateNavigator().SelectSingleNode(
                            "//data-xml/row");
                }
                else
                {
                    string transform = (sttTypeDef == null) ?
                        ThingTypeDefinitionHelper.Create(typeDefDict[item.TypeId]).TransformItem(transformName, item) :
                        ThingTypeDefinitionHelper.Create(sttTypeDef).TransformItem(transformName, item);

                    XmlReaderSettings settings = new XmlReaderSettings()
                    {
                        XmlResolver = null,
                        DtdProcessing = DtdProcessing.Prohibit
                    };

                    using (StringReader stringReader = new StringReader(transform))
                    {
                        itemTransformNav = new XPathDocument(XmlReader.Create(stringReader, settings)).CreateNavigator();
                    }

                    if (!itemTransformNav.MoveToFirstChild())
                    {
                        continue;
                    }
                }

                if (firstRow)
                {
                    SetupColumns(itemTransformNav.Clone());
                    firstRow = false;
                }
                AddRow(itemTransformNav);
            }
        }

        /// <summary>
        /// Fills in the data table with data from the HealthVault service.
        /// </summary>
        ///
        /// <param name="recordId">
        /// The unique health record identifier to get the data from.
        /// </param>
        ///
        /// <param name="connection">
        /// The connection to the HealthVault service to use.
        /// </param>
        ///
        /// <remarks>
        /// This method makes a web-method call to the HealthVault service.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// An error occurred while accessing the HealthVault service.
        /// </exception>
        ///
        public async Task GetData(
            Guid recordId,
            IConnectionInternal connection)
        {
            HealthRecordAccessor record =
                new HealthRecordAccessor(connection, recordId);
            await GetDataAsync(record).ConfigureAwait(false);
        }

        /// <summary>
        /// Fills in the data table with data from the HealthVault service.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to get the data from.
        /// </param>
        ///
        /// <remarks>
        /// This method makes a web-method call to the HealthVault service.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// An error occurred while accessing the HealthVault service.
        /// </exception>
        ///
        public async Task GetDataAsync(HealthRecordAccessor record)
        {
            await GetData(record, 0, Int32.MaxValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Fills in the data table with data from the HealthVault service
        /// starting at the specific index for the count specified.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to get the data from.
        /// </param>
        ///
        /// <param name="startIndex">
        /// The index to start retrieving full data from HealthVault.
        /// </param>
        ///
        /// <param name="count">
        /// The count of full items to retrieve.
        /// </param>
        ///
        /// <remarks>
        /// This method makes a web-method call to the HealthVault service.
        ///
        /// The default <see cref="GetData(HealthRecordAccessor)"/> implementation
        /// fills the data with complete information for all items matching
        /// the filter. If the <see cref="ThingDataTable"/> is being
        /// bound to a HealthServiceDataGrid or other such control that supports
        /// paging, this may not be the desired result as many calls to
        /// HealthVault may be required to fetch all the data.  This overload
        /// of GetData allows the caller to specify the index and the count of
        /// the full items to retrieve to match the page that is currently visible.
        /// The <see cref="ThingDataTable"/> will be filled with
        /// empty values except for the rows specified.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// An error occurred while accessing the HealthVault service.
        /// </exception>
        ///
        public async Task GetData(
            HealthRecordAccessor record,
            int startIndex,
            int count)
        {
            ThingDataTableView effectiveView =
                await this.ApplyEffectiveViewAsync(record.Connection).ConfigureAwait(false);

            // Need to specify the type version to ensure that the columns match when the app
            // supports multiple versions.
            if (effectiveView == ThingDataTableView.SingleTypeTable)
            {
                for (int index = 0; index < this.Query.TypeIds.Count; ++index)
                {
                    this.Query.View.TypeVersionFormat.Add(this.Query.TypeIds[index]);
                }
            }

            HealthRecordSearcher searcher = record.CreateSearcher();
            searcher.Filters.Add(this.Query);

            XPathNavigator nav = await searcher.GetMatchingItemsRaw().ConfigureAwait(false);

            _hasData = true;

            XPathNavigator navFiltered =
                nav.SelectSingleNode("//group/filtered");

            if (navFiltered != null)
            {
                _wasFiltered = navFiltered.ValueAsBoolean;
            }

            int numberOfFullThingsToRetrieve = AddRows(nav);

            List<ThingKey> partialThingKeys =
                GetPartialThingKeys(nav);

            int thingIndex = numberOfFullThingsToRetrieve;
            int partialThingsCurrentIndex = 0;

            while (thingIndex < startIndex &&
                    partialThingsCurrentIndex < partialThingKeys.Count)
            {
                AddPartialThingRow(partialThingKeys[partialThingsCurrentIndex++]);
                ++thingIndex;
            }

            while (thingIndex < startIndex + count &&
                   partialThingsCurrentIndex < partialThingKeys.Count)
            {
                nav =
                    await GetPartialThings(
                        record,
                        partialThingKeys,
                        partialThingsCurrentIndex,
                        numberOfFullThingsToRetrieve).ConfigureAwait(false);

                // Note, not all partial things may still exist when doing
                // the next query so AddRows may return less than
                // numberOfFullThingsToRetrieve. Just skip anything that is
                // missing.
                AddRows(nav);

                partialThingsCurrentIndex += numberOfFullThingsToRetrieve;
                thingIndex += numberOfFullThingsToRetrieve;
            }

            while (partialThingsCurrentIndex < partialThingKeys.Count)
            {
                AddPartialThingRow(partialThingKeys[partialThingsCurrentIndex++]);
                ++thingIndex;
            }
        }

        private bool _isFirstRow = true;

        /// <summary>
        /// Gets a value indicating whether there is any signed health data in the table.
        /// </summary>
        public bool HasSignedData
        {
            get { return _hasSignedData; }
        }
        private bool _hasSignedData;

        /// <summary>
        /// Gets a value indicating whether there is any personal health data in the table.
        /// </summary>
        public bool HasPersonalData
        {
            get { return _hasPersonalData; }
        }
        private bool _hasPersonalData;

        private int AddRows(XPathNavigator nav)
        {
            int rowsAdded = 0;

            XPathNodeIterator rowIterator = nav.Select("//data-xml/row");
            foreach (XPathNavigator rowNav in rowIterator)
            {
                if (_isFirstRow)
                {
                    SetupColumns(rowNav.Clone());
                    _isFirstRow = false;
                }
                AddRow(rowNav);
                ++rowsAdded;
            }
            return rowsAdded;
        }

        private void AddRow(XPathNavigator rowNav)
        {
            DataRow row = this.NewRow();
            foreach (DataColumn column in this.Columns)
            {
                string columnName = column.ColumnName;
                string columnValue = rowNav.GetAttribute(columnName, String.Empty);

                if (columnValue.Length == 0)
                {
                    row[column.Ordinal] = ItemTypeDataColumn.GetNotPresentValue(column.DataType);
                }
                else
                {
                    row[column.Ordinal] = columnValue;
                    if (!this._hasSignedData && columnName == "wc-issigned" &&
                        columnValue.Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
                    {
                        this._hasSignedData = true;
                    }
                    if (!this._hasPersonalData && columnName == "wc-ispersonal" &&
                        columnValue.Equals(Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
                    {
                        this._hasPersonalData = true;
                    }
                }
            }

            this.Rows.Add(row);
        }

        private void AddPartialThingRow(ThingKey key)
        {
            DataRow row = this.NewRow();
            foreach (DataColumn column in this.Columns)
            {
                switch (column.ColumnName)
                {
                    case "wc-id":
                        row[column.Ordinal] = key.Id.ToString();
                        break;

                    case "wc-version":
                        row[column.Ordinal] = key.VersionStamp.ToString();
                        break;

                    default:
                        row[column.Ordinal] =
                            ItemTypeDataColumn.GetNotPresentValue(column.DataType);
                        break;
                }
            }
            this.Rows.Add(row);
        }

        private void SetupColumns(XPathNavigator rowNav)
        {
            if (rowNav.MoveToFirstAttribute())
            {
                do
                {
                    if (_displayColumns.ContainsKey(rowNav.Name))
                    {
                        this.Columns.Add(_displayColumns[rowNav.Name]);
                    }
                    else
                    {
                        this.Columns.Add(
                            new DataColumn(rowNav.Name, typeof(String)));
                    }
                } while (rowNav.MoveToNextAttribute());
            }
        }

        private async Task<ThingDataTableView> ApplyEffectiveViewAsync(
            IConnectionInternal connection)
        {
            ThingDataTableView effectiveView =
                ThingDataTableView.MultipleTypeTable;

            ThingTypeDefinition typeDefinition = null;

            if (Query.TypeIds.Count == 1 &&
                View != ThingDataTableView.MultipleTypeTable)
            {
                typeDefinition =
                    await ItemTypeManager.GetHealthRecordItemTypeDefinitionAsync(
                        this.Query.TypeIds[0],
                        connection).ConfigureAwait(false);

                ThingTypeDefinitionHelper thingTypeDefinitionHelper = null;

                if (typeDefinition != null)
                {
                    thingTypeDefinitionHelper = ThingTypeDefinitionHelper.Create(typeDefinition);
                }

                if (thingTypeDefinitionHelper != null &&
                    thingTypeDefinitionHelper.ColumnDefinitions.Count > 0)
                {
                    effectiveView
                        = ThingDataTableView.SingleTypeTable;
                    _singleTypeDefinition = typeDefinition;

                    foreach (
                        ItemTypeDataColumn column in
                        thingTypeDefinitionHelper.ColumnDefinitions)
                    {
                        _displayColumns.Add(
                            column.ColumnName, column.Clone());
                    }

                    this.Query.View.TransformsToApply.Clear();
                    this.Query.View.TransformsToApply.Add("stt");
                }
            }

            if (_singleTypeDefinition == null)
            {
                typeDefinition =
                    await ItemTypeManager.GetBaseHealthRecordItemTypeDefinitionAsync(
                        connection).ConfigureAwait(false);

                effectiveView
                    = ThingDataTableView.MultipleTypeTable;

                if (typeDefinition != null)
                {
                    var healthRecordItemTypeDefinitionHelper = ThingTypeDefinitionHelper.Create(typeDefinition);

                    foreach (
                        ItemTypeDataColumn column in
                        healthRecordItemTypeDefinitionHelper.ColumnDefinitions)
                    {
                        _displayColumns.Add(column.ColumnName, column.Clone());
                    }

                    this.Query.View.TransformsToApply.Clear();
                    this.Query.View.TransformsToApply.Add("mtt");
                }
            }

            return effectiveView;
        }

        private static List<ThingKey> GetPartialThingKeys(
            XPathNavigator nav)
        {
            List<ThingKey> partialThingKeys
                = new List<ThingKey>();

            XPathNodeIterator partialThingIterator =
                nav.Select("//unprocessed-thing-key-info/thing-id");
            foreach (XPathNavigator partialThingNav in partialThingIterator)
            {
                string versionStamp
                    = partialThingNav.GetAttribute(
                        "version-stamp", String.Empty);
                ThingKey key
                    = new ThingKey(
                        new Guid(partialThingNav.Value),
                        new Guid(versionStamp));
                partialThingKeys.Add(key);
            }
            return partialThingKeys;
        }

        private async Task<XPathNavigator> GetPartialThings(
            HealthRecordAccessor record,
            IList<ThingKey> thingKeys,
            int currentThingKeyIndex,
            int numberOfFullThingsToRetrieve)
        {
            HealthRecordSearcher searcher = record.CreateSearcher();
            ThingQuery query = new ThingQuery();

            for (int i = currentThingKeyIndex;
                 i < thingKeys.Count &&
                    i < currentThingKeyIndex + numberOfFullThingsToRetrieve;
                 i++)
            {
                query.ItemKeys.Add(thingKeys[i]);
            }
            query.View = this.Query.View;
            query.States = this.Query.States;
            query.CurrentVersionOnly = this.Query.CurrentVersionOnly;
            if (Query.OrderByClauses.Count > 0)
            {
                foreach (var orderByClause in Query.OrderByClauses)
                {
                    query.OrderByClauses.Add(orderByClause);
                }
            }

            searcher.Filters.Add(query);

            return await searcher.GetMatchingItemsRaw().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the definition for the type of items in the data table.
        /// </summary>
        ///
        /// <remarks>
        /// This value is set only if the single type table view is being
        /// shown.
        /// </remarks>
        ///
        public ThingTypeDefinition SingleTypeDefinition
        {
            get { return _singleTypeDefinition; }
        }
        private ThingTypeDefinition _singleTypeDefinition;

        /// <summary>
        /// Gets the display columns for the table.
        /// </summary>
        ///
        /// <remarks>
        /// This collection will be empty until
        /// <see cref="GetData(HealthRecordAccessor)"/> or
        /// <see cref="GetData(Guid,ApplicationConnection)"/> is called.
        /// </remarks>
        ///
        public Dictionary<string, ItemTypeDataColumn> DisplayColumns
        {
            get { return _displayColumns; }
        }
        private Dictionary<string, ItemTypeDataColumn> _displayColumns =
            new Dictionary<string, ItemTypeDataColumn>();

        /// <summary>
        /// Gets the view of the data that the table will show.
        /// </summary>
        ///
        public ThingDataTableView View
        {
            get { return _view; }
        }
        private ThingDataTableView _view;

        /// <summary>
        /// Gets or sets the filter to use when getting data from the
        /// health record.
        /// </summary>
        ///
        public ThingQuery Query
        {
            get
            {
                return query;
            }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Filter", "ArgumentNull");
                query = value;
            }
        }
        private ThingQuery query = new ThingQuery();

        /// <summary>
        /// Gets a value indicating whether the data was filtered by the
        /// HealthVault service.
        /// </summary>
        ///
        /// <remarks>
        /// This value is set only after
        /// <see cref="GetData(HealthRecordAccessor)"/>
        /// or <see cref="GetData(Guid,ApplicationConnection)"/> is called.
        /// </remarks>
        ///
        public bool WasFiltered
        {
            get { return _wasFiltered; }
        }
        private bool _wasFiltered;

        /// <summary>
        /// <b>true</b> if the data has been retrieved from the HealthVault service;
        /// otherwise, <b>false</b>.
        /// </summary>
        ///
        /// <remarks>
        /// This value is set only after
        /// <see cref="GetData(HealthRecordAccessor)"/>
        /// or <see cref="GetData(Guid,ApplicationConnection)"/> is called.
        /// <br/><br/>
        /// This property returns <b>false</b> if there was an error contacting
        /// the HealthVault service, but returns <b>true</b> if the call was
        /// made successfully but the filter produced no results.
        /// </remarks>
        ///
        public bool HasData
        {
            get { return _hasData; }
        }
        private bool _hasData;
    }
}
