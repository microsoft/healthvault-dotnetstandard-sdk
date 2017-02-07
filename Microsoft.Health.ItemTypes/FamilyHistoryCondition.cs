// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public class FamilyHistoryCondition : HealthRecordItem
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryCondition"/> 
        /// class with default values.
        /// </summary>
        /// 
        public FamilyHistoryCondition()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="FamilyHistoryCondition"/> 
        /// class with condition.
        /// </summary>
        /// 
        /// <param name="relativeCondition">
        /// Relative condition is the condition of a relative. 
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="relativeCondition"/> is <b>null</b>.
        /// </exception>
        /// 
        public FamilyHistoryCondition(ConditionEntry relativeCondition)
            : base(TypeId)
        {
            this.RelativeCondition = relativeCondition;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("6705549b-0e3d-474e-bfa7-8197ddd6786a");

        /// <summary>
        /// Populates this <see cref="FamilyHistoryCondition"/> instance from the data in the XML. 
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the family history condition data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a family history condition node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("family-history-condition");

            Validator.ThrowInvalidIfNull(itemNav, "FamilyHistoryConditionUnexpectedNode");

            _relativeCondition = new ConditionEntry();
            _relativeCondition.ParseXml(itemNav.SelectSingleNode("condition"));
        }

        /// <summary>
        /// Writes the family history condition data to the specified XmlWriter.
        /// </summary> 
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the concern data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="RelativeCondition"/> is <b> null </b>.
        /// </exception> 
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_relativeCondition, "FamilyHistoryConditionConditionNotSet");

            // <family-history-condition>
            writer.WriteStartElement("family-history-condition");

            // <condition>
            _relativeCondition.WriteXml("condition", writer);

            // </familty-history-conditon>
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
        public ConditionEntry RelativeCondition
        {
            get { return _relativeCondition; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "RelativeCondition", "FamilyHistoryConditionConditionMandatory");
                _relativeCondition = value;
            }
        }
        private ConditionEntry _relativeCondition;

        /// <summary>
        /// Gets a string representation of the family history condition item.
        /// </summary> 
        ///
        /// <returns>
        /// A string representation of the family history condition item.
        /// </returns>
        ///
        public override string ToString()
        {
            return _relativeCondition.Name.ToString();
        }
    }
}

