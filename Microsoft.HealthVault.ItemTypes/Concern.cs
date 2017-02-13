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


namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A concern that a person has about a condition or life issue. 
    /// </summary>
    /// 
    /// <remarks>
    /// A concern that a person has about a condition or life issue. 
    /// Concerns are more general than conditions, and are about the 
    /// person's feelings. Examples include "concerned about managing 
    /// a chronic condition", "family issues", "emotional issues", etc. 
    /// </remarks>
    /// 
    public class Concern : HealthRecordItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Concern"/> class, 
        /// with default values.
        /// </summary>
        /// 
        public Concern()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Concern"/> class,
        /// with a specified description. 
        /// </summary>
        /// 
        /// <param name="description">
        /// The description of a concern.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException"> 
        /// If <paramref name="description"/> is <b>null</b>. 
        /// </exception>
        /// 
        public Concern(CodableValue description)
            : base(TypeId)
        {
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Concern"/> class,
        /// with the specified description. 
        /// </summary>
        /// 
        /// <param name="description">
        /// The description of a concern.
        /// </param>
        /// 
        /// <exception cref="ArgumentException"> 
        /// If <paramref name="description"/> is <b> null</b> or empty. 
        /// </exception>
        /// 
        public Concern(string description)
            : base(TypeId)
        {
            _description = new CodableValue(description);
        } 

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
            new Guid("AEA2E8F2-11DD-4A7D-AB43-1D58764EBC19");

        /// <summary>
        /// Populates this <see cref="Concern"/> instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the concern data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a concern node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("concern");

            Validator.ThrowInvalidIfNull(itemNav, "ConcernUnexpectedNode");

            // description
            _description = new CodableValue();
            _description.ParseXml(itemNav.SelectSingleNode("description"));

            // status
            _status =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "status");
        }

        /// <summary>
        /// Writes the concern data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the concern data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Description"/> is <b>null</b> or empty.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_description, "ConcernDescriptionNotSet");
            Validator.ThrowSerializationIfNull(_description.Text, "CodableValueNullText");

            // <concern>
            writer.WriteStartElement("concern");

            // description
            _description.WriteXml("description",writer);

            // status
            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "status",
                _status);

            // </concern>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description of a concern.
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="value"/> is <b>null</b> on set.
        /// </exception>
        /// 
        public CodableValue Description
        {
            get { return _description; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Description", "ConcernDescriptionMandatory");
                _description = value;
            }
        }
        private CodableValue _description = new CodableValue();

        /// <summary>
        /// Gets or sets the status of the concern.
        /// </summary>
        /// 
        /// <remarks>
        /// Examples of status include active or inactive. If the 
        /// status is not known the value should be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets the description of a concern instance.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the concern item.
        /// </returns>
        /// 
        public override string ToString()
        {
            return _description.ToString();
        }
    }
}
