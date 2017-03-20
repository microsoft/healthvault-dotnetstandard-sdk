// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Transport;
using System.Xml.XPath;
using System.Xml;
using System.Collections.ObjectModel;
using Microsoft.HealthVault.Record;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// A HealthVault person client. Used to access information and records associated with the currently athenticated user.
    /// </summary>
    public class PersonClient : IPersonClient
    {
        private static readonly XPathExpression InfoPersonAppSettingsPath = XPathExpression.Compile("//wc:info");
        private static readonly XPathExpression InfoPersonPath = XPathExpression.Compile("//wc:info/person-info");
        private static readonly XPathExpression InfoRecordPath = XPathExpression.Compile("//wc:info/record");

        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; set; }

        public virtual async Task<ApplicationSettings> GetApplicationSettingsAsync()
        {
            HealthServiceResponseData responseData = await this.Connection
                .ExecuteAsync(HealthVaultMethods.GetApplicationSettings, 1, null)
                .ConfigureAwait(false);

            XPathExpression xPathExpression = this.GetPersonAppSettingsXPathExpression(responseData.InfoNavigator);

            XPathNavigator appSettingsNav = responseData.InfoNavigator.SelectSingleNode(xPathExpression);

            ApplicationSettings settings = null;
            if (appSettingsNav != null)
            {
                settings = new ApplicationSettings();
                settings.ParseXml(appSettingsNav);
            }

            return settings;
        }

        public virtual async Task SetApplicationSettingsAsync(IXPathNavigable applicationSettings)
        {
            string requestParameters = HealthVaultPlatformPerson.GetSetApplicationSettingsParameters(applicationSettings);

            await this.SetApplicationSettingsAsync(requestParameters).ConfigureAwait(false);
        }

        public virtual async Task SetApplicationSettingsAsync(string requestParameters)
        {
            await this.Connection.ExecuteAsync(HealthVaultMethods.SetApplicationSettings, 1, requestParameters).ConfigureAwait(false);
        }

        public virtual async Task<PersonInfo> GetPersonInfoAsync()
        {
            HealthServiceResponseData responseData = await this.Connection.ExecuteAsync(HealthVaultMethods.GetPersonInfo, 1).ConfigureAwait(false);

            XPathExpression personPath = this.GetPersonXPathExpression(responseData.InfoNavigator);

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(personPath);

            return PersonInfo.CreateFromXml(infoNav);
        }

        public virtual async Task<Collection<HealthRecordInfo>> GetAuthorizedRecordsAsync(IList<Guid> recordIds)
        {
            StringBuilder parameters = new StringBuilder(128);
            foreach (Guid id in recordIds)
            {
                parameters.Append(
                    "<id>" + id + "</id>");
            }

            HealthServiceResponseData responseData = await this.Connection.ExecuteAsync(
                    HealthVaultMethods.GetAuthorizedRecords,
                    1,
                    parameters.ToString())
                .ConfigureAwait(false);

            Collection<HealthRecordInfo> results = new Collection<HealthRecordInfo>();

            XPathNodeIterator records = responseData.InfoNavigator.Select(this.GetRecordXPathExpression(responseData.InfoNavigator));

            foreach (XPathNavigator recordNav in records)
            {
                results.Add(HealthRecordInfo.CreateFromXml(this.Connection, recordNav));
            }

            return results;
        }

        private XPathExpression GetPersonAppSettingsXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager = new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace("wc", "urn:com.microsoft.wc.methods.response.GetApplicationSettings");

            XPathExpression infoPersonAppSettingsPathClone;
            lock (InfoPersonAppSettingsPath)
            {
                infoPersonAppSettingsPathClone = InfoPersonAppSettingsPath.Clone();
            }

            infoPersonAppSettingsPathClone.SetContext(infoXmlNamespaceManager);

            return infoPersonAppSettingsPathClone;
        }

        private XPathExpression GetPersonXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager = new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace("wc", "urn:com.microsoft.wc.methods.response.GetPersonInfo");

            XPathExpression infoPersonPathClone;
            lock (InfoPersonPath)
            {
                infoPersonPathClone = InfoPersonPath.Clone();
            }

            infoPersonPathClone.SetContext(infoXmlNamespaceManager);

            return infoPersonPathClone;
        }

        private XPathExpression GetRecordXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager = new XmlNamespaceManager(infoNav.NameTable);
            infoXmlNamespaceManager.AddNamespace("wc", "urn:com.microsoft.wc.methods.response.GetAuthorizedRecords");

            XPathExpression infoRecordPathClone;
            lock (InfoRecordPath)
            {
                infoRecordPathClone = InfoRecordPath.Clone();
            }

            infoRecordPathClone.SetContext(infoXmlNamespaceManager);

            return infoRecordPathClone;
        }
    }
}