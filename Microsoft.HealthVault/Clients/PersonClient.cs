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
        private static readonly XPathExpression InfoPersonAppSettingsPath = XPathExpression.Compile("/wc:info");
        private static readonly XPathExpression InfoPersonPath = XPathExpression.Compile("/wc:info/person-info");
        private static readonly XPathExpression InfoRecordPath = XPathExpression.Compile("/wc:info/record");

        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; set; }

        public virtual async Task<ApplicationSettings> GetApplicationSettingsAsync()
        {
            HealthServiceResponseData responseData = await this.Connection
                .ExecuteAsync("GetApplicationSettings", 1, null)
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
            await this.Connection.ExecuteAsync("SetApplicationSettings", 1, requestParameters).ConfigureAwait(false);
        }

        public virtual async Task<PersonInfo> GetPersonInfoAsync()
        {
            HealthServiceResponseData responseData = await this.Connection.ExecuteAsync("GetPersonInfo", 1).ConfigureAwait(false);

            XPathExpression personPath = this.GetPersonXPathExpression(responseData.InfoNavigator);

            XPathNavigator infoNav = responseData.InfoNavigator.SelectSingleNode(personPath);

            return PersonInfo.CreateFromXml(this.Connection, infoNav);
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
                    "GetAuthorizedRecords",
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

            XPathExpression infoPersonAppSettingsPathClone = null;
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

            XPathExpression infoPersonPathClone = null;
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

            XPathExpression infoRecordPathClone = null;
            lock (InfoRecordPath)
            {
                infoRecordPathClone = InfoRecordPath.Clone();
            }

            infoRecordPathClone.SetContext(infoXmlNamespaceManager);

            return infoRecordPathClone;
        }
    }
}