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
    /// A collection of <see cref="PatientActivity" />(s) which describe patient activity information
    /// for Meaningful Use VDT Report from HealthVault. 
    /// </summary>
    internal class PatientActivityCollection : ReportCollection<PatientActivity>
    {
        private HealthServiceConnection _connection;
        private DateRange _reportingPeriodFilter;

        /// <summary>
        /// Constructs an instance of PatientActivityCollection with specified values.
        /// </summary>
        /// 
        /// <param name="connection">
        /// The connection used to obtain the information from HealthVault.
        /// </param>
        /// 
        /// <param name="reportingPeriodFilter">
        /// <see cref="DateRange"/> specifies the reporting period criteria to match when retrieving Timely Access Report.
        /// </param>
        /// 
        internal PatientActivityCollection(
            HealthServiceConnection connection,
            DateRange reportingPeriodFilter)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");
            Validator.ThrowIfArgumentNull(reportingPeriodFilter, "reportingPeriodFilter", "FilterNull");

            _connection = connection;
            _reportingPeriodFilter = reportingPeriodFilter;
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
                "GetMeaningfulUseVDTReport");

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
                    XPathNodeIterator activityNavs = sourceNav.Select("patient-activities/activity");

                    if (sourceNavs != null)
                    {
                        foreach (XPathNavigator activityNav in activityNavs)
                        {
                            PatientActivity item = new PatientActivity(source);
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
            HealthServiceRequest request = _connection.CreateRequest("GetMeaningfulUseVDTReport", 1);

            StringBuilder parameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                writer.WriteStartElement("filters");
                writer.WriteStartElement("reporting-period");

                writer.WriteElementString("min-date", XmlConvert.ToString(_reportingPeriodFilter.Start, XmlDateTimeSerializationMode.Utc));
                writer.WriteElementString("max-date", XmlConvert.ToString(_reportingPeriodFilter.End, XmlDateTimeSerializationMode.Utc));

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
