// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault
{
    internal static class XmlWriterHelper
    {
        static internal void WriteOptString(
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

        static internal void WriteOptBool(
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

        static internal void WriteOptInt(
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

        static internal void WriteOptUInt(
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

        static internal void WriteOptDouble(
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

        static internal void WriteOpt<DataType>(
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

        static internal void WriteOptUrl(
            XmlWriter writer,
            string elementName,
            Uri value)
        {
            if (value != null)
            {
                writer.WriteElementString(elementName, value.OriginalString);
            }
        }

        static internal void WriteOptGuid(
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

        static internal void WriteDecimal(
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
        static internal void WriteXmlCollection<T>(XmlWriter writer, string enclosingElementName, Collection<T> collection, string itemNodeName) where T : HealthRecordItemData
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
        static internal void WriteXmlCollection<T>(XmlWriter writer, Collection<T> collection, string itemNodeName) where T : HealthRecordItemData
        {
            foreach (HealthRecordItemData item in collection)
            {
                item.WriteXml(itemNodeName, writer);
            }
        }
    }
}
