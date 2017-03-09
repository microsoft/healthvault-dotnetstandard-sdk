using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Connection
{
    internal abstract class SessionCredentialClientBase : ISessionCredentialClient
    {
        public IConnectionInternal Connection { get; set; }

        protected SessionCredentialClientBase(IConnectionInternal connection)
        {
            this.Connection = connection;
        }

        public async Task<SessionCredential> GetSessionCredentialAsync(CancellationToken token)
        {
            if (this.Connection == null)
            {
                throw new NotSupportedException($"{nameof(this.Connection)} is required");
            }

            HealthServiceRequest request = new HealthServiceRequest(
                this.Connection, ConnectionInternalBase.SessionAuthenticationMethodName, 2)
            {
                Parameters = this.ConstructCreateTokenInfoXml()
            };

            HealthServiceResponseData responseData = await request.ExecuteAsync().ConfigureAwait(false);

            return this.GetSessionCredential(responseData);
        }

        public virtual string ConstructCreateTokenInfoXml()
        {
            StringBuilder infoXml = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(infoXml, settings))
            {
                // Add the PersonInfo elements
                writer.WriteStartElement("auth-info");

                this.ConstructCreateTokenInfoXmlAppIdPart(writer);

                writer.WriteStartElement("credential");
                this.WriteInfoXml(writer);
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.Flush();
            }

            return infoXml.ToString();
        }

        private SessionCredential GetSessionCredential(HealthServiceResponseData responseData)
        {
            if (responseData == null)
            {
                throw new ArgumentNullException($"{nameof(responseData)}");
            }

            SessionCredential sessionCredential = this.GetAuthenticationToken(responseData.InfoNavigator);

            return sessionCredential;
        }

        /// <summary>
        /// Extracts the authentication token from the response XML.
        /// </summary>
        ///
        ///
        /// <param name="nav">
        /// The path to the token.
        /// </param>
        ///
        private SessionCredential GetAuthenticationToken(
            XPathNavigator nav)
        {
            SessionCredential sessionCredential = new SessionCredential();

            XPathExpression authTokenPath = this.GetAuthTokenXPath(nav);
            XPathNodeIterator navTokenIterator = nav.Select(authTokenPath);

            GetTokenByParseResponse(navTokenIterator, sessionCredential);

            XPathNavigator sharedSecret = nav.SelectSingleNode("shared-secret");
            sessionCredential.SharedSecret = sharedSecret.Value;

            return sessionCredential;
        }

        private XPathExpression GetAuthTokenXPath(XPathNavigator infoNav)
        {
            return this.GetTokenXPathExpression(infoNav, "/wc:info/token");
        }

        private XPathExpression GetTokenXPathExpression(
            XPathNavigator infoNav,
            string xPath)
        {
            XPathExpression infoXPathExp = XPathExpression.Compile(xPath);

            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            string nsName = ConnectionInternalBase.SessionAuthenticationMethodName;
            if (nsName == "CreateAuthenticatedSessionToken")
            {
                nsName = "CreateAuthenticatedSessionToken2";
            }

            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + nsName);

            infoXPathExp.SetContext(infoXmlNamespaceManager);

            return infoXPathExp;
        }

        private static void GetTokenByParseResponse(
            XPathNodeIterator navTokenIterator,
            SessionCredential sessionCredential)
        {
            foreach (XPathNavigator tokenNav in navTokenIterator)
            {
                sessionCredential.Token = tokenNav.Value;
            }
        }

        public virtual void ConstructCreateTokenInfoXmlAppIdPart(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            writer.WriteStartElement("app-id");

            var healthApplicationConfiguration = Ioc.Get<IConfiguration>();

            if (healthApplicationConfiguration.IsMultiRecordApp)
            {
                writer.WriteAttributeString("is-multi-record-app", "true");
            }

            writer.WriteValue(this.ApplicationInstanceId.ToString());
            writer.WriteEndElement();
        }

        public abstract Guid ApplicationInstanceId { get; set; }

        public abstract void WriteInfoXml(XmlWriter writer);
    }
}
