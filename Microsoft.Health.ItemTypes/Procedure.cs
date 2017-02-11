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
    /// Represents a health record item type that encapsulates a medical procedure.
    /// </summary>
    /// 
    public class Procedure : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Procedure"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
        /// is called.
        /// </remarks>
        /// 
        public Procedure()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Procedure"/> class with the 
        /// specified date and time.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the procedure.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public Procedure(CodableValue name)
            : base(TypeId)
        {
            this.Name = name;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        /// <value>
        /// A GUID.
        /// </value>
        /// 
        public new static readonly Guid TypeId =
            new Guid("df4db479-a1ba-42a2-8714-2b083b88150f");

        /// <summary>
        /// Populates this procedure instance from the data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the procedure data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a procedure node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("procedure");

            Validator.ThrowInvalidIfNull(itemNav, "ProcedureUnexpectedNode");

            _when = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "when");

            _name =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "name");
            
            _primaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "primary-provider");

            _anatomicLocation =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "anatomic-location");

            _secondaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "secondary-provider");
        }

        /// <summary>
        /// Writes the procedure data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the procedure data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Name"/> property has not been set.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, "ProcedureNameNotSet");

            writer.WriteStartElement("procedure");

            XmlWriterHelper.WriteOpt<ApproximateDateTime>(
                writer,
                "when",
                _when);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "name",
                _name);

            XmlWriterHelper.WriteOpt<CodableValue>(
                writer,
                "anatomic-location",
                _anatomicLocation);      
            
            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "primary-provider",
                _primaryProvider);


            XmlWriterHelper.WriteOpt<PersonItem>(
                writer,
                "secondary-provider",
                _secondaryProvider);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the procedure occurred.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="ApproximateDateTime"/> instance representing 
        /// the date. The default value is the current year.
        /// </value>
        /// 
        public ApproximateDateTime When
        {
            get { return _when; }
            set { _when = value; }
        }
        private ApproximateDateTime _when = new ApproximateDateTime();

        /// <summary>
        /// Gets or sets the name of the procedure.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> representing the title.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the title should not be 
        /// stored.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="Name"/> is <b>null</b>.
        /// </exception>
        /// 
        public CodableValue Name
        {
            get { return _name; }
            set 
            {
                Validator.ThrowIfArgumentNull(value, "Name", "ProcedureNameNullValue");
                _name = value;
            }
        }
        private CodableValue _name;

        /// <summary>
        /// Gets or sets the primary provider contact information.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the primary provider contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem PrimaryProvider
        {
            get { return _primaryProvider; }
            set { _primaryProvider = value; }
        }
        private PersonItem _primaryProvider;

        /// <summary>
        /// Gets or sets the anatomic location for the procedure.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="CodableValue"/> representing the location.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the location should not be 
        /// stored.
        /// </remarks>
        /// 
        public CodableValue AnatomicLocation
        {
            get { return _anatomicLocation; }
            set { _anatomicLocation = value; }
        }
        private CodableValue _anatomicLocation;

        /// <summary>
        /// Gets or sets the secondary provider contact information.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if the secondary provider contact information
        /// should not be stored.
        /// </remarks>
        /// 
        public PersonItem SecondaryProvider
        {
            get { return _secondaryProvider; }
            set { _secondaryProvider = value; }
        }
        private PersonItem _secondaryProvider;

        /// <summary>
        /// Gets a string representation of the procedure.
        /// </summary>
        /// 
        /// <returns>
        /// A string representing the procedure.
        /// </returns>
        /// 
        public override string ToString()
        {
            string result = String.Empty;

            if (Name != null && AnatomicLocation != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "ProcedureToStringFormatNameAndLocation"),
                        Name.ToString(),
                        AnatomicLocation.ToString());
            }
            else if (Name != null)
            {
                result = Name.ToString();
            }
            else if (AnatomicLocation != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "ProcedureToStringFormatLocation"),
                        AnatomicLocation.ToString());
            }

            return result;
        }

    }
}
