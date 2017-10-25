// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Searches for things in HealthVault records.
    /// </summary>
    ///
    /// <remarks>
    /// This class wraps up the logic for constructing a "GetThings" query
    /// against the HealthVault service.  It generates the necessary XML to
    /// call the "GetThings" and retrieve things that match the specified
    /// criteria.
    /// </remarks>
    ///
    internal class HealthRecordSearcher
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
        internal HealthRecordSearcher(HealthRecordAccessor record)
        {
            Validator.ThrowIfArgumentNull(record, nameof(record), Resources.HealthRecordSearcherCtorArgumentNull);
            Record = record;
        }

        /// <summary>
        /// Gets the health record that is being searched for things.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthRecordAccessor"/> representing the record.
        /// </value>
        ///
        /// <remarks>
        /// The authenticated person must have
        /// <see cref="ThingPermissions.Read"/> access rights to the
        /// health record to get results from the query.
        /// </remarks>
        ///
        public HealthRecordAccessor Record { get; }

        /// <summary>
        /// Gets the filters associated with the search.
        /// </summary>
        ///
        /// <remarks>
        /// To add a search filter, call the Add method of the
        /// returned collection.
        /// </remarks>
        ///
        public Collection<ThingQuery> Filters { get; } = new Collection<ThingQuery>();

        /// <summary>
        /// Gets the things specified by the
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
                error.Message = Resources.HealthRecordSearcherNoFilters;

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter, error);
                throw e;
            }

            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                foreach (ThingQuery filter in Filters)
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
