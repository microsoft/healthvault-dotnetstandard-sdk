// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Abstract base class for all types that represent data that can
    /// be serialized into a health record item.
    /// </summary>
    /// 
    public abstract class HealthRecordItemData
    {
        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the information.
        /// </param>
        /// 
        public abstract void ParseXml(XPathNavigator navigator);

        /// <summary>
        /// Writes the XML representation of the information into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the contact information.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the contact information should be 
        /// written.
        /// </param>
        /// 
        public abstract void WriteXml(string nodeName, XmlWriter writer);
    }

}
