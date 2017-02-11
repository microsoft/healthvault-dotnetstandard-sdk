// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Health.Web;

namespace Microsoft.Health.Events
{
    /// <summary>
    /// Information describing a health record item changed notification.
    /// </summary>
    public class HealthRecordItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="CommonNotificationData" /> instance associated with this notification.
        /// </summary>
        public CommonNotificationData Common
        {
            get { return _common; }
            set { _common = value; }
        }
        private CommonNotificationData _common;

        /// <summary>
        /// The person id associated with this notification.
        /// </summary>
        public Guid PersonId
        {
            get { return _personId; }
            set { _personId = value; }
        }
        private Guid _personId;

        /// <summary>
        /// The record id associated with this notification.
        /// </summary>
        public Guid RecordId
        {
            get { return _recordId; }
            set { _recordId = value; }
        }
        private Guid _recordId;

        /// <summary>
        /// Gets the collection of items that were changed.
        /// </summary>
        public Collection<HealthRecordItemChangedItem> ChangedItems
        {
            get { return _changedItems; }
            internal set { _changedItems = value; }
        }
        private Collection<HealthRecordItemChangedItem> _changedItems;

        /// <summary>
        /// Fetch the <see cref="HealthRecordItem" /> instances that are specified
        /// in the ChangedItems collection.
        /// </summary>
        /// <remarks>
        /// After the operation has completed, see <see cref="HealthRecordItemChangedItem.Item" /> 
        /// to ues the fetched <see cref="HealthRecordItem" />
        /// 
        /// Items that have been removed from a record or are otherwise unaccessible will
        /// have a <see cref="HealthRecordItemChangedItem.Item" /> value of null.
        /// </remarks>
        public void GetItems()
        {
            GetItems(
                    HealthApplicationConfiguration.Current.ApplicationId,
                    HealthApplicationConfiguration.Current.HealthVaultUrl.OriginalString);
        }

        /// <summary>
        /// Fetch the <see cref="HealthRecordItem" /> instances that are specified
        /// in the ChangedItems collection.
        /// </summary>
        /// <remarks>
        /// After the operation has completed, see <see cref="HealthRecordItemChangedItem.Item" /> 
        /// to use the fetched <see cref="HealthRecordItem" />
        /// 
        /// Items that have been removed from a record or are otherwise inaccessible will
        /// have a <see cref="HealthRecordItemChangedItem.Item" /> value of null.
        /// </remarks>
        /// <param name="connection">The connection to use to fetch the instances.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        public void GetItems(HealthServiceConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");

            GetItems(connection.ApplicationId, connection.RequestUrl.OriginalString);
        }

        /// <summary>
        /// Fetch the <see cref="HealthRecordItem" /> instances that are specified
        /// in the ChangedItems collection.
        /// </summary>
        /// <remarks>
        /// After the operation has completed, see <see cref="HealthRecordItemChangedItem.Item" /> 
        /// to use the fetched <see cref="HealthRecordItem" />
        /// 
        /// Items that have been removed from a record or are otherwise inaccessible will
        /// have a <see cref="HealthRecordItemChangedItem.Item" /> value of null.
        /// </remarks>
        /// 
        /// <param name="applicationId">The application id to use.</param>
        /// <param name="healthServiceUrl">The health service URL to use.</param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="healthServiceUrl"/> parameter is <b>null</b>.
        /// </exception>
        public void GetItems(Guid applicationId, string healthServiceUrl)
        {
            Validator.ThrowIfArgumentNull(healthServiceUrl, "healthServiceUrl", "HealthServiceUrlNull");

            if (ChangedItems.Count == 0)
            {
                return;
            }

            OfflineWebApplicationConnection offlineConn =
                new OfflineWebApplicationConnection(
                    applicationId,
                    healthServiceUrl,
                    _personId);

            offlineConn.Authenticate();

            HealthRecordAccessor accessor = new HealthRecordAccessor(offlineConn, _recordId);

            HealthRecordSearcher searcher = accessor.CreateSearcher();
            HealthRecordFilter filter = new HealthRecordFilter();
            searcher.Filters.Add(filter);

            Dictionary<Guid, HealthRecordItemChangedItem> changedLookup =
                new Dictionary<Guid, HealthRecordItemChangedItem>(ChangedItems.Count);

            foreach (HealthRecordItemChangedItem item in ChangedItems)
            {
                filter.ItemIds.Add(item.Id);
                changedLookup.Add(item.Id, item);
            }

            HealthRecordItemCollection things = searcher.GetMatchingItems()[0];

            // Take the resultant items and put them back in the ChangedItems collection

            foreach (HealthRecordItem fetchedItem in things)
            {
                HealthRecordItemChangedItem item = changedLookup[fetchedItem.Key.Id];
                item.Item = fetchedItem;
            }
        }
    }
}
