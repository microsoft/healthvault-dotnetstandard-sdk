// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.MeaningfulUse
{
    /// <summary>
    /// A collection of <see cref="DOPUDocumentReceipt" />(s) which describe DOPU document receipt information
    /// for Meaningful Use Timely Access Report from HealthVault. 
    /// </summary>
    internal class DOPUDocumentReceiptCollection : ReportCollection<DOPUDocumentReceipt>
    {
        private HealthServiceConnection _connection;
        private DateRange _availableDateFilter;

        /// <summary>
        /// Constructs an instance of DOPUDocumentReceiptCollection with specified values.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection used to obtain the information from HealthVault.
        /// </param>
        /// 
        /// <param name="availableDatefilter">
        /// <see cref="DateRange"/> specifies the date range criteria to match when retrieving Timely Access Report.
        /// </param>
        /// 
        internal DOPUDocumentReceiptCollection(
            HealthServiceConnection connection,
            DateRange availableDatefilter)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");
            Validator.ThrowIfArgumentNull(availableDatefilter, "availableDatefilter", "FilterNull");

            _connection = connection;
            _availableDateFilter = availableDatefilter;
        }

        #region helpers

        /// <summary>
        /// Helper to get items from platform
        /// </summary>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// </exception>
        protected override void GetItemsFromPlatform()
        {
            XPathNavigator responseInfoNavigator = ExecuteRequest();
            XPathExpression infoPath =
                SDKHelper.GetInfoXPathExpressionForMethod(
                responseInfoNavigator,
                "GetMeaningfulUseTimelyAccessReport");

            XPathNavigator infoNav = responseInfoNavigator.SelectSingleNode(infoPath);
            ParseResponse(infoNav);
        }

        /// <summary>
        /// Helper to execute request
        /// </summary>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error. 
        /// </exception>
        private XPathNavigator ExecuteRequest()
        {
            HealthServiceRequest request = CreateRequest();
            request.Execute();

            return request.Response.InfoNavigator;
        }

        /// <summary>
        /// Helper to parse items from response
        /// </summary>
        /// 
        private void ParseResponse(XPathNavigator responseInfoNavigator)
        {
            XPathNodeIterator sourceNavs = responseInfoNavigator.Select("sources/source");

            if (sourceNavs != null)
            {
                foreach (XPathNavigator sourceNav in sourceNavs)
                {
                    string source = sourceNav.GetAttribute("id", String.Empty);
                    XPathNodeIterator activityNavs = sourceNav.Select("dopu-document-receipts/dopu-document-receipt");

                    if (sourceNavs != null)
                    {
                        foreach (XPathNavigator activityNav in activityNavs)
                        {
                            DOPUDocumentReceipt item = new DOPUDocumentReceipt(source);
                            item.ParseXml(activityNav);
                            Items.Add(item);
                        }
                    }
                }
            }

            string cursor = XPathHelper.GetOptNavValue(responseInfoNavigator, "cursor");
            if (!String.IsNullOrEmpty(cursor))
            {
                Cursor = cursor;
            }
            else
            {
                HasMoreItems = false;
            }
        }

        /// <summary>
        /// Helper to create request
        /// </summary>
        /// 
        private HealthServiceRequest CreateRequest()
        {
            HealthServiceRequest request = _connection.CreateRequest("GetMeaningfulUseTimelyAccessReport", 1);

            StringBuilder parameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                writer.WriteStartElement("dopu-document-filter");
                writer.WriteStartElement("available-date-filter");

                writer.WriteElementString("min-date", XmlConvert.ToString(_availableDateFilter.Start, XmlDateTimeSerializationMode.Utc));
                writer.WriteElementString("max-date", XmlConvert.ToString(_availableDateFilter.End, XmlDateTimeSerializationMode.Utc));

                writer.WriteEndElement();
                writer.WriteEndElement();

                if (!String.IsNullOrEmpty(Cursor))
                {
                    writer.WriteElementString("cursor", Cursor);
                }
            }

            request.Parameters = parameters.ToString();

            return request;
        }
        #endregion helpers
    }
}
