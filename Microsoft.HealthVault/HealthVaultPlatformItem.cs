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

namespace Microsoft.HealthVault.PlatformPrimitives
{

    /// <summary>
    /// Provides low-level access to the HealthVault item operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set 
    /// HealthVaultPlatformItem.Current to a derived class to intercept all calls.
    /// </remarks>

    public class HealthVaultPlatformItem
    {
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
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
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
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }

        internal static HealthVaultPlatformItem Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformItem _current = new HealthVaultPlatformItem();
        private static HealthVaultPlatformItem _saved;

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
        public virtual void NewItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            IList<HealthRecordItem> items)
        {
            Validator.ThrowIfArgumentNull(items, "items", "NewItemsNullItem");

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "PutThings", 2, accessor);

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

            // Add the XML to the request.
            request.Parameters = infoXml.ToString();

            // Call the web-service
            request.Execute();

            // Now update the Id for the new item
            XPathNodeIterator thingIds =
                request.Response.InfoNavigator.Select(
                    GetThingIdXPathExpression(request.Response.InfoNavigator));

            int thingIndex = 0;
            foreach (XPathNavigator thingIdNav in thingIds)
            {
                if (items[thingIndex] != null)
                {
                    items[thingIndex].Key =
                        new HealthRecordItemKey(
                            new Guid(thingIdNav.Value),
                            new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", String.Empty)));
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
        public virtual void UpdateItems(
            ApplicationConnection connection,
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
                HealthServiceRequest request =
                    new HealthServiceRequest(connection, "PutThings", 2, accessor);

                // Add the XML to the request.
                request.Parameters = infoXml.ToString();

                // Call the web-service
                request.Execute();

                XPathNodeIterator thingIds =
                    request.Response.InfoNavigator.Select(
                        GetThingIdXPathExpression(request.Response.InfoNavigator));

                int index = 0;

                foreach (XPathNavigator thingIdNav in thingIds)
                {
                    HealthRecordItem thing = itemsToUpdate[index];
                    thing.Key = new HealthRecordItemKey(
                        new Guid(thingIdNav.Value),
                        new Guid(thingIdNav.GetAttribute(
                            "version-stamp", String.Empty)));
                    thing.ClearDirtyFlags();
                    ++index;
                }

            }
        }

        private static XPathExpression _thingIdPath =
            XPathExpression.Compile("/wc:info/thing-id");

        /// <summary>
        /// Returns the XPathExpression for the ThingId from the supplied XPathNavigator
        /// </summary>
        /// 
        /// <param name="infoNav"></param>
        /// 
        /// <returns></returns>
        /// 
        internal static XPathExpression GetThingIdXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                    "wc",
                    "urn:com.microsoft.wc.methods.response.PutThings");

            XPathExpression infoThingIdPathClone = null;
            lock (_thingIdPath)
            {
                infoThingIdPathClone = _thingIdPath.Clone();
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
        public virtual void RemoveItems(
            ApplicationConnection connection,
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

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "RemoveThings", 1, accessor);

            request.Parameters = parameters.ToString();
            request.Execute();
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
        public virtual ReadOnlyCollection<HealthRecordItemCollection> GetMatchingItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            ValidateFilters(searcher);

            return Execute(connection, accessor, searcher);
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
        public virtual XmlReader GetMatchingItemsReader(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceRequest request = PrepareRequest(connection, accessor, searcher);
            request.Execute();

            return request.Response.InfoReader;
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
        public virtual XPathNavigator GetMatchingItemsRaw(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceRequest request = PrepareRequest(connection, accessor, searcher);
            request.Execute();

            return request.Response.InfoNavigator;
        }

        /// <summary>
        /// Gets the health record items specified by the 
        /// <see cref="HealthRecordSearcher"/> and runs them through the specified 
        /// transform.
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
        /// <param name="transform">
        /// A URL to a transform to run on the resulting XML. This can be
        /// a fully-qualified URL or the name of one of the standard XSLs
        /// provided by the HealthVault system.
        /// </param>
        /// 
        /// <returns>
        /// The string resulting from performing the specified transform on
        /// the XML representation of the items.
        /// </returns>
        /// 
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// Any call to HealthVault may specify a transform to be run on the
        /// response XML. The transform can be specified as a XSL fragment or
        /// a well-known transform tag provided by the HealthVault service. If a
        /// XSL fragment is specified, it gets compiled and cached on the server.
        /// <br/>
        /// <br/>
        /// A final-xsl is useful when you want to convert the result from XML to
        /// HTML so that you can display the result directly in a web page.
        /// You may also use it to generate other data formats like CCR, CCD, CSV,
        /// RSS, etc.
        /// <br/>
        /// <br/>
        /// Transform fragments cannot contain embedded script. The following set
        /// of parameters are passed to all final-xsl transforms:<br/>
        /// <ul>
        ///     <li>currentDateTimeUtc - the date and time just before the transform 
        ///     started executing</li>
        ///     <li>requestingApplicationName - the name of the application that
        ///     made the request to HealthVault.</li>
        ///     <li>countryCode - the ISO 3166 country code from the request.</li>
        ///     <li>languageCode - the ISO 639-1 language code from the request.</li>
        ///     <li>personName - the name of the person making the request.</li>
        ///     <li>recordName - if the request identified a HealthVault record to 
        ///     be used, this parameter contains the name of that record.</li>
        /// </ul>
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="transform"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentException">
        /// <see cref="HealthRecordView.Sections"/> does not
        /// contain the XML section in the view.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// There is a failure retrieving the items.
        /// -or-
        /// No filters have been specified.
        /// </exception>
        /// 
        public virtual string GetTransformedItems(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher,
            string transform)
        {
            Validator.ThrowIfStringNullOrEmpty(transform, "transform");

            HealthServiceRequest request = PrepareRequest(connection, accessor, searcher);

            return request.ExecuteForTransform(transform);
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
        private static HealthServiceRequest PrepareRequest(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetThings", 3, accessor);

            request.Parameters = GetParametersXml(searcher);
            return request;
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
        static internal void ValidateFilters(HealthRecordSearcher searcher)
        {
            if (searcher.Filters.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError();

                error.Message =
                    ResourceRetriever.GetResourceString(
                        "HealthRecordSearcherInvalidFilter");

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter,
                        error);
                throw e;
            }

            for (int i = 0; i < searcher.Filters.Count; ++i)
            {
                searcher.Filters[i].ThrowIfNotValid();
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
        private static ReadOnlyCollection<HealthRecordItemCollection> Execute(
            ApplicationConnection connection,
            HealthRecordAccessor accessor,
            HealthRecordSearcher searcher)
        {
            HealthServiceRequest request = PrepareRequest(connection, accessor, searcher);

            request.Execute();

            XmlReader infoReader = request.Response.InfoReader;

            Collection<HealthRecordItemCollection> result =
                new Collection<HealthRecordItemCollection>();

            if ((infoReader != null) && (infoReader.ReadToDescendant("group")))
            {
                while (infoReader.Name == "group")
                {
                    XmlReader groupReader = infoReader.ReadSubtree();

                    groupReader.MoveToContent();

                    HealthRecordItemCollection resultGroup =
                        HealthRecordItemCollection.CreateResultGroupFromResponse(
                            accessor,
                            groupReader,
                            searcher.Filters);

                    groupReader.Close();

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
                HealthServiceResponseError error = new HealthServiceResponseError();
                error.Message =
                    ResourceRetriever.GetResourceString(
                        "HealthRecordSearcherNoFilters");

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter, error);
                throw e;

            }

            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                foreach (HealthRecordFilter filter in searcher.Filters)
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

