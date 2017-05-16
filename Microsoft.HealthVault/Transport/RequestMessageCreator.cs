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
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Transport.Serializers;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// <see cref="IRequestMessageCreator"/>
    /// </summary>
    internal class RequestMessageCreator : IRequestMessageCreator
    {
        private readonly IConnectionInternal _connectionInternal;

        private readonly HealthVaultConfiguration _healthVaultConfiguration;
        private readonly SdkTelemetryInformation _telemetryInformation;
        private readonly ICryptographer _cryptographer;

        public RequestMessageCreator(
            IConnectionInternal connectionInternal,
            IServiceLocator serviceLocator)
        {
            _connectionInternal = connectionInternal;

            _healthVaultConfiguration = serviceLocator.GetInstance<HealthVaultConfiguration>();
            _telemetryInformation = serviceLocator.GetInstance<SdkTelemetryInformation>();
            _cryptographer = serviceLocator.GetInstance<ICryptographer>();
        }

        /// <summary>
        /// <see cref="IRequestMessageCreator.Create"/>
        /// </summary>
        public string Create(
            HealthVaultMethods method,
            int methodVersion,
            bool isMethodAnonymous,
            string parameters = null,
            Guid? recordId = null,
            Guid? appId = null)
        {
            Request request = new Request { Info = new RequestInfo { InfoXml = parameters } };

            // Serialize info part of the request message
            string infoXml = Serialize(new RequestInfoSerializer(), request.Info.InfoXml);

            SetRequestHeader(
                method,
                methodVersion,
                isMethodAnonymous,
                recordId,
                appId,
                infoXml,
                request);

            //Serialize header part of the request message
            string headerXml = Serialize(new RequestHeaderSerializer(), request.Header);

            string authXml = null;

            // in case the method is anonymous, there is no need to set auth
            if (!isMethodAnonymous)
            {
                SetAuth(headerXml, request);
                authXml = Serialize(new RequestAuthSerializer(), request.Auth);
            }

            string requestXml = SerializeRequest(authXml, headerXml, infoXml);
            return requestXml;
        }

        public void SetRequestHeader(
            HealthVaultMethods method,
            int methodVersion,
            bool isAnonymous,
            Guid? recordId,
            Guid? appId,
            string infoXml,
            Request request)
        {
            request.Header = new RequestHeader
            {
                Method = method.ToString(),
                MethodVersion = methodVersion
            };

            if (recordId != null)
            {
                request.Header.RecordId = recordId.Value.ToString();
            }

            // in case the method is anonymous - set app id, else set auth session
            if (isAnonymous)
            {
                request.Header.AppId = appId.HasValue
                    ? appId.Value.ToString()
                    : _healthVaultConfiguration.MasterApplicationId.ToString();
            }
            else
            {
                request.Header.AuthSession = _connectionInternal.GetAuthSessionHeader();
            }

            request.Header.MessageTime = SDKHelper.XmlFromNow();
            request.Header.MessageTtl = (int)_healthVaultConfiguration.RequestTimeToLiveDuration.TotalSeconds;

            request.Header.Version =
                $"{_telemetryInformation.Category}/{_telemetryInformation.FileVersion} {_telemetryInformation.OsInformation}";

            request.Header.InfoHash = new InfoHash
            {
                HashData = _cryptographer.Hash(Encoding.UTF8.GetBytes(infoXml))
            };
        }

        public void SetAuth(string headerXml, Request request)
        {
            request.Auth = new RequestAuth
            {
                HmacData = _cryptographer.Hmac(
                    _connectionInternal.SessionCredential.SharedSecret,
                    Encoding.UTF8.GetBytes(headerXml))
            };
        }

        private string Serialize<T>(IRequestMessageSerializer<T> requestMessageSerializer, T objectToSerialize)
        {
            return requestMessageSerializer.Serialize(objectToSerialize);
        }

        private string SerializeRequest(string authXml, string headerXml, string infoXml)
        {
            using (StringWriter requestXml = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(requestXml, SDKHelper.XmlUtf8WriterSettings))
                {
                    using (new TagWriter(writer, "request", "wc-request", "urn:com.microsoft.wc.request"))
                    {
                        if (authXml != null)
                        {
                            writer.WriteRaw(authXml);
                        }

                        writer.WriteRaw(headerXml);
                        writer.WriteRaw(infoXml);
                    }
                }

                return requestXml.ToString();
            }
        }
    }
}
