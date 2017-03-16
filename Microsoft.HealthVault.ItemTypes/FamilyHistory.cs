// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// The condition of a relative.
    /// </summary>
    ///
    /// <remarks>
    /// The family history condition item stores a condition that
    /// a relative of the record owner has. Relating this item to a family
    /// history person item will provide a comprehensive family medical history
    /// record.
    /// </remarks>
    ///
    public class FamilyHistory : ThingBase
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistory"/>
        /// class with default values.
        /// </summary>
        ///
        public FamilyHistory()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistory"/>
        /// class with condition.
        /// </summary>
        ///
        /// <param name="condition">
        /// Relative condition is the condition of a relative.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="condition"/> is <b>null</b>.
        /// </exception>
        ///
        public FamilyHistory(ConditionEntry condition)
            : base(TypeId)
        {
            this.Condition = condition;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("22826e13-41e1-4ba3-8447-37dadd208fd8");

        /// <summary>
        /// Populates this <see cref="FamilyHistory"/> instance from the data in the XML.
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

            this.relativeCondition = new ConditionEntry();
            this.relativeCondition.ParseXml(itemNav.SelectSingleNode("condition"));

            this.relative =
                XPathHelper.GetOptNavValue<FamilyHistoryRelative>(itemNav, "relative");
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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Condition"/> is <b> null </b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.relativeCondition, "FamilyHistoryConditionNotSet");

            // <family-history>
            writer.WriteStartElement("family-history");

            // <condition>
            this.relativeCondition.WriteXml("condition", writer);

            // <relative>
            XmlWriterHelper.WriteOpt(
                writer,
                "relative",
                this.relative);

            // </familty-history>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets a condition.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> of relative condition is <b>null</b> on set.
        /// </exception>
        ///
        public ConditionEntry Condition
        {
            get { return this.relativeCondition; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Condition", "FamilyHistoryConditionMandatory");
                this.relativeCondition = value;
            }
        }

        private ConditionEntry relativeCondition;

        /// <summary>
        /// Gets or sets information about the relative with this condition.
        /// </summary>
        ///
        /// <remarks>
        /// The relative should be set to <b>null</b> if it is unknown.
        /// </remarks>
        ///
        public FamilyHistoryRelative Relative
        {
            get { return this.relative; }
            set { this.relative = value; }
        }

        private FamilyHistoryRelative relative;

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
            string result = this.relativeCondition.Name.ToString();

            if (this.relative != null && this.relative.Relationship != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryToStringFormat"),
                        this.relativeCondition.Name.ToString(),
                        this.relative.Relationship.ToString());
            }

            return result;
        }
    }
}
