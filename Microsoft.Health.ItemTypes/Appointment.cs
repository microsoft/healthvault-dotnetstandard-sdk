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
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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
            _when = new HealthServiceDateTime();
            _when.ParseXml(appointmentNav.SelectSingleNode("when"));

            // <duration>
            _duration =
                XPathHelper.GetOptNavValue<DurationValue>(
                    appointmentNav,
                    "duration");

            // <service>
            XPathNavigator serviceNav =
                appointmentNav.SelectSingleNode("service");
            if (serviceNav != null)
            {
                _service = new CodableValue();
                _service.ParseXml(serviceNav);
            }

            // <clinic>
            XPathNavigator clinicNav =
                appointmentNav.SelectSingleNode("clinic");

            if (clinicNav != null)
            {
                _clinic = new PersonItem();
                _clinic.ParseXml(clinicNav);
            }

            // <specialty>
            XPathNavigator specialtyNav =
                appointmentNav.SelectSingleNode("specialty");
            if (specialtyNav != null)
            {
                _specialty = new CodableValue();
                _specialty.ParseXml(specialtyNav);
            }

            // <status>
            XPathNavigator statusNav =
                appointmentNav.SelectSingleNode("status");
            if (statusNav != null)
            {
                _status = new CodableValue();
                _status.ParseXml(statusNav);
            }

            // <care-class>
            XPathNavigator careClassNav =
                appointmentNav.SelectSingleNode("care-class");
            if (careClassNav != null)
            {
                _careClass = new CodableValue();
                _careClass.ParseXml(careClassNav);
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
            Validator.ThrowSerializationIfNull(_when, "AppointmentWhenNotSet");

            // <appointment>
            writer.WriteStartElement("appointment");

            // <when>
            _when.WriteXml("when", writer);

            // <duration>
            XmlWriterHelper.WriteOpt<DurationValue>(
                writer,
                "duration",
                _duration);

            // <service>
            if (Service != null)
            {
                Service.WriteXml("service", writer);
            }

            // <clinic>
            if (Clinic != null)
            {
                Clinic.WriteXml("clinic", writer);
            }

            // <specialty>
            if (Specialty != null)
            {
                Specialty.WriteXml("specialty", writer);
            }

            // <status>
            if (Status != null)
            {
                Status.WriteXml("status", writer);
            }

            // <care-class>
            if (CareClass != null)
            {
                CareClass.WriteXml("care-class", writer);
            }

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
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when = new HealthServiceDateTime();

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
            get { return _duration; }
            set { _duration = value; }
        }
        private DurationValue _duration;

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
            get { return _service; }
            set { _service = value; }
        }
        private CodableValue _service;

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
            get { return _clinic; }
            set { _clinic = value; }
        }
        private PersonItem _clinic;
        
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
            get { return _specialty; }
            set { _specialty = value; }
        }
        private CodableValue _specialty;

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
            get { return _status; }
            set { _status = value; }
        }
        private CodableValue _status;
        
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
            get { return _careClass; }
            set { _careClass = value; }
        }
        private CodableValue _careClass;

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

            result.Append(When.ToString());

            if (Duration != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Duration.ToString());
            }

            if (Clinic != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Clinic.ToString());
            }

            if (Status != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Status.Text);
            }

            if (Service != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    Service.Text);
            }
            return result.ToString();
        }
    }
}
