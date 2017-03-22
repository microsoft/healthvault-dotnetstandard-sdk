using System;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.PlatformInformation;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Web connection
    /// </summary>
    internal class WebConnection : HealthVaultConnectionBase
    {
        private WebConfiguration config;

        public WebConnection(
            IServiceLocator serviceLocator,
            WebConfiguration configuration)
            : base(serviceLocator)
        {
            this.config = configuration;
        }

        public override Guid ApplicationId => this.config.MasterApplicationId;

        public string SubCredential { get; set; }

        public override Task AuthenticateAsync()
        {
            // Do nothing for now.
            return Task.FromResult<object>(null);
        }

        protected override ISessionCredentialClient CreateSessionCredentialClient()
        {
            var sessionCredentialClient = this.ServiceLocator.GetInstance<ISessionCredentialClient>();
            sessionCredentialClient.Connection = this;

            return sessionCredentialClient;
        }

        public override void PrepareAuthSessionHeader(XmlWriter writer, Guid? recordId)
        {
            if (!string.IsNullOrEmpty(this.SessionCredential?.Token))
            {
                writer.WriteStartElement("auth-session");

                writer.WriteElementString("auth-token", this.SessionCredential.Token);

                if (!string.IsNullOrEmpty(this.SubCredential))
                {
                    writer.WriteElementString(
                        "user-auth-token",
                        this.SubCredential);
                }

                writer.WriteEndElement();
            }
        }
    }
}
