// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.ItemTypes;
using System;
using System.Collections.ObjectModel;
using System.Xml;

namespace Microsoft.HealthVault
{
    internal static class XmlWriterHelper
    {
        internal static void WriteOptString(
            XmlWriter writer,
            string elementName,
            string value)
        {
            if (!String.IsNullOrEmpty(value))
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
            where DataType : new()
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
            Decimal value)
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
        /// <param name="collection">The collection to write.</param>
        /// <param name="enclosingElementName">The name of an element to enclose the items in the collection.</param>
        /// <param name="itemNodeName">The name of the item node element.</param>
        internal static void WriteXmlCollection<T>(XmlWriter writer, string enclosingElementName, Collection<T> collection, string itemNodeName) where T : HealthRecordItemData
        {
            if (collection == null || collection.Count == 0)
            {
                return;
            }

            writer.WriteStartElement(enclosingElementName);

            WriteXmlCollection<T>(writer, collection, itemNodeName);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a collection.
        /// </summary>
        /// <typeparam name="T">The item type in the collection.</typeparam>
        /// <param name="writer">The writer to use.</param>
        /// <param name="collection">The collection to write.</param>
        /// <param name="itemNodeName">The name of the item node element.</param>
        internal static void WriteXmlCollection<T>(XmlWriter writer, Collection<T> collection, string itemNodeName) where T : HealthRecordItemData
        {
            foreach (HealthRecordItemData item in collection)
            {
                item.WriteXml(itemNodeName, writer);
            }
        }
    }
}
