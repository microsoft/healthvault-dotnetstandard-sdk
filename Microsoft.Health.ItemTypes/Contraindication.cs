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
    /// Represents a health record item type that encapsulates a contraindication.
    /// </summary>
    /// 
    /// <remarks>
    /// A contraindication consists of the substances (such as foods or drugs) that interacts badly
    /// with a specific condition or drugs a patient is already taking.
    /// </remarks>
    /// 
    public class Contraindication : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Contraindication"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Contraindication()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Contraindication"/> class with the 
        /// specified substance and status.
        /// </summary>
        /// 
        /// <param name="substance">
        /// The substance that is contraindicated.
        /// </param>
        /// 
        /// <param name="status">
        /// The status of the contraindication. Usually 'Active' or 'inactive'.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="substance"/> or <paramref name="status"/> is <b>null</b>.
        /// </exception>
        /// 
        public Contraindication(CodableValue substance, CodableValue status)
            : base(TypeId)
        {
            this.Substance = substance;
            this.Status = status;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// <value>A GUID.</value>
        public new static readonly Guid TypeId =
            new Guid("046d0ad7-6d7f-4bfd-afd4-4192ca2e913d");

        /// <summary>
        /// Populates this contraindication instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the contraindication data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a contraindication node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("contraindication");

            Validator.ThrowInvalidIfNull(itemNav, "ContraindicationUnexpectedNode");

            // <substance>
            _substance = new CodableValue();
            _substance.ParseXml(itemNav.SelectSingleNode("substance"));

            // <status>
            _status = new CodableValue();
            _status.ParseXml(itemNav.SelectSingleNode("status"));

            // <source>
            _source = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "source");

            // <documenter>
            _documenter = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "documenter");

            // <documented-date>
            _documentedDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "documented-date");

        }

        /// <summary>
        /// Writes the contraindication data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the contraindication data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Substance"/> or <see cref="Status"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_substance, "ContraindicationSubstanceNotSet");
            Validator.ThrowSerializationIfNull(_status, "ContraindicationStatusNotSet");
           
            // <contraindication>
            writer.WriteStartElement("contraindication");

            // <substance>
            _substance.WriteXml("substance", writer);

            // <status>
            _status.WriteXml("status", writer);

            // <source>
            XmlWriterHelper.WriteOpt<CodableValue>(writer, "source", _source);

            // <documenter>
            XmlWriterHelper.WriteOpt<PersonItem>(writer, "documenter", _documenter);

            // <documented-date>
            XmlWriterHelper.WriteOpt<ApproximateDateTime>(writer, "documented-date", _documentedDate);

            // </contraindication>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the substance of the contraindication.
        /// </summary>
        /// 
        /// <value>
        /// A codable value indicating the substance that is contraindicated.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Substance
        {
            get { return _substance; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Substance", "ContraindicationSubstanceMandatory");
                _substance = value;
            }
        }
        private CodableValue _substance;

        /// <summary>
        /// Gets or sets the status of the contraindication.
        /// </summary>
        /// 
        /// <value>
        /// The status of the contraindication is usually 'active' or 'inactive'.
        /// </value>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Status
        {
            get { return _status; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Status", "ContraindicationStatusMandatory");
                _status = value;
            }
        }
        private CodableValue _status;

        /// <summary>
        /// Gets or sets the source of the contraindication.
        /// </summary>
        /// 
        /// <value>
        /// For example, a contraindication against leafy green
        /// vegetables might have as a source the blood thinner Warfarin, which would be coded here
        /// using an appropriate medication vocabulary.
        /// </value>
        /// 
        /// <remarks>
        /// If the source is unknown the value will be set to <b>null</b>.
        /// </remarks>
        /// 
        public CodableValue Source
        {
            get { return _source; }
            set { _source = value; }
        }
        private CodableValue _source;

        /// <summary>
        /// Gets or sets the documenter of the contraindication.
        /// </summary>
        /// 
        /// <value>
        /// For example, a contraindication against leafy green
        /// vegetables might have as a source the blood thinner Warfarin, which would be coded here
        /// using an appropriate medication vocabulary.
        /// </value>
        /// 
        /// <remarks>
        /// This is the person that documented the issue (physician, pharmacist, etc.)
        /// If the documenter is unknown the value will be set to <b>null</b>.
        /// </remarks>
        /// 
        public PersonItem Documenter
        {
            get { return _documenter; }
            set { _documenter = value; }
        }
        private PersonItem _documenter;

        /// <summary>
        /// Gets or sets the date the contraindication was documented.
        /// </summary>
        /// 
        /// <remarks>
        /// If the documented date is unknown the value will be set to <b>null</b>.
        /// </remarks>
        /// 
        public ApproximateDateTime DocumentedDate
        {
            get { return _documentedDate; }
            set { _documentedDate = value; }
        }
        private ApproximateDateTime _documentedDate;


        /// <summary>
        /// Gets a string representation of the contraindication item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the contraindication item.
        /// </returns>
        /// 
        public override string ToString()
        {

            return Substance != null ? Substance.Text : String.Empty;
        }

    }
}
