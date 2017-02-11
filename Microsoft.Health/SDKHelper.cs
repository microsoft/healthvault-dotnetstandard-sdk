// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
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
                XmlReaderSettings settings = new XmlReaderSettings();

                settings.CheckCharacters = true;
                settings.CloseInput = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;
                settings.DtdProcessing = DtdProcessing.Prohibit;
                settings.XmlResolver = null;

                return settings;
            }
        }

        internal static XmlReaderSettings MinimumSafeXmlReaderSettings
        {
            get
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Prohibit;
                settings.XmlResolver = null;

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
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.CheckCharacters = true;
                settings.CloseOutput = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.Indent = false;
                settings.OmitXmlDeclaration = true;

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
        /// <returns></returns>
        /// 
        static internal string XmlFromNow()
        {
            return (XmlFromUtcDateTime(DateTime.UtcNow));
        }

        /// <summary>
        /// XML-formatted date time value
        /// </summary>
        /// 
        /// <param name="dateTime"></param>
        /// 
        /// <returns></returns>
        /// 
        static internal string XmlFromLocalDateTime(DateTime dateTime)
        {
            return (XmlFromUtcDateTime(dateTime.ToUniversalTime()));
        }

        /// <summary>
        /// XML Formatted date time value
        /// </summary>
        /// 
        /// <param name="dateTime"></param>
        /// 
        /// <returns></returns>
        /// 
        /// <exception cref="ArgumentException">
        /// If the date time value provided is not in UTC format.
        /// </exception>
        /// 
        static internal string XmlFromUtcDateTime(DateTime dateTime)
        {
            Validator.ThrowArgumentExceptionIf(
                dateTime.Kind != DateTimeKind.Utc,
                "dateTime",
                "NonUTCDateTime");

            return (XmlFromDateTime(dateTime));
        }

        /// <summary>
        /// XML-formatted date time value
        /// </summary>
        /// 
        /// <param name="dateTime"></param>
        /// 
        /// <returns></returns>
        /// 
        static internal string XmlFromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFZ",
                    CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Read until we find a node with the name
        /// </summary>
        /// 
        /// <param name="xmlReader"></param>
        /// 
        /// <param name="elementName"></param>
        /// 
        static internal bool ReadUntil(XmlReader xmlReader,
                                    string elementName)
        {
            Validator.ThrowIfArgumentNull(xmlReader, "xmlReader", "ArgumentNull");
            Validator.ThrowIfStringNullOrEmpty(elementName, "elementName");

            if (xmlReader.NodeType != XmlNodeType.Element ||
                xmlReader.Name != elementName)
            {
                if (!xmlReader.ReadToFollowing(elementName))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Read until we get to the next element
        /// </summary>
        /// 
        /// <param name="xmlReader"></param>
        /// 
        static internal bool SkipToElement(XmlReader xmlReader)
        {
            Validator.ThrowIfArgumentNull(xmlReader, "xmlReader", "ArgumentNull");

            while (xmlReader.NodeType != XmlNodeType.Element)
            {
                if (!xmlReader.Read())
                    return false;
            }
            return true;
        }

        static internal string XmlFromBool(bool value)
        {
            return XmlConvert.ToString(value);
        }

        static internal string[] SplitAndTrim(
                string all,
                Char separator)
        {
            string[] parts = all.Split(
                    new Char[] { separator },
                    StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; ++i)
            {
                parts[i] = parts[i].Trim();
            }

            return parts;
        }

        private static XPathExpression _infoPath = XPathExpression.Compile("/wc:info");

        internal static XPathExpression GetInfoXPathExpressionForMethod(
            XPathNavigator infoNav,
            string methodName)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);
            infoXmlNamespaceManager.AddNamespace(
                "wc",
                "urn:com.microsoft.wc.methods.response." + methodName);

            XPathExpression infoPathClone = null;
            lock (_infoPath)
            {
                infoPathClone = _infoPath.Clone();
            }

            infoPathClone.SetContext(infoXmlNamespaceManager);

            return infoPathClone;
        }

        public static void SafeLoadXml(this XmlDocument document, string text)
        {
            using (var stringReader = new StringReader(text))
            {
                using (var xmlReader = XmlReader.Create(stringReader, MinimumSafeXmlReaderSettings))
                {
                    document.Load(xmlReader);
                }
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
