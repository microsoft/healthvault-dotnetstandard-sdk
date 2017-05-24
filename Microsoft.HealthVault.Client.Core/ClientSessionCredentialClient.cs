// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text;
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
        public string AppSharedSecret { get; set; }

        public override void WriteInfoXml(XmlWriter writer)
        {
            byte[] hmacContentBytes = GetHmacContentBytes();
            CryptoData hmacResult = Cryptographer.Hmac(AppSharedSecret, hmacContentBytes);

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
            {
                using (XmlWriter writer = XmlWriter.Create(contentMemoryStream, settings))
                {
                    writer.WriteStartElement("content");
                    writer.WriteElementString("app-id", Connection.ApplicationId.ToString());
                    writer.WriteElementString("hmac", HealthVaultConstants.Cryptography.HmacAlgorithm);
                    writer.WriteElementString("signing-time", DateTimeOffset.UtcNow.ToString("o"));
                    writer.WriteEndElement(); // content
                }

                return contentMemoryStream.ToArray();
            }
        }
    }
}
