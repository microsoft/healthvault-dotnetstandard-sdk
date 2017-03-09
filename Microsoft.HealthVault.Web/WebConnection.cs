using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Web connection
    /// </summary>
    internal class WebConnection : ConnectionInternalBase
    {
        public string SubCredential { get; set; }

        public WebConnection(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override Task AuthenticateAsync()
        {
            // Do nothing for now.
            return Task.FromResult<object>(null);
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            if (!String.IsNullOrEmpty(SessionCredential?.Token))
            {
                writer.WriteStartElement("auth-session");

                writer.WriteElementString("auth-token", SessionCredential.Token);

                if (!string.IsNullOrEmpty(this.SubCredential))
                {
                    writer.WriteElementString(
                        "user-auth-token",
                        this.SubCredential);
                }

                writer.WriteEndElement();
            }
        }

        public override void StoreSessionCredentialInCookieXml(XmlWriter writer)
        {
            writer.WriteStartElement("appserver");
            writer.WriteElementString("auth-token", this.SessionCredential.Token);
            writer.WriteElementString("shared-secret", this.SessionCredential.SharedSecret);
            writer.WriteElementString("user-auth-token", this.SubCredential);
            writer.WriteEndElement();
        }

        public override void SetSessionCredentialFromCookieXml(XPathNavigator navigator)
        {
            XPathNavigator credNavigator = navigator.SelectSingleNode("appserver");

            var authToken = credNavigator?.SelectSingleNode("auth-token")?.Value;
            var sharedSecret = credNavigator?.SelectSingleNode("shared-secret")?.Value;

            this.SubCredential = credNavigator?.SelectSingleNode("user-auth-token")?.Value;
            this.SessionCredentialInternal = new SessionCredential() {SharedSecret = sharedSecret, Token = authToken};
        }
    }
}
