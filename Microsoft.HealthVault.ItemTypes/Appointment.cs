// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Things;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a medical appointment.
    /// </summary>
    ///
    public class Appointment : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Appointment"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item isn't added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Appointment()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Appointment"/> class with
        /// the specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date and time for the appointment.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Appointment(HealthServiceDateTime when)
            : base(TypeId)
        {
            this.When = when;
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
            new Guid("4B18AEB6-5F01-444C-8C70-DBF13A2F510B");

        /// <summary>
        /// Populates this appointment instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the appointment data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an appointment node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator appointmentNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("appointment");

            Validator.ThrowInvalidIfNull(appointmentNav, "AppointmentUnexpectedNode");

            // <when>
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(appointmentNav.SelectSingleNode("when"));

            // <duration>
            this.duration =
                XPathHelper.GetOptNavValue<DurationValue>(
                    appointmentNav,
                    "duration");

            // <service>
            XPathNavigator serviceNav =
                appointmentNav.SelectSingleNode("service");
            if (serviceNav != null)
            {
                this.service = new CodableValue();
                this.service.ParseXml(serviceNav);
            }

            // <clinic>
            XPathNavigator clinicNav =
                appointmentNav.SelectSingleNode("clinic");

            if (clinicNav != null)
            {
                this.clinic = new PersonItem();
                this.clinic.ParseXml(clinicNav);
            }

            // <specialty>
            XPathNavigator specialtyNav =
                appointmentNav.SelectSingleNode("specialty");
            if (specialtyNav != null)
            {
                this.specialty = new CodableValue();
                this.specialty.ParseXml(specialtyNav);
            }

            // <status>
            XPathNavigator statusNav =
                appointmentNav.SelectSingleNode("status");
            if (statusNav != null)
            {
                this.status = new CodableValue();
                this.status.ParseXml(statusNav);
            }

            // <care-class>
            XPathNavigator careClassNav =
                appointmentNav.SelectSingleNode("care-class");
            if (careClassNav != null)
            {
                this.careClass = new CodableValue();
                this.careClass.ParseXml(careClassNav);
            }
        }

        /// <summary>
        /// Writes the appointment data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the appointment data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            Validator.ThrowSerializationIfNull(this.when, "AppointmentWhenNotSet");

            // <appointment>
            writer.WriteStartElement("appointment");

            // <when>
            this.when.WriteXml("when", writer);

            // <duration>
            XmlWriterHelper.WriteOpt(
                writer,
                "duration",
                this.duration);

            // <service>
            this.Service?.WriteXml("service", writer);

            // <clinic>
            this.Clinic?.WriteXml("clinic", writer);

            // <specialty>
            this.Specialty?.WriteXml("specialty", writer);

            // <status>
            this.Status?.WriteXml("status", writer);

            // <care-class>
            this.CareClass?.WriteXml("care-class", writer);

            // </appointment>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the appointment occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date. The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the duration of the appointment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="DurationValue"/> instance representing  the duration.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the duration should not be stored.
        /// </remarks>
        ///
        public DurationValue Duration
        {
            get { return this.duration; }
            set { this.duration = value; }
        }

        private DurationValue duration;

        /// <summary>
        /// Gets or sets the service for the appointment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the service.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the service should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue Service
        {
            get { return this.service; }
            set { this.service = value; }
        }

        private CodableValue service;

        /// <summary>
        /// Gets or sets the clinic information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> instance representing the clinic information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the clinic information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Clinic
        {
            get { return this.clinic; }
            set { this.clinic = value; }
        }

        private PersonItem clinic;

        /// <summary>
        /// Gets or sets the specialty for the appointment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the specialty.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the specialty should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue Specialty
        {
            get { return this.specialty; }
            set { this.specialty = value; }
        }

        private CodableValue specialty;

        /// <summary>
        /// Gets or sets the status for the appointment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the status.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the status should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        private CodableValue status;

        /// <summary>
        /// Gets or sets the care class for the appointment.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the care class.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the care class should not be
        /// stored.
        /// </remarks>
        ///
        public CodableValue CareClass
        {
            get { return this.careClass; }
            set { this.careClass = value; }
        }

        private CodableValue careClass;

        /// <summary>
        /// Gets a string representation of the appointment item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the appointment item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(this.When);

            if (this.Duration != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Duration.ToString());
            }

            if (this.Clinic != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Clinic.ToString());
            }

            if (this.Status != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Status.Text);
            }

            if (this.Service != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Service.Text);
            }

            return result.ToString();
        }
    }
}
