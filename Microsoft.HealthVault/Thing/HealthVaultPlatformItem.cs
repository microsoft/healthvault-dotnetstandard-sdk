// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;
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
    /// Provides low-level access to the HealthVault item operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set
    /// HealthVaultPlatformItem.Current to a derived class to intercept all calls.
    /// </remarks>
    internal class HealthVaultPlatformItem
    {
        private static readonly XPathExpression ThingIdPath =
            XPathExpression.Compile("/wc:info/thing-id");

        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        ///
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked.
        /// </remarks>
        ///
        /// <param name="mock">The mocking class.</param>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        ///
        public static void EnableMock(HealthVaultPlatformItem mock)
        {
            Validator.ThrowInvalidIf(saved != null, "ClassAlreadyMocked");

            saved = Current;
            Current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        ///
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        ///
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(saved, "ClassIsntMocked");

            Current = saved;
            saved = null;
        }

        internal static HealthVaultPlatformItem Current { get; private set; } = new HealthVaultPlatformItem();

        private static HealthVaultPlatformItem saved;

        /// <summary>
        /// Creates new health record items associated with the record.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
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
        public virtual async Task NewItemsAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItem> items)
        {
            Validator.ThrowIfArgumentNull(items, "items", "NewItemsNullItem");

            StringBuilder infoXml = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (HealthRecordItem item in items)
                {
                    Validator.ThrowIfArgumentNull(item, "items", "NewItemsNullItem");

                    item.WriteItemXml(infoXmlWriter);
                }

                infoXmlWriter.Flush();
            }

            // Call the web-service
            HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString()).ConfigureAwait(false);

            // Now update the Id for the new item
            XPathNodeIterator thingIds =
                responseData.InfoNavigator.Select(
                    GetThingIdXPathExpression(responseData.InfoNavigator));

            int thingIndex = 0;
            foreach (XPathNavigator thingIdNav in thingIds)
            {
                if (items[thingIndex] != null)
                {
                    items[thingIndex].Key =
                        new HealthRecordItemKey(
                            new Guid(thingIdNav.Value),
                            new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", string.Empty)));
                }

                thingIndex++;
            }
        }

        /// <summary>
        /// Updates the specified health record items in one batch call to
        /// the service.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
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
        public virtual async Task UpdateItemsAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItem> itemsToUpdate)
        {
            Validator.ThrowIfArgumentNull(itemsToUpdate, "itemsToUpdate", "UpdateItemsArgumentNull");

            StringBuilder infoXml = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            bool somethingRequiresUpdate = false;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (HealthRecordItem item in itemsToUpdate)
                {
                    Validator.ThrowIfArgumentNull(item, "items", "UpdateItemsArgumentNull");

                    Validator.ThrowArgumentExceptionIf(
                        item.Key == null,
                        "itemsToUpdate",
                        "UpdateItemsWithNoId");

                    if (item.WriteItemXml(infoXmlWriter, false))
                    {
                        somethingRequiresUpdate = true;
                    }
                }

                infoXmlWriter.Flush();
            }

            if (somethingRequiresUpdate)
            {
                // Call the web-service
                HealthServiceResponseData responseData = await connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString()).ConfigureAwait(false);

                XPathNodeIterator thingIds =
                    responseData.InfoNavigator.Select(
                        GetThingIdXPathExpression(responseData.InfoNavigator));

                int index = 0;

                foreach (XPathNavigator thingIdNav in thingIds)
                {
                    HealthRecordItem healthRecordItem = itemsToUpdate[index];
                    healthRecordItem.Key = new HealthRecordItemKey(
                        new Guid(thingIdNav.Value),
                        new Guid(thingIdNav.GetAttribute(
                            "version-stamp", string.Empty)));
                    healthRecordItem.ClearDirtyFlags();
                    ++index;
                }
            }
        }

        /// <summary>
        /// Returns the XPathExpression for the ThingId from the supplied XPathNavigator
        /// </summary>
        /// <param name="infoNav">the XPathNavigator for the associated thing</param>
        /// <returns>The XPathExpression</returns>
        ///
        internal static XPathExpression GetThingIdXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                    "wc",
                    "urn:com.microsoft.wc.methods.response.PutThings");

            XPathExpression infoThingIdPathClone;
            lock (ThingIdPath)
            {
                infoThingIdPathClone = ThingIdPath.Clone();
            }

            infoThingIdPathClone.SetContext(infoXmlNamespaceManager);

            return infoThingIdPathClone;
        }

        /// <summary>
        /// Marks the specified health record item as deleted.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
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
        public virtual async Task RemoveItemsAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItemKey> itemsToRemove)
        {
            Validator.ThrowArgumentExceptionIf(
                itemsToRemove == null || itemsToRemove.Count == 0,
                "itemsToRemove",
                "RemoveItemsListNullOrEmpty");

            StringBuilder parameters = new StringBuilder(128 * itemsToRemove.Count);
            for (int i = 0; i < itemsToRemove.Count; ++i)
            {
                parameters.Append("<thing-id version-stamp=\"");
                parameters.Append(itemsToRemove[i].VersionStamp);
                parameters.Append("\">");
                parameters.Append(itemsToRemove[i].Id);
                parameters.Append("</thing-id>");
            }

            await connection.ExecuteAsync(HealthVaultMethods.RemoveThings, 1, parameters.ToString()).ConfigureAwait(false);
        }

        #region GetThings

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="searcher">
        /// The searcher that defines what items to return.
        /// </param>
        ///
        /// <returns>
        /// A collection of health record items that match the applied filters.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The response from the server was anything but
        /// <see cref="HealthServiceStatusCode.Ok"/>.
        /// -or-
        /// <see cref="HealthRecordSearcher.Filters"/> is empty
        /// or contains invalid filters.
        /// </exception>
        ///
        public virtual async Task<ReadOnlyCollection<HealthRecordItemCollection>> GetMatchingItemsAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            ValidateFilters(searcher);

            return await this.ExecuteAsync(connection, accessor, searcher).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="searcher">
        /// The searcher that defines what items to return.
        /// </param>
        ///
        /// <returns>
        /// An XmlReader representing the raw results of the search.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// This method is typically used when the calling application wants to
        /// handle the raw health record item XML directly instead of using the
        /// object model.
        /// </remarks>
        ///
        public virtual async Task<XmlReader> GetMatchingItemsReaderAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceResponseData responseData = await this.ExecuteGetThingsRequest(connection, accessor, searcher).ConfigureAwait(false);

            return responseData.InfoReader;
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
        ///
        /// <param name="connection">
        /// The connection to use to access the data.
        /// </param>
        ///
        /// <param name="accessor">
        /// The record to use.
        /// </param>
        ///
        /// <param name="searcher">
        /// The searcher that defines what items to return.
        /// </param>
        ///
        /// <returns>
        /// An XPathNavigator representing the raw results of the search.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// This method is typically used when the calling application wants to
        /// handle the raw health record item XML directly instead of using the
        /// object model.
        /// </remarks>
        ///
        public virtual async Task<XPathNavigator> GetMatchingItemsRawAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceResponseData responseData = await this.ExecuteGetThingsRequest(connection, accessor, searcher).ConfigureAwait(false);

            return responseData.InfoNavigator;
        }

        /// <summary>
        /// Gets the request object including the necessary parameters for
        /// the "GetThings" request.
        /// </summary>
        ///
        /// <returns>
        /// The request object for the thing search with the parameters filled
        /// in.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// No filters have been specified.
        /// </exception>
        ///
        private async Task<HealthServiceResponseData> ExecuteGetThingsRequest(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            return await connection.ExecuteAsync(HealthVaultMethods.GetThings, 3, GetParametersXml(searcher)).ConfigureAwait(false);
        }

        /// <summary>
        /// Checks whether at least one filter was specified
        /// and that all specified filters are valid.
        /// </summary>
        ///
        /// <exception cref="HealthServiceResponseError">
        /// If no filter was specified or if any specified filter was invalid.
        /// </exception>
        ///
        internal static void ValidateFilters(HealthRecordSearcher searcher)
        {
            if (searcher.Filters.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = ResourceRetriever.GetResourceString(
                        "HealthRecordSearcherInvalidFilter")
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter,
                        error);
                throw e;
            }

            foreach (ThingQuery filter in searcher.Filters)
            {
                filter.ThrowIfNotValid();
            }
        }

        /// <summary>
        /// Executes the search using the filters supplied.
        /// </summary>
        ///
        /// <exception cref="HealthServiceException">
        /// The response from the server was anything but
        /// HealthServiceStatusCode.OK, or no filters have been specified.
        /// </exception>
        ///
        private async Task<ReadOnlyCollection<HealthRecordItemCollection>> ExecuteAsync(
            IConnectionInternal connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceResponseData responseData = await this.ExecuteGetThingsRequest(connection, accessor, searcher).ConfigureAwait(false);

            XmlReader infoReader = responseData.InfoReader;

            Collection<HealthRecordItemCollection> result =
                new Collection<HealthRecordItemCollection>();

            if ((infoReader != null) && infoReader.ReadToDescendant("group"))
            {
                while (infoReader.Name == "group")
                {
                    using (XmlReader groupReader = infoReader.ReadSubtree())
                    {
                        groupReader.MoveToContent();

                        HealthRecordItemCollection resultGroup =
                            HealthRecordItemCollection.CreateResultGroupFromResponse(
                                accessor,
                                groupReader,
                                searcher.Filters);

                        // infoReader will normally be at the end element of
                        // the group at this point, and needs a read to get to
                        // the next element. If the group was empty, infoReader
                        // will be at the beginning of the group, and a
                        // single read will still move to the next element.
                        infoReader.Read();
                        if (resultGroup != null)
                        {
                            result.Add(resultGroup);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<HealthRecordItemCollection>(result);
        }

        /// <summary>
        /// Generates the XML for the parameters for the "GetThings" request.
        /// </summary>
        ///
        /// <returns>
        /// An XML string defining the parameters for the "GetThings" call.
        /// </returns>
        ///
        /// <exception cref="HealthServiceException">
        /// No filters have been specified.
        /// </exception>
        ///
        internal static string GetParametersXml(HealthRecordSearcher searcher)
        {
            if (searcher.Filters.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = ResourceRetriever.GetResourceString(
                        "HealthRecordSearcherNoFilters")
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter, error);
                throw e;
            }

            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                foreach (ThingQuery filter in searcher.Filters)
                {
                    // Add all filters
                    filter.AddFilterXml(writer);
                }

                writer.Flush();
            }

            return parameters.ToString();
        }

        #endregion GetThings
    }
}
