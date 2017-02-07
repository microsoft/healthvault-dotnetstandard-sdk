// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// The condition of a relative.
    /// </summary>
    /// 
    /// <remarks>
    /// The family history condition item stores a condition 
    /// that a relative of the record-owner has.
    /// 
    /// To create a family tree, use the relationship-types vocabulary
    /// to code both directions of the parent/child relationship between
    /// one family member and another. These codes are stored as
    /// related items for both parent and child data instances.
    ///
    /// The flexibity of having both condition and relative be optional
    /// allows applications to give the users the ability to just put in
    /// the details they know at the time they want to create the tree.
    /// If they just know the name of the relative, or a particular condition
    /// that's all it takes to get a new instance started.
    /// </remarks>
    ///
    public class FamilyHistoryV3 : HealthRecordItem
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryV3"/> 
        /// class with default values.
        /// </summary>
        /// 
        public FamilyHistoryV3()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("4a04fcc8-19c1-4d59-a8c7-2031a03f21de");

        /// <summary>
        /// Populates this <see cref="FamilyHistoryV3"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the family history data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a family history node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("family-history");

            Validator.ThrowInvalidIfNull(itemNav, "FamilyHistoryUnexpectedNode");

            _conditions.Clear();
            foreach (XPathNavigator conditionNav in itemNav.Select("condition"))
            {
                ConditionEntry condition = new ConditionEntry();
                condition.ParseXml(conditionNav);
                _conditions.Add(condition);
            }

            _relative =
                XPathHelper.GetOptNavValue<FamilyHistoryRelativeV3>(itemNav, "relative");
        }

        /// <summary>
        /// Writes the family history data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the family history data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            
            // <family-history>
            writer.WriteStartElement("family-history");

            // <condition>
            foreach (ConditionEntry condition in _conditions)
            {
                condition.WriteXml("condition", writer);
            }

            // <relative>
            XmlWriterHelper.WriteOpt<FamilyHistoryRelativeV3>(
                writer,
                "relative",
                _relative);

            // </familty-history>
            writer.WriteEndElement();

        }

        /// <summary>
        /// Gets a collection of a conditions.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about the condition of the relative the collection should be empty.
        /// </remarks>
        /// 
        public Collection<ConditionEntry> Conditions
        {
            get { return _conditions; }
        }
        private Collection<ConditionEntry> _conditions = new Collection<ConditionEntry>();

        /// <summary>
        /// Gets or sets information about the relative with this condition.
        /// </summary>
        /// 
        /// <remarks>
        /// The relative should be set to <b>null</b> if it is unknown. 
        /// </remarks>
        /// 
        public FamilyHistoryRelativeV3 Relative
        {
            get { return _relative; }
            set { _relative = value; }
        }
        private FamilyHistoryRelativeV3 _relative;

        /// <summary>
        /// Gets a string representation of the family history item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the family history item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(200);

            foreach (ConditionEntry condition in Conditions)
            {
                if (sb.Length > 0)
                {
                    sb.Append(ResourceRetriever.GetResourceString("ListSeparator"));
                }
                sb.Append(condition.Name.ToString());
            }

            string result = sb.ToString();
            if (_relative != null && _relative.Relationship != null)
            {
                result = String.Format(
                        ResourceRetriever.GetResourceString("FamilyHistoryToStringFormat"),
                        result, 
                        _relative.Relationship.Text);
            }
            return result;
        }
    }
}
