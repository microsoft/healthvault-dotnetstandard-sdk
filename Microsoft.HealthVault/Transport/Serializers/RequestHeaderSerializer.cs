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

namespace Microsoft.HealthVault.Transport.Serializers
{
    /// <summary>
    /// Serializes Header section of the request
    /// </summary>
    internal class RequestHeaderSerializer : IRequestMessageSerializer<RequestHeader>
    {
        public string Serialize(RequestHeader requestHeader)
        {
            if (requestHeader == null)
            {
                throw new ArgumentException(nameof(requestHeader));
            }

            string result;

            using (StringWriter headerXml = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(headerXml, SDKHelper.XmlUtf8WriterSettings))
                {
                    using (new TagWriter(writer, "header"))
                    {
                        // <method>
                        writer.WriteElementString("method", requestHeader.Method);

                        if (requestHeader.MethodVersion > 0)
                        {
                            // <method-version>
                            writer.WriteElementString("method-version", requestHeader.MethodVersion.ToString());
                        }

                        if (!string.IsNullOrEmpty(requestHeader.TargetPersonId))
                        {
                            // <target-person-id>
                            writer.WriteElementString("target-person-id", requestHeader.TargetPersonId);
                        }

                        if (!string.IsNullOrEmpty(requestHeader.RecordId))
                        {
                            // <record-id>
                            writer.WriteElementString("record-id", requestHeader.RecordId);
                        }

                        if (requestHeader.HasAuthSession)
                        {
                            using (new TagWriter(writer, "auth-session"))
                            {
                                writer.WriteElementString("auth-token", requestHeader.AuthSession.AuthToken);

                                if (requestHeader.HasOfflinePersonInfo)
                                {
                                    writer.WriteStartElement("offline-person-info");

                                    // <offline-person-id>
                                    writer.WriteElementString(
                                        "offline-person-id",
                                        requestHeader.AuthSession.Person.OfflinePersonId.ToString());

                                    // </offline-person-info>
                                    writer.WriteEndElement();
                                }
                                else if(requestHeader.HasUserAuthToken)
                                {
                                    writer.WriteElementString("user-auth-token",
                                        requestHeader.AuthSession.UserAuthToken);
                                }
                            }
                        }
                        // In case auth session is not present, appid will be sent
                        else
                        {
                            writer.WriteElementString("app-id", requestHeader.AppId);
                        }

                        if (!string.IsNullOrEmpty(requestHeader.CultureCode))
                        {
                            writer.WriteElementString("culture-code", requestHeader.CultureCode);
                        }

                        writer.WriteElementString("msg-time", SDKHelper.XmlFromNow());
                        writer.WriteElementString("msg-ttl", requestHeader.MessageTtl.ToString());

                        writer.WriteElementString("version", requestHeader.Version);

                        InfoHash infoHash = requestHeader.InfoHash;

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