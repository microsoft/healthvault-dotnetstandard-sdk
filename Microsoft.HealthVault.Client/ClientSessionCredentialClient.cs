using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Client
{
    /// <summary>
    /// Gets session credentials on the Client SDK.
    /// </summary>
    internal class ClientSessionCredentialClient : SessionCredentialClientBase, IClientSessionCredentialClient
    {
        private readonly ICryptographer cryptographer;

        public ClientSessionCredentialClient(ICryptographer cryptographer)
        {
            this.cryptographer = cryptographer;
        }

        public string AppSharedSecret { get; set; }

        public override void WriteInfoXml(XmlWriter writer)
        {
            byte[] hmacContentBytes = this.GetHmacContentBytes();
            CryptoData hmacResult = this.cryptographer.Hmac(this.AppSharedSecret, hmacContentBytes);

            writer.WriteStartElement("appserver2");
            writer.WriteStartElement("hmacSig");
            writer.WriteAttributeString("algName", hmacResult.Algorithm);
            writer.WriteValue(hmacResult.Value); // HMAC of content
            writer.WriteEndElement(); // hmacSig
            writer.WriteRaw(Encoding.UTF8.GetString(hmacContentBytes));
            writer.WriteEndElement(); // appserver2
        }

        private byte[] GetHmacContentBytes()
        {
            XmlWriterSettings settings = SDKHelper.XmlUtf8WriterSettings;

            using (MemoryStream contentMemoryStream = new MemoryStream())
            using (XmlWriter writer = XmlWriter.Create(contentMemoryStream, settings))
            {
                writer.WriteStartElement("content");
                writer.WriteElementString("app-id", this.Connection.ApplicationId.ToString());
                writer.WriteElementString("hmac", HealthVaultConstants.Cryptography.HmacAlgorithm);
                writer.WriteElementString("signing-time", DateTimeOffset.UtcNow.ToString("u"));
                writer.WriteEndElement(); // content

                return contentMemoryStream.ToArray();
            }
        }
    }
}
