// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about the supported versions of a HealthVault
    /// service web method.
    /// </summary>
    ///
    public class HealthServiceMethodVersionInfo
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceMethodVersionInfo"/> class.
        /// </summary>
        /// <param name="method">The method information.</param>
        public HealthServiceMethodVersionInfo(HealthServiceMethodInfo method)
        {
            _method = method;
        }

        internal void ParseXml(XPathNavigator nav)
        {
            string versionString = nav.GetAttribute("number", string.Empty);
            if (!string.IsNullOrEmpty(versionString))
            {
                Version = XmlConvert.ToInt32(versionString);
            }

            XPathNavigator requestNav =
                nav.SelectSingleNode("request-schema-url");
            if (requestNav != null)
            {
                RequestSchemaUrl = new Uri(requestNav.Value);
            }

            ResponseSchemaUrl =
                new Uri(nav.SelectSingleNode("response-schema-url").Value);
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        ///
        /// <value>
        /// A string representing the method name.
        /// </value>
        ///
        public string Name => _method.Name;

        private HealthServiceMethodInfo _method;

        /// <summary>
        /// Gets the method version number.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the method version number.
        /// </value>
        ///
        public int Version { get; set; }

        /// <summary>
        /// Gets the URL of the request schema for the method.
        /// </summary>
        ///
        /// <value>
        /// A Uri representing the request schema.
        /// </value>
        ///
        /// <remarks>
        /// This property returns <b>null</b> if the method takes no parameters.
        /// </remarks>
        ///
        public Uri RequestSchemaUrl { get; set; }

        /// <summary>
        /// Gets the file name of the request schema.
        /// </summary>
        ///
        /// <value>
        /// A string representing the file name.
        /// </value>
        ///
        public string RequestFileName
        {
            get
            {
                if (_requestFileName == null)
                {
                    _requestFileName = GetFileNameFromUrl(RequestSchemaUrl);
                }

                return _requestFileName;
            }
        }

        private string _requestFileName;

        /// <summary>
        /// Gets the URL of the response schema for the method.
        /// </summary>
        ///
        public Uri ResponseSchemaUrl { get; set; }

        /// <summary>
        /// Gets the file name of the response schema.
        /// </summary>
        ///
        /// <value>
        /// A string representing the file name.
        /// </value>
        ///
        public string ResponseFileName
        {
            get
            {
                if (_responseFileName == null)
                {
                    _responseFileName = GetFileNameFromUrl(ResponseSchemaUrl);
                }

                return _responseFileName;
            }
        }

        private string _responseFileName;

        private static string GetFileNameFromUrl(Uri url)
        {
            string result = null;
            if (url != null)
            {
                int filenameStart =
                    url.OriginalString.LastIndexOf('/') + 1;

                result = url.OriginalString.Substring(filenameStart);
            }

            return result;
        }
    }
}
