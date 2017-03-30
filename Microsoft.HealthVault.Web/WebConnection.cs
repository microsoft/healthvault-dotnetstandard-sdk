using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Person;
using Microsoft.HealthVault.Rest;

namespace Microsoft.HealthVault.Web
{
    /// <summary>
    /// Web connection
    /// </summary>
    internal class WebConnection : HealthVaultConnectionBase
    {
        private readonly WebConfiguration config;

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

        public override string GetRestAuthSessionHeader(Guid? recordId)
        {
            string authToken = this.SessionCredential.Token;
            if (string.IsNullOrEmpty(authToken))
            {
                return string.Empty;
            }

            List<string> tokens = new List<string>();
            tokens.Add(this.FormatRestHeaderToken(RestConstants.AppToken, authToken));
            if (recordId.HasValue && recordId != Guid.Empty)
            {
                tokens.Add(this.FormatRestHeaderToken(RestConstants.RecordId, recordId.Value.ToString()));
            }

            return string.Format(CultureInfo.InvariantCulture, RestConstants.MSHV1HeaderFormat, string.Join(",", tokens));
        }

        private string FormatRestHeaderToken(string name, string value)
        {
            return string.Format(CultureInfo.InvariantCulture, RestConstants.AuthorizationHeaderElement, name, value);
        }

        public override Task<PersonInfo> GetPersonInfoAsync()
        {
            throw new NotImplementedException();
        }
    }
}
