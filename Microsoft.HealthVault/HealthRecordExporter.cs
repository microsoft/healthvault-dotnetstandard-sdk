// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Exports HealthVault record items.
    /// </summary>
    ///
    /// <remarks>
    /// This class will return results in HealthVault XML format. Derived classes should override...
    /// </remarks>
    ///
    public class HealthRecordExporter
    {
        /// <summary>
        /// Constructs an instance of a HealthRecordExporter for the specified
        /// health record.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to export data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthRecordExporter(HealthRecordAccessor record)
        {
            Validator.ThrowIfArgumentNull(record, "record", "HealthRecordExporterCtorArgumentNull");

            _record = record;
        }

        /// <summary>
        /// Constructs an instance of a HealthRecordExporter for the specified
        /// health record and the specified transform.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to export data from.
        /// </param>
        ///
        /// <param name="transform">
        /// The transform used to convert HealthVault XML to the destination format.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthRecordExporter(HealthRecordAccessor record, XslCompiledTransform transform)
        {
            Validator.ThrowIfArgumentNull(record, "record", "HealthRecordExporterCtorArgumentNull");

            _record = record;
            _transform = transform;
        }

        /// <summary>
        /// Constructs an instance of a HealthRecordExporter for the specified
        /// health record and the specified transform tag.
        /// </summary>
        ///
        /// <param name="record">
        /// The health record to export data from.
        /// </param>
        ///
        /// <param name="transformTag">
        /// The name of the transform to be retrieved from HealthVault to convert
        /// the HealthVault XML to the destination format. For example, "toccr" will
        /// convert the data to the Continuity of Care Record XML format.
        /// </param>
        ///
        /// <remarks>
        /// Note, this constructor makes a call to HealthVault to retrieve the specified
        /// data transform. The web request may cause a variety of WebExceptions to be thrown.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="record"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="transformTag"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="WebException">
        /// If the transform with the specified <paramref name="transformTag"/>
        /// could not be found.
        /// </exception>
        ///
        public HealthRecordExporter(HealthRecordAccessor record, string transformTag)
        {
            Validator.ThrowIfArgumentNull(record, "record", "HealthRecordExporterCtorArgumentNull");
            Validator.ThrowIfStringNullOrEmpty(transformTag, "transformTag");

            _record = record;
            _transform = GetTransform(transformTag);
        }

        private XslCompiledTransform GetTransform(string tag)
        {
            Uri url = HealthServiceLocation.GetServiceBaseUrl(Record.Connection.RequestUrl);
            url = new Uri(url.OriginalString + "/xsl/" + HttpUtility.UrlEncode(tag) + ".xsl");

            XslCompiledTransform transform = new XslCompiledTransform(false);
            XsltSettings settings = new XsltSettings(true, true);
            transform.Load(url.OriginalString, settings, null);
            return transform;
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
        public Collection<HealthRecordFilter> Filters { get; } = new Collection<HealthRecordFilter>();

        /// <summary>
        ///
        /// </summary>
        public XslCompiledTransform Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
        private XslCompiledTransform _transform;

        /// <summary>
        ///
        /// </summary>
        public XsltArgumentList TransformArgumentList
        {
            get { return _transformArgumentList; }
        }
        private XsltArgumentList _transformArgumentList = new XsltArgumentList();

        /// <summary>
        /// </summary>
        ///
        /// <returns>
        /// </returns>
        ///
        /// <remarks>
        /// This method accesses the HealthVault service across the network.
        /// If the filters specified do not reduce the amount of data being retrieved from the
        /// record, this method could take a significant amount of time as data gets paged into
        /// memory from HealthVault.
        ///
        /// Note: There may be data in items that HealthVault does not include when converting
        /// to the CCR or CCD formats.
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
        public string ExportItems()
        {
            HealthRecordSearcher searcher = new HealthRecordSearcher(_record);

            for (int index = 0; index < Filters.Count; ++index)
            {
                searcher.Filters.Add(Filters[index]);
            }

            ReadOnlyCollection<HealthRecordItemCollection> results = searcher.GetMatchingItems();

            StringBuilder resultXml = new StringBuilder(1000);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.ConformanceLevel = ConformanceLevel.Document;

            using (XmlWriter writer = XmlWriter.Create(resultXml, settings))
            {
                // <response>
                writer.WriteStartElement("response");

                // <wc:info>
                writer.WriteStartElement("wc", "info", "urn:com.microsoft.wc.methods.response.GetThings3");

                foreach (HealthRecordItemCollection items in results)
                {
                    // <group>
                    writer.WriteStartElement("group");

                    // We are leveraging the HealthRecordItemCollection paging here to retrieve
                    // all the data from HealthVault if it doesn't come with the first request.
                    for (int itemIndex = 0; itemIndex < items.Count; ++itemIndex)
                    {
                        bool filterApproved = true;
                        foreach (HealthRecordClientFilterHandler filter in ClientFilters)
                        {
                            filterApproved = filter(items[itemIndex]);
                            if (!filterApproved)
                            {
                                break;
                            }
                        }

                        if (filterApproved)
                        {
                            items[itemIndex].WriteItemXml(writer, true, "thing");
                        }
                    }

                    // </group>
                    writer.WriteEndElement();
                }

                // </wc:info>
                writer.WriteEndElement();

                // </response>
                writer.WriteEndElement();

                writer.Flush();
            }

            return TransformItemXml(resultXml.ToString());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="healthRecordItemsXml"></param>
        /// <returns></returns>
        protected virtual string TransformItemXml(string healthRecordItemsXml)
        {
            string result = healthRecordItemsXml;

            if (_transform != null)
            {
                AddStandardParametersToTransformArguments();

                XmlReaderSettings readerSettings = SDKHelper.XmlReaderSettings;

                using (XmlReader reader = SDKHelper.GetXmlReaderForXml(healthRecordItemsXml, readerSettings))
                {
                    reader.NameTable.Add("wc");

                    StringBuilder resultXml = new StringBuilder(1000);
                    XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
                    settings.ConformanceLevel = ConformanceLevel.Document;

                    using (XmlWriter writer = XmlWriter.Create(resultXml, settings))
                    {
                        _transform.Transform(reader, _transformArgumentList, writer, null);

                        writer.Flush();
                    }

                    result = resultXml.ToString();
                }
            }
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private void AddStandardParametersToTransformArguments()
        {
            if (_transformArgumentList.GetParam("currentDateTimeUtc", String.Empty) == null)
            {
                _transformArgumentList.AddParam(
                    "currentDateTimeUtc",
                    String.Empty,
                    DateTime.UtcNow);
            }

            if (_transformArgumentList.GetParam("requestingApplicationName", String.Empty) == null)
            {
                _transformArgumentList.AddParam(
                    "requestingApplicationName",
                    String.Empty,
                    HealthVaultPlatform.GetApplicationInfo(Record.Connection).Name);
            }

            if (_transformArgumentList.GetParam("languageCode", String.Empty) == null)
            {
                string language = Record.Connection.Culture.TwoLetterISOLanguageName;

                _transformArgumentList.AddParam(
                    "languageCode",
                    String.Empty,
                    language);
            }

            if (_transformArgumentList.GetParam("countryCode", String.Empty) == null)
            {
                string country = "US";
                string[] langAndCountry = Record.Connection.Culture.Name.Split('-');
                if (langAndCountry.Length > 1)
                {
                    country = langAndCountry[1];
                }
                _transformArgumentList.AddParam(
                    "countryCode",
                    String.Empty,
                    country);
            }

            if (_transformArgumentList.GetParam("personName", String.Empty) == null)
            {
                _transformArgumentList.AddParam(
                    "personName",
                    String.Empty,
                    HealthVaultPlatform.GetPersonInfo(Record.Connection).Name);
            }

            HealthRecordInfo info = Record as HealthRecordInfo;
            if (info != null)
            {
                if (_transformArgumentList.GetParam("recordName", String.Empty) == null)
                {
                    _transformArgumentList.AddParam(
                        "recordName",
                        String.Empty,
                        info.Name);
                }
            }
        }

        /// <summary>
        /// Client side filtering of the results of the HealthVault search for items.
        /// </summary>
        ///
        /// <remarks>
        /// If more than one handler is added to the filter, the item will only be deemed to
        /// pass the filter if all handlers return true.
        /// </remarks>
        ///
        public Collection<HealthRecordClientFilterHandler> ClientFilters
        {
            get { return _clientFilters; }
        }
        private Collection<HealthRecordClientFilterHandler> _clientFilters =
            new Collection<HealthRecordClientFilterHandler>();
    }

    /// <summary>
    /// Defines the method signature for client side filtering of HealthRecordItems.
    /// </summary>
    ///
    /// <param name="item">
    /// The <see cref="HealthRecordItem"/> to perform the filter test on.
    /// </param>
    ///
    /// <returns>
    /// True if the <paramref name="item"/> passes the filter.
    /// </returns>
    ///
    public delegate bool HealthRecordClientFilterHandler(HealthRecordItem item);
}
