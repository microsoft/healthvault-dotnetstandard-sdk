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
    /// Represents thing type that contains information about an immunization.
    /// </summary>
    ///
    public class Immunization : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Immunization"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Immunization()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Immunization"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the immunization.
        /// </param>
        ///
        /// <param name="dateAdministrated">
        /// The approximate date that the immunization was adminstrated.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Immunization(CodableValue name, ApproximateDateTime dateAdministrated)
            : base(TypeId)
        {
            this.Name = name;
            this.DateAdministrated = dateAdministrated;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Immunization"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The name of the immunization.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is null.
        /// </exception>
        ///
        public Immunization(CodableValue name)
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
            new Guid("cd3587b5-b6e1-4565-ab3b-1c3ad45eb04f");

        /// <summary>
        /// Populates this <see cref="Immunization"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the immunization data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an immunization node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "immunization");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ImmunizationUnexpectedNode);

            // <name>
            this.name.ParseXml(itemNav.SelectSingleNode("name"));

            // <administration-date>
            this.dateAdministrated =
                XPathHelper.GetOptNavValue<ApproximateDateTime>(
                    itemNav,
                    "administration-date");

            // <administrator>
            this.administrator =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "administrator");

            // <manufacturer>
            this.manufacturer =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "manufacturer");

            // <lot>
            this.lot = XPathHelper.GetOptNavValue(itemNav, "lot");

            // <route>
            this.route =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "route");

            // <expiration-date> approx-date-time
            this.expirationDate =
                XPathHelper.GetOptNavValue<ApproximateDate>(
                    itemNav,
                    "expiration-date");

            // <sequence>
            this.sequence =
                XPathHelper.GetOptNavValue(itemNav, "sequence");

            // <anatomic-surface>
            this.anatomicSurface =
                XPathHelper.GetOptNavValue<CodableValue>(
                    itemNav,
                    "anatomic-surface");

            // <adverse-event> string
            this.adverseEvent =
                XPathHelper.GetOptNavValue(itemNav, "adverse-event");

            // <consent>
            this.consent =
                XPathHelper.GetOptNavValue(itemNav, "consent");
        }

        /// <summary>
        /// Writes the immunization data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the immunization data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Name"/> has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name.Text, Resources.ImmunizationNameNotSet);

            // <immunization>
            writer.WriteStartElement("immunization");

            this.name.WriteXml("name", writer);

            // <administration-date>
            XmlWriterHelper.WriteOpt(
                writer,
                "administration-date",
                this.dateAdministrated);

            // <administrator>
            XmlWriterHelper.WriteOpt(
                writer,
                "administrator",
                this.Administrator);

            // <manufacturer>
            XmlWriterHelper.WriteOpt(
                writer,
                "manufacturer",
                this.manufacturer);

            // <lot>
            XmlWriterHelper.WriteOptString(
                writer,
                "lot",
                this.lot);

            // <route>
            XmlWriterHelper.WriteOpt(
                writer,
                "route",
                this.Route);

            // <expiration-date>
            XmlWriterHelper.WriteOpt(
                writer,
                "expiration-date",
                this.expirationDate);

            // <sequence>
            XmlWriterHelper.WriteOptString(
                writer,
                "sequence",
                this.sequence);

            // <anatomic-surface>
            XmlWriterHelper.WriteOpt(
                writer,
                "anatomic-surface",
                this.AnatomicSurface);

            // <adverse-event>
            XmlWriterHelper.WriteOptString(
                writer,
                "adverse-event",
                this.adverseEvent);

            // <consent>
            XmlWriterHelper.WriteOptString(
                writer,
                "consent",
                this.consent);

            // </immunization>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the immunization.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the name.
        /// </value>
        ///
        /// <remarks>
        /// The preferred vocabulary for the immunization name is "vaccines-cvx".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is null during set.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.ImmunizationNameMandatory);
                this.name = value;
            }
        }

        private CodableValue name = new CodableValue();

        /// <summary>
        /// Gets or sets the date the immunization was administrated.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="ApproximateDateTime"/> instance representing the date.
        /// </value>
        ///
        public ApproximateDateTime DateAdministrated
        {
            get { return this.dateAdministrated; }
            set { this.dateAdministrated = value; }
        }

        private ApproximateDateTime dateAdministrated;

        /// <summary>
        /// Gets or sets the name of the administrator of the immunization.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> instance representing the person.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the administrator should not be stored.
        /// </remarks>
        ///
        public PersonItem Administrator
        {
            get { return this.administrator; }
            set { this.administrator = value; }
        }

        private PersonItem administrator;

        /// <summary>
        /// Gets or sets the manufacturer of the vaccine.
        /// </summary>
        ///
        /// <value>
        /// A string representing the manufacturer.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the manufacturer should not be stored.
        /// The preferred vocabulary for the immunization manufacturer is "vaccine-manufacturers-mvx".
        /// </remarks>
        ///
        public CodableValue Manufacturer
        {
            get { return this.manufacturer; }
            set { this.manufacturer = value; }
        }

        private CodableValue manufacturer;

        /// <summary>
        /// Gets or sets the lot of the vaccine.
        /// </summary>
        ///
        /// <value>
        /// A string representing the lot.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the lot should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Lot
        {
            get { return this.lot; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Lot");
                this.lot = value;
            }
        }

        private string lot;

        /// <summary>
        /// Gets or sets the medical route for the immunization.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the route.
        /// </value>
        ///
        public CodableValue Route
        {
            get { return this.route; }
            set { this.route = value; }
        }

        private CodableValue route;

        /// <summary>
        /// Gets or sets the expiration date for the vaccine.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="ApproximateDate"/> instance representing the date.
        /// </value>
        ///
        public ApproximateDate ExpirationDate
        {
            get { return this.expirationDate; }
            set { this.expirationDate = value; }
        }

        private ApproximateDate expirationDate;

        /// <summary>
        /// Gets or sets the sequence for the immunization.
        /// </summary>
        ///
        /// <value>
        /// A string representing the sequence.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the consent should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Sequence
        {
            get { return this.sequence; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Sequence");
                this.sequence = value;
            }
        }

        private string sequence;

        /// <summary>
        /// Gets or sets the anatomic surface for the immunization.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the surface.
        /// </value>
        ///
        public CodableValue AnatomicSurface
        {
            get { return this.anatomicSurface; }
            set { this.anatomicSurface = value; }
        }

        private CodableValue anatomicSurface;

        /// <summary>
        /// Gets or sets any adverse event description for the immunization.
        /// </summary>
        ///
        /// <value>
        /// A string representing the event description.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the description should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string AdverseEvent
        {
            get { return this.adverseEvent; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "AdverseEvent");
                this.adverseEvent = value;
            }
        }

        private string adverseEvent;

        /// <summary>
        /// Gets or sets the consent description for the immunization.
        /// </summary>
        ///
        /// <value>
        /// A string representing the consent description.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to null if the consent should not be stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Consent
        {
            get { return this.consent; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Consent");
                this.consent = value;
            }
        }

        private string consent;

        /// <summary>
        /// Gets a string representation of the immunization item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the immunization item.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = this.Name.ToString();

            if (this.DateAdministrated != null)
            {
                result =
                    string.Format(
                        Resources.ImmunizationToStringFormat,
                        this.Name.ToString(),
                        this.DateAdministrated.ToString());
            }

            return result;
        }
    }
}
