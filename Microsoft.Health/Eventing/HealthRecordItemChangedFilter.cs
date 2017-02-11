// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.Events
{
    /// <summary>
    /// A filter that defines what changes to items will result in notification.
    /// </summary>
    public class HealthRecordItemChangedFilter
    {
        /// <summary>
        /// Create an instance of the HealthRecordItemChangedFilter class, specifying the set 
        /// of type ids on which to send event notifications.
        /// </summary>
        /// <param name="typeIds">The type ids on which to send event notifications</param>
        public HealthRecordItemChangedFilter(params Guid[] typeIds)
        {
            if (typeIds != null)
            {
                _typeIds.AddRange(typeIds);
            }
        }

        /// <summary>
        /// Create an instance of the HealthRecordItemChangedFilter class, specifying the set 
        /// of type ids on which to send event notifications.
        /// </summary>
        /// <param name="typeIds">The type ids on which to send event notifications</param>
        public HealthRecordItemChangedFilter(IList<Guid> typeIds)
        {
            _typeIds.AddRange(typeIds);
        }

        internal void WriteXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("filter");
            {
                xmlWriter.WriteStartElement("type-ids");
                {
                    foreach (Guid typeId in _typeIds)
                    {
                        xmlWriter.WriteElementString("type-id", typeId.ToString());
                    }
                }
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
        }

        internal void ParseXml(XPathNavigator filterNav)
        {
            Validator.ThrowIfNavigatorNull(filterNav);

            XPathNavigator typeIdsNav = filterNav.SelectSingleNode("type-ids");
            if (typeIdsNav != null)
            {
                foreach (XPathNavigator typeIdNav in typeIdsNav.SelectChildren(XPathNodeType.Element))
                {
                    Guid guid = new Guid(typeIdNav.Value);
                    _typeIds.Add(guid);
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of type ids for which events will be sent.
        /// </summary>
        public IList<Guid> TypeIds
        {
            get { return _typeIds; }
        }
        List<Guid> _typeIds = new List<Guid>();

        /// <summary>
        ///  Return a string representation of the filter.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            List<string> typeNames = new List<string>();

            foreach (Guid typeId in _typeIds)
            {
                Type type = ItemTypeManager.GetRegisteredTypeForTypeId(typeId);

                if (type != null)
                {
                    typeNames.Add(type.Name);
                }
                else
                {
                    typeNames.Add(typeId.ToString());
                }
            }

            string result = String.Join(
                                ResourceRetriever.GetResourceString("ListSeparator"),
                                typeNames.ToArray());

            return result;
        }
    }
}