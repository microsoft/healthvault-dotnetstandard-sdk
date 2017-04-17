// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// <see cref="IHealthServiceResponseParser"/>
    /// </summary>
    internal class HealthServiceResponseParser : IHealthServiceResponseParser
    {
        /// <summary>
        /// <see cref="IHealthServiceResponseParser.ParseResponseAsync"/>
        /// </summary>
        public async Task<HealthServiceResponseData> ParseResponseAsync(HttpResponseMessage response)
        {
            using (Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var reader = new StreamReader(responseStream, Encoding.UTF8, false, 1024))
                {
                    HealthServiceResponseData result =
                        new HealthServiceResponseData { ResponseHeaders = response.Headers };

                    XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
                    settings.CloseInput = false;
                    settings.IgnoreWhitespace = false;

                    XmlReader xmlReader = XmlReader.Create(reader, settings);
                    xmlReader.NameTable.Add("wc");

                    if (!SDKHelper.ReadUntil(xmlReader, "code"))
                    {
                        throw new MissingFieldException("code");
                    }

                    result.CodeId = xmlReader.ReadElementContentAsInt();

                    if (result.Code == HealthServiceStatusCode.Ok)
                    {
                        if (xmlReader.ReadToFollowing("wc:info"))
                        {
                            result.InfoNavigator = new XPathDocument(xmlReader).CreateNavigator();
                            result.InfoNavigator.MoveToFirstChild();
                        }

                        return result;
                    }

                    result.Error = this.HandleErrorResponse(xmlReader);

                    HealthServiceException healthServiceException =
                        HealthServiceExceptionHelper.GetHealthServiceException(result);

                    throw healthServiceException;
                }
            }
        }

        private HealthServiceResponseError HandleErrorResponse(XmlReader reader)
        {
            HealthServiceResponseError error = new HealthServiceResponseError();

            // <error>
            if (string.Equals(reader.Name, "error", StringComparison.Ordinal))
            {
                // <message>
                if (!SDKHelper.ReadUntil(reader, "message"))
                {
                    throw new MissingFieldException("message");
                }

                error.Message = reader.ReadElementContentAsString();

                // <context>
                SDKHelper.SkipToElement(reader);
                if (string.Equals(reader.Name, "context", StringComparison.Ordinal))
                {
                    HealthServiceErrorContext errorContext = new HealthServiceErrorContext();

                    // <server-name>
                    if (SDKHelper.ReadUntil(reader, "server-name"))
                    {
                        errorContext.ServerName = reader.ReadElementContentAsString();
                    }
                    else
                    {
                        throw new MissingFieldException("server-name");
                    }

                    // <server-ip>
                    Collection<IPAddress> ipAddresses = new Collection<IPAddress>();

                    SDKHelper.SkipToElement(reader);
                    while (reader.Name.Equals("server-ip", StringComparison.Ordinal))
                    {
                        string ipAddressString = reader.ReadElementContentAsString();
                        IPAddress ipAddress;
                        if (IPAddress.TryParse(ipAddressString, out ipAddress))
                        {
                            ipAddresses.Add(ipAddress);
                        }

                        SDKHelper.SkipToElement(reader);
                    }

                    errorContext.SetServerIpAddresses(ipAddresses);

                    // <exception>
                    if (reader.Name.Equals("exception", StringComparison.Ordinal))
                    {
                        errorContext.InnerException = reader.ReadElementContentAsString();
                        SDKHelper.SkipToElement(reader);
                    }
                    else
                    {
                        throw new MissingFieldException("exception");
                    }

                    error.Context = errorContext;
                }

                // <error-info>
                if (SDKHelper.ReadUntil(reader, "error-info"))
                {
                    error.ErrorInfo = reader.ReadElementContentAsString();
                    SDKHelper.SkipToElement(reader);
                }
            }

            return error;
        }
    }
}
