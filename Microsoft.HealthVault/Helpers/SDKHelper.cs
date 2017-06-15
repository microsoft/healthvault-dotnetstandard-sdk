// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Helpers
{
    /// <summary>
    /// HealthVault XML Helper Class
    /// </summary>
    ///
    internal static class SDKHelper
    {
        internal static XmlReaderSettings XmlReaderSettings
        {
            get
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    CheckCharacters = true,
                    CloseInput = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true,
                    DtdProcessing = DtdProcessing.Prohibit
                };

                return settings;
            }
        }

        internal static XmlReaderSettings MinimumSafeXmlReaderSettings
        {
            get
            {
                XmlReaderSettings settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit };

                return settings;
            }
        }

        internal static XmlWriterSettings XmlUnicodeWriterSettings
        {
            get
            {
                XmlWriterSettings settings = XmlWriterSettings;

                settings.Encoding = new UnicodeEncoding(false, false);

                return settings;
            }
        }

        internal static XmlWriterSettings XmlUtf8WriterSettings
        {
            get
            {
                XmlWriterSettings settings = XmlWriterSettings;

                settings.Encoding = new UTF8Encoding(false, true);

                return settings;
            }
        }

        private static XmlWriterSettings XmlWriterSettings
        {
            get
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CheckCharacters = true,
                    CloseOutput = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Indent = false,
                    OmitXmlDeclaration = true
                };

                return settings;
            }
        }

        /// <summary>
        /// invalid date marker
        /// </summary>
        ///
        internal static readonly DateTime DateUnspecified = DateTime.MinValue;

        /// <summary>
        /// XML-formatted date time value
        /// </summary>
        ///
        /// <returns>Xml formatted time from now</returns>
        ///
        internal static string XmlFromNow()
        {
            return XmlFromUtcDateTime(DateTime.UtcNow);
        }

        /// <summary>
        /// XML-formatted date time value
        /// </summary>
        ///
        /// <param name="dateTime">The date to format to XML</param>
        ///
        /// <returns>Xml formatted time from the date</returns>
        ///
        internal static string XmlFromLocalDateTime(DateTime dateTime)
        {
            return XmlFromUtcDateTime(dateTime.ToUniversalTime());
        }

        /// <summary>
        /// XML Formatted date time value
        /// </summary>
        ///
        /// <param name="dateTime">The date to format to XML</param>
        ///
        /// <returns>The date formatted to XML</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If the date time value provided is not in UTC format.
        /// </exception>
        ///
        internal static string XmlFromUtcDateTime(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(Resources.NonUTCDateTime, nameof(dateTime));
            }

            return XmlFromDateTime(dateTime);
        }

        /// <summary>
        /// XML-formatted date time value
        /// </summary>
        ///
        /// <param name="dateTime">The date to format to XML</param>
        ///
        /// <returns>The date formatted to XML</returns>
        ///
        internal static string XmlFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString(
                "yyyy-MM-ddTHH:mm:ss.FFFZ",
                    CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Read until we find a node with the name
        /// </summary>
        ///
        /// <param name="xmlReader">The XML to read</param>
        ///
        /// <param name="elementName">The element to look for</param>
        ///
        internal static bool ReadUntil(
            XmlReader xmlReader,
            string elementName)
        {
            Validator.ThrowIfArgumentNull(xmlReader, nameof(xmlReader), Resources.ArgumentNull);
            Validator.ThrowIfStringNullOrEmpty(elementName, "elementName");

            if (xmlReader.NodeType != XmlNodeType.Element ||
                xmlReader.Name != elementName)
            {
                if (!xmlReader.ReadToFollowing(elementName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Read until we get to the next element
        /// </summary>
        ///
        /// <param name="xmlReader">The xml element to skip within</param>
        ///
        internal static bool SkipToElement(XmlReader xmlReader)
        {
            Validator.ThrowIfArgumentNull(xmlReader, nameof(xmlReader), Resources.ArgumentNull);

            while (xmlReader.NodeType != XmlNodeType.Element)
            {
                if (!xmlReader.Read())
                {
                    return false;
                }
            }

            return true;
        }

        internal static string XmlFromBool(bool value)
        {
            return XmlConvert.ToString(value);
        }

        internal static string[] SplitAndTrim(
                string all,
                char separator)
        {
            string[] parts = all.Split(
                    new[] { separator },
                    StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Trim();
            }

            return parts;
        }

        private static XPathExpression s_infoPath = XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetInfoXPathExpressionForMethod(
            XPathNavigator infoNav,
            string methodName)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);
            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + methodName);

            XPathExpression infoPathClone;
            lock (s_infoPath)
            {
                infoPathClone = s_infoPath.Clone();
            }

            infoPathClone.SetContext(infoXmlNamespaceManager);

            return infoPathClone;
        }

        public static XDocument SafeLoadXml(string text)
        {
            using (var xmlReader = XmlReader.Create(new StringReader(text), MinimumSafeXmlReaderSettings))
            {
                return XDocument.Load(xmlReader);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "New instance of StringReader is returned to the caller")]
        internal static XmlReader GetXmlReaderForXml(string xml, XmlReaderSettings settings = null)
        {
            return GetXmlReader(new StringReader(xml), settings);
        }

        internal static XmlReader GetXmlReader(TextReader input, XmlReaderSettings settings = null)
        {
            settings = settings ?? MinimumSafeXmlReaderSettings;
            return XmlReader.Create(input, settings);
        }
    }
}
