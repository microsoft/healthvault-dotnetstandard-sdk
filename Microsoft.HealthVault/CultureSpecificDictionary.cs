// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Dictionary for storing localized values.
    /// </summary>
    /// <remarks>
    /// The key is the language-country string such as "en-US", and the value is whatever type is
    /// necessary, usually either string or byte[].
    /// </remarks>
    internal abstract class CultureSpecificDictionary<TValue> : Dictionary<string, TValue>
    {
        internal delegate TValue ConvertToType(string xmlValue);

        internal delegate string ConvertFromType(TValue typeValue);

        internal abstract void AppendLocalizedElements(
            XmlWriter writer,
            string elementName);

        /// <summary>
        /// Creates an instance of the <see cref="CultureSpecificDictionary{T}"/> type.
        /// </summary>
        protected CultureSpecificDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        internal void AppendLocalizedElements(
            XmlWriter writer,
            string elementName,
            ConvertFromType convert)
        {
            foreach (KeyValuePair<string, TValue> kvp in this)
            {
                var elementValue = convert(kvp.Value);
                if (!string.IsNullOrEmpty(elementValue))
                {
                    writer.WriteStartElement(elementName);

                    if (!string.Equals(DefaultLanguage, kvp.Key))
                    {
                        writer.WriteAttributeString("xml", "lang", null, kvp.Key);
                    }

                    writer.WriteValue(elementValue);
                    writer.WriteEndElement();
                }
            }
        }

        internal abstract void PopulateFromXml(
            XPathNavigator containerNav,
            string elementName);

        internal void PopulateFromXml(
            XPathNavigator containerNav,
            string elementName,
            ConvertToType convert)
        {
            XPathNodeIterator elementsIterator = containerNav.Select(elementName);
            foreach (XPathNavigator elementNav in elementsIterator)
            {
                var language = elementNav.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");
                if (string.IsNullOrEmpty(language))
                {
                    language = DefaultLanguage;
                }

                var elementValue = elementNav.Value;
                if (!string.IsNullOrEmpty(elementValue))
                {
                    this[language] = convert(elementValue);
                }
                else
                {
                    // Remove any existing value.
                    if (ContainsKey(language))
                    {
                        Remove(language);
                    }
                }
            }
        }

        /// <summary>
        /// Get the best value.
        /// </summary>
        /// <remarks>
        /// The caller will typically only get one translated language back from the platform,
        /// and it may have a non-default language specifier. The best value property allows
        /// getting that value.
        /// </remarks>
        ///
        public TValue BestValue
        {
            get
            {
                if (Count == 1)
                {
                    return Values.First();
                }

                if (ContainsKey(DefaultLanguage))
                {
                    return this[DefaultLanguage];
                }

                return default(TValue);
            }
        }

        /// <summary>
        /// Get or set the default value., with the key = DefaultLangauge.
        /// </summary>
        /// <remarks>
        /// The default value is a value with no corresponding language-country specified.
        /// It's stored in the dictionary with key = DefaultLangauge.
        /// </remarks>
        ///
        public TValue DefaultValue
        {
            get
            {
                if (ContainsKey(DefaultLanguage))
                {
                    return this[DefaultLanguage];
                }

                return default(TValue);
            }

            set
            {
                if (value == null)
                {
                    if (ContainsKey(DefaultLanguage))
                    {
                        Remove(DefaultLanguage);
                    }
                }
                else
                {
                    this[DefaultLanguage] = value;
                }
            }
        }

        /// <summary>
        /// Constant key to indicate the default string. Used in place of the language specifier.
        /// </summary>
        ///
        public const string DefaultLanguage = "DefaultLanguage";
    }

    /// <summary>
    /// Dictionary for storing localized string values.
    /// </summary>
    /// <remarks>
    /// The key is the language-country string such as "en-US", and the value is of type string.
    /// </remarks>
    internal class CultureSpecificStringDictionary : CultureSpecificDictionary<string>
    {
        /// <summary>
        /// Creates a delegate for converting an xml string to a string.
        /// </summary>
        internal static string ConvertToString(string xmlValue)
        {
            return xmlValue;
        }

        /// <summary>
        /// Creates a delegate for converting an string to an xml string.
        /// </summary>
        internal static string ConvertFromString(string stringValue)
        {
            return stringValue;
        }

        internal override void PopulateFromXml(
            XPathNavigator containerNav,
            string elementName)
        {
            ConvertToType stringConvert = ConvertToString;
            PopulateFromXml(
                containerNav,
                elementName,
                stringConvert);
        }

        internal override void AppendLocalizedElements(
            XmlWriter writer,
            string elementName)
        {
            ConvertFromType stringConvert =
                ConvertFromString;

            AppendLocalizedElements(
                writer,
                elementName,
                stringConvert);
        }
    }

    /// <summary>
    /// Dictionary for storing localized string values.
    /// </summary>
    /// <remarks>
    /// The key is the language-country string such as "en-US", and the value is of type string.
    /// </remarks>
    internal class CultureSpecificUrlDictionary : CultureSpecificDictionary<Uri>
    {
        /// <summary>
        /// Creates a delegate for converting an xml string to a string.
        /// </summary>
        internal static Uri ConvertToUri(string xmlValue)
        {
            return new Uri(xmlValue);
        }

        /// <summary>
        /// Creates a delegate for converting an string to an xml string.
        /// </summary>
        internal static string ConvertFromUri(Uri urlValue)
        {
            return urlValue.OriginalString;
        }

        internal override void PopulateFromXml(
            XPathNavigator containerNav,
            string elementName)
        {
            ConvertToType stringConvert =
                ConvertToUri;
            PopulateFromXml(
                containerNav,
                elementName,
                stringConvert);
        }

        internal override void AppendLocalizedElements(
            XmlWriter writer,
            string elementName)
        {
            ConvertFromType stringConvert =
                ConvertFromUri;

            AppendLocalizedElements(
                writer,
                elementName,
                stringConvert);
        }
    }

    /// <summary>
    /// Dictionary for storing localized byte array values.
    /// </summary>
    /// <remarks>
    /// The key is the language-country string such as "en-US", and the value is of type byte[].
    /// </remarks>
    internal class CultureSpecificByteArrayDictionary : CultureSpecificDictionary<byte[]>
    {
        /// <summary>
        /// Creates a delegate for converting an xml string to a byte array.
        /// </summary>
        internal static byte[] ConvertToByteArray(string xmlString)
        {
            return Convert.FromBase64String(xmlString);
        }

        /// <summary>
        /// Creates a delegate for converting a byte array to an xml string.
        /// </summary>
        internal static string ConvertFromByteArray(byte[] typeValue)
        {
            return Convert.ToBase64String(typeValue);
        }

        internal override void PopulateFromXml(
            XPathNavigator containerNav,
            string elementName)
        {
            ConvertToType byteArrayConvert =
                ConvertToByteArray;
            PopulateFromXml(
                containerNav,
                elementName,
                byteArrayConvert);
        }

        internal override void AppendLocalizedElements(
            XmlWriter writer,
            string elementName)
        {
            ConvertFromType byteArrayConvert =
                ConvertFromByteArray;

            AppendLocalizedElements(
                writer,
                elementName,
                byteArrayConvert);
        }
    }
}
