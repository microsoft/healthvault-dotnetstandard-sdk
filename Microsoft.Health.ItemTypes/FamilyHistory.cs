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
    /// The family history condition item stores a condition that 
    /// a relative of the record owner has. Relating this item to a family
    /// history person item will provide a comprehensive family medical history 
    /// record. 
    /// </remarks>
    ///
    public class FamilyHistory : HealthRecordItem
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
        public new static readonly Guid TypeId =
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

            _relativeCondition = new ConditionEntry();
            _relativeCondition.ParseXml(itemNav.SelectSingleNode("condition"));

            _relative =
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
            Validator.ThrowSerializationIfNull(_relativeCondition, "FamilyHistoryConditionNotSet");

            // <family-history>
            writer.WriteStartElement("family-history");

            // <condition>
            _relativeCondition.WriteXml("condition", writer);

            // <relative>
            XmlWriterHelper.WriteOpt<FamilyHistoryRelative>(
                writer,
                "relative",
                _relative);

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
            get { return _relativeCondition; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Condition", "FamilyHistoryConditionMandatory");
                _relativeCondition = value;
            }
        }
        private ConditionEntry _relativeCondition;

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
            get { return _relative; }
            set { _relative = value; } 
        }
        private FamilyHistoryRelative _relative;

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
            string result = _relativeCondition.Name.ToString();

            if (_relative != null && _relative.Relationship != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "FamilyHistoryToStringFormat"),
                        _relativeCondition.Name.ToString(),
                        _relative.Relationship.ToString());
            }
            return result;
        }
    }
}

