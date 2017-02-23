// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Transport
{
    /// <summary>
    /// Contains the response information from the HealthVault service after
    /// processing a request.
    /// </summary>
    ///
    public class HealthServiceResponseData
    {
        // Prevents creation of an instance.
        internal HealthServiceResponseData()
        {
        }

        /// <summary>
        /// Gets the status code of the response.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceStatusCode"/> value.
        /// </value>
        ///
        /// <remarks>
        /// Any code other than <see cref="HealthServiceStatusCode.Ok"/> indicates
        /// an error. Use the <see cref="HealthServiceResponseData.Error"/> property
        /// to get more information about the error.
        /// </remarks>
        ///
        public HealthServiceStatusCode Code => HealthServiceStatusCodeManager.GetStatusCode(this.CodeId);

        /// <summary>
        /// Gets the integer identifier of the status code in the HealthVault
        /// response.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the status code ID.
        /// </value>
        ///
        /// <remarks>
        /// Use this property when the SDK is out of sync with the HealthVault
        /// status code set. You can look up the actual integer value of the
        /// status code for further investigation.
        /// </remarks>
        ///
        public int CodeId { get; internal set; }

        /// <summary>
        /// Gets the information about an error that occurred while processing
        /// the request.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceResponseError"/>.
        /// </value>
        /// <remarks>
        /// This property is <b>null</b> if
        /// <see cref="HealthServiceResponseData.Code"/> returns
        /// <see cref="HealthServiceStatusCode.Ok"/>.
        /// </remarks>
        ///
        public HealthServiceResponseError Error { get; internal set; }

        /// <summary>
        /// Gets the info section of the response XML.
        /// </summary>
        ///
        public XPathNavigator InfoNavigator
        {
            get
            {
                if (this.infoNavigator == null && this.InfoReader != null)
                {
                    this.infoNavigator = new XPathDocument(this.NewInfoReader).CreateNavigator();
                    this.infoNavigator.MoveToFirstChild();
                }

                return this.infoNavigator;
            }

            internal set { this.infoNavigator = value; }
        }

        private XPathNavigator infoNavigator;

        /// <summary>
        /// Gets the info section of the response XML.
        /// </summary>
        ///
        public XmlReader InfoReader
        {
            get { return this.infoReader ?? (this.infoReader = this.NewInfoReader); }

            internal set { this.infoReader = value; }
        }

        private XmlReader infoReader;

        /// <summary>
        /// Gets the headers on the response.
        /// </summary>
        public HttpResponseHeaders ResponseHeaders { get; internal set; }

        internal XmlReader NewInfoReader
        {
            get
            {
                XmlReader reader = null;
                MemoryStream ms = null;

                try
                {
                    if (this.ResponseText.Array != null && this.ResponseText.Count > 0)
                    {
                        ms =
                            new MemoryStream(
                                this.ResponseText.Array,
                                this.ResponseText.Offset,
                                this.ResponseText.Count,
                                false);

                        reader = XmlReader.Create(ms, SDKHelper.XmlReaderSettings);
                        reader.NameTable.Add("wc");
                        reader.ReadToFollowing("wc:info");
                    }

                    return reader;
                }
                catch
                {
                    ms?.Dispose();

                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the cached response results text (UTF8 encoded).
        /// </summary>
        ///
        internal ArraySegment<byte> ResponseText { get; set; }
    }
}
