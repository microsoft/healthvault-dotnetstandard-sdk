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
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
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
        public static new readonly Guid TypeId =
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

            this.when = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "when");

            this.name =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "name");

            this.primaryProvider =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "primary-provider");

            this.anatomicLocation =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "anatomic-location");

            this.secondaryProvider =
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
            Validator.ThrowSerializationIfNull(this.name, "ProcedureNameNotSet");

            writer.WriteStartElement("procedure");

            XmlWriterHelper.WriteOpt(
                writer,
                "when",
                this.when);

            XmlWriterHelper.WriteOpt(
                writer,
                "name",
                this.name);

            XmlWriterHelper.WriteOpt(
                writer,
                "anatomic-location",
                this.anatomicLocation);

            XmlWriterHelper.WriteOpt(
                writer,
                "primary-provider",
                this.primaryProvider);

            XmlWriterHelper.WriteOpt(
                writer,
                "secondary-provider",
                this.secondaryProvider);

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
            get { return this.when; }
            set { this.when = value; }
        }

        private ApproximateDateTime when = new ApproximateDateTime();

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
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "ProcedureNameNullValue");
                this.name = value;
            }
        }

        private CodableValue name;

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
            get { return this.primaryProvider; }
            set { this.primaryProvider = value; }
        }

        private PersonItem primaryProvider;

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
            get { return this.anatomicLocation; }
            set { this.anatomicLocation = value; }
        }

        private CodableValue anatomicLocation;

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
            get { return this.secondaryProvider; }
            set { this.secondaryProvider = value; }
        }

        private PersonItem secondaryProvider;

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
            string result = string.Empty;

            if (this.Name != null && this.AnatomicLocation != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "ProcedureToStringFormatNameAndLocation"),
                        this.Name.ToString(),
                        this.AnatomicLocation.ToString());
            }
            else if (this.Name != null)
            {
                result = this.Name.ToString();
            }
            else if (this.AnatomicLocation != null)
            {
                result =
                    string.Format(
                        ResourceRetriever.GetResourceString(
                            "ProcedureToStringFormatLocation"),
                        this.AnatomicLocation.ToString());
            }

            return result;
        }
    }
}
