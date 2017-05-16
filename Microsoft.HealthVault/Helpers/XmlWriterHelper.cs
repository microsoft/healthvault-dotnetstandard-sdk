// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Helpers
{
    internal static class XmlWriterHelper
    {
        internal static void WriteOptString(
            XmlWriter writer,
            string elementName,
            string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteElementString(
                    elementName,
                    value);
            }
        }

        internal static void WriteOptBool(
            XmlWriter writer,
            string elementName,
            bool? value)
        {
            if (value != null)
            {
                writer.WriteElementString(
                    elementName,
                    SDKHelper.XmlFromBool((bool)value));
            }
        }

        internal static void WriteOptInt(
            XmlWriter writer,
            string elementName,
            int? value)
        {
            if (value != null)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString((int)value));
            }
        }

        internal static void WriteOptUInt(
            XmlWriter writer,
            string elementName,
            uint? value)
        {
            if (value != null)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString((uint)value));
            }
        }

        internal static void WriteOptDouble(
            XmlWriter writer,
            string elementName,
            double? value)
        {
            if (value != null)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString((double)value));
            }
        }

        internal static void WriteOpt<DataType>(
            XmlWriter writer,
            string elementName,
            DataType value)
            where DataType : ItemBase, new()
        {
            if (value != null)
            {
                value.WriteXml(elementName, writer);
            }
        }

        internal static void WriteOptUrl(
            XmlWriter writer,
            string elementName,
            Uri value)
        {
            if (value != null)
            {
                writer.WriteElementString(elementName, value.OriginalString);
            }
        }

        internal static void WriteOptGuid(
            XmlWriter writer,
            string elementName,
            Guid? value)
        {
            if (value != null)
            {
                writer.WriteElementString(
                    elementName,
                    XmlConvert.ToString((Guid)value));
            }
        }

        internal static void WriteDecimal(
            XmlWriter writer,
            string elementName,
            decimal value)
        {
            writer.WriteElementString(
                elementName, XmlConvert.ToString(value));
        }

        /// <summary>
        /// Write out a collection.
        /// </summary>
        /// <remarks>
        /// If the collection is empty, not items are written.
        /// </remarks>
        ///
        /// <typeparam name="T">The item type in the collection.</typeparam>
        /// <param name="writer">The writer to use.</param>
        /// <param name="enclosingElementName">The name of an element to enclose the items in the collection.</param>
        /// <param name="collection">The collection to write.</param>
        /// <param name="itemNodeName">The name of the item node element.</param>
        internal static void WriteXmlCollection<T>(XmlWriter writer, string enclosingElementName, Collection<T> collection, string itemNodeName)
            where T : ItemBase
        {
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            writer.WriteStartElement(enclosingElementName);

            WriteXmlCollection(writer, collection, itemNodeName);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a collection.
        /// </summary>
        /// <typeparam name="T">The item type in the collection.</typeparam>
        /// <param name="writer">The writer to use.</param>
        /// <param name="collection">The collection to write.</param>
        /// <param name="itemNodeName">The name of the item node element.</param>
        internal static void WriteXmlCollection<T>(XmlWriter writer, Collection<T> collection, string itemNodeName)
            where T : ItemBase
        {
            foreach (T item in collection)
            {
                item.WriteXml(itemNodeName, writer);
            }
        }
    }
}
