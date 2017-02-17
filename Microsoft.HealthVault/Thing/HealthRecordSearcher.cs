// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Searches for health record items in HealthVault records.
    /// </summary>
    ///
    /// <remarks>
    /// This class wraps up the logic for constructing a "GetThings" query
    /// against the HealthVault service.  It generates the necessary XML to
    /// call the "GetThings" and retrieve health record items that match the specified
    /// criteria.
    /// </remarks>
    ///
    public class HealthRecordSearcher
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordSearcher"/>
        /// class with the default parameters.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthRecordSearcher(HealthRecordAccessor record)
        {
            Validator.ThrowIfArgumentNull(record, "record", "HealthRecordSearcherCtorArgumentNull");
            _record = record;
        }

        /// <summary>
        /// Gets the health record that is being searched for health record items.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthRecordAccessor"/> representing the record.
        /// </value>
        ///
        /// <remarks>
        /// The authenticated person must have
        /// <see cref="HealthRecordItemPermissions.Read"/> access rights to the
        /// health record to get results from the query.
        /// </remarks>
        ///
        public HealthRecordAccessor Record
        {
            get { return _record; }
        }
        private HealthRecordAccessor _record;

        /// <summary>
        /// Gets the filters associated with the search.
        /// </summary>
        ///
        /// <remarks>
        /// To add a search filter, call the Add method of the
        /// returned collection.
        /// </remarks>
        ///
        public Collection<HealthRecordFilter> Filters
        {
            get { return _filters; }
        }
        private Collection<HealthRecordFilter> _filters =
            new Collection<HealthRecordFilter>();

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
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
        public async Task<ReadOnlyCollection<HealthRecordItemCollection>> GetMatchingItems()
        {
            return await HealthVaultPlatform.GetMatchingItemsAsync(Record.Connection, Record, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
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
        public async Task<XmlReader> GetMatchingItemsReader()
        {
            return await HealthVaultPlatform.GetMatchingItemsReaderAsync(Record.Connection, Record, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the health record items that match the filters as specified by
        /// the properties of this class.
        /// </summary>
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
        public async Task<XPathNavigator> GetMatchingItemsRaw()
        {
            return await HealthVaultPlatform.GetMatchingItemsRawAsync(Record.Connection, Record, this).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a single health record item from the associated record by
        /// using the item identifier.
        /// </summary>
        ///
        /// <param name="itemId">
        /// The unique identifier for the health record item.
        /// </param>
        ///
        /// <param name="sections">
        /// The data sections of the health record item that should be retrieved.
        /// </param>
        ///
        /// <returns>
        /// An instance of a <see cref="HealthRecordItem"/>
        /// representing the health record item with the specified identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// <br/><br/>
        /// All filters are cleared and replaced with a single filter
        /// for the specified item.
        /// </remarks>
        ///
        /// <exception cref="HealthServiceException">
        /// The server returned something other than a code of
        /// HealthServiceStatusCode.OK, or the result count did not equal one (1).
        /// -or-
        /// <see cref="HealthRecordSearcher.Filters"/> is empty
        /// or contains invalid filters.
        /// </exception>
        ///
        public async Task<HealthRecordItem> GetSingleItem(
            Guid itemId,
            HealthRecordItemSections sections)
        {
            // Create a new searcher to get the item.
            HealthRecordSearcher searcher = new HealthRecordSearcher(Record);

            HealthRecordFilter filter = new HealthRecordFilter();
            filter.ItemIds.Add(itemId);
            filter.View.Sections = sections;
            filter.CurrentVersionOnly = true;

            searcher.Filters.Add(filter);

            ReadOnlyCollection<HealthRecordItemCollection> resultSet =
                await HealthVaultPlatform.GetMatchingItemsAsync(Record.Connection, Record, searcher).ConfigureAwait(false);

            // Check in case HealthVault returned invalid data.
            if (resultSet.Count > 1)
            {
                HealthServiceResponseError error = new HealthServiceResponseError();
                error.Message =
                    ResourceRetriever.GetResourceString(
                        "GetSingleThingTooManyResults");

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.MoreThanOneThingReturned,
                        error);
                throw e;
            }

            HealthRecordItem result = null;
            if (resultSet.Count == 1)
            {
                HealthRecordItemCollection resultGroup = resultSet[0];

                if (resultGroup.Count > 1)
                {
                    HealthServiceResponseError error = new HealthServiceResponseError();
                    error.Message =
                        ResourceRetriever.GetResourceString(
                            "GetSingleThingTooManyResults");

                    HealthServiceException e =
                        HealthServiceExceptionHelper.GetHealthServiceException(
                            HealthServiceStatusCode.MoreThanOneThingReturned,
                            error);
                    throw e;
                }

                if (resultGroup.Count == 1)
                {
                    result = resultGroup[0];
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the health record items specified by the
        /// <see cref="Filters"/> and runs them through the specified
        /// transform.
        /// </summary>
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
        public async Task<string> GetTransformedItems(string transform)
        {
            return await HealthVaultPlatform.GetTransformedItemsAsync(Record.Connection, Record, this, transform).ConfigureAwait(false);
        }

        #region helpers

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
        internal string GetParametersXml()
        {
            if (Filters.Count == 0)
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
                foreach (HealthRecordFilter filter in Filters)
                {
                    // Add all filters
                    filter.AddFilterXml(writer);
                }
                writer.Flush();
            }
            return parameters.ToString();
        }

        #endregion helpers
    }
}
