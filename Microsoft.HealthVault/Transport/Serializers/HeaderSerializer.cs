// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Xml;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport.Serializers;

namespace Microsoft.HealthVault.Transport.MessageFormatters
{
    internal class HeaderSerializer : IRequestMessageSerializer<RequestHeader>
    {
        public string Serialize(RequestHeader header)
        {
            if (header == null)
            {
                throw new ArgumentException(nameof(header));
            }

            string result;

            using (StringWriter headerXml = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(headerXml, SDKHelper.XmlUtf8WriterSettings))
                {
                    using (new TagWriter(writer, "header"))
                    {
                        // <method>
                        writer.WriteElementString("method", header.Method);

                        if (header.MethodVersion > 0)
                        {
                            // <method-version>
                            writer.WriteElementString("method-version", header.MethodVersion.ToString());
                        }

                        if (!string.IsNullOrEmpty(header.TargetPersonId))
                        {
                            // <target-person-id>
                            writer.WriteElementString("target-person-id", header.TargetPersonId);
                        }

                        if (!string.IsNullOrEmpty(header.RecordId))
                        {
                            // <record-id>
                            writer.WriteElementString("record-id", header.RecordId);
                        }

                        if (header.HasAuthSession)
                        {
                            writer.WriteStartElement("auth-session");
                            writer.WriteElementString("auth-token", header.AuthSession.AuthToken);

                            if (header.HasOfflinePersonInfo)
                            {
                                writer.WriteStartElement("offline-person-info");

                                // <offline-person-id>
                                writer.WriteElementString(
                                    "offline-person-id",
                                    header.AuthSession.Person.OfflinePersonId.ToString());

                                // </offline-person-info>
                                writer.WriteEndElement();
                            }
                            else
                            {
                                writer.WriteElementString("user-auth-token", header.AuthSession.UserAuthToken);
                            }
                        }
                        // In case auth session is not present, appid will be sent
                        else
                        {
                            writer.WriteElementString("app-id", header.AppId);
                        }

                        if (string.IsNullOrEmpty(header.CultureCode))
                        {
                            writer.WriteElementString("culture-code", header.CultureCode);
                        }

                        writer.WriteElementString("msg-time", SDKHelper.XmlFromNow());
                        writer.WriteElementString("msg-ttl", header.MessageTtl.ToString());

                        writer.WriteElementString("version", header.Version);

                        InfoHash infoHash = header.InfoHash;

                        if (infoHash != null)
                        {
                            using (new TagWriter(writer, "info-hash"))
                            {
                                using (new TagWriter(writer, "hash-data"))
                                {
                                    writer.WriteAttributeString("algName", infoHash.HashData.Algorithm);
                                    writer.WriteString(infoHash.HashData.Value);
                                }
                            }
                        }
                    }
                }

                result = headerXml.ToString();
            }

            return result;
        }
    }
}