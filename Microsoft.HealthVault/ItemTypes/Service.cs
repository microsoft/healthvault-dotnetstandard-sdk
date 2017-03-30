// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information about a specific service provided on an explanation of benefits.
    /// </summary>
    ///
    public class Service : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Service"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called
        /// </remarks>
        ///
        public Service()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Service"/> class specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        /// <param name="serviceType">
        /// The type of the service.
        /// </param>
        /// <param name="serviceDates">
        /// The dates for this service.
        /// </param>
        /// <param name="claimAmounts">
        /// The financial information for this service.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="serviceType"/> is <b> null </b>.
        /// If <paramref name="serviceDates"/> is <b> null </b>.
        /// If <paramref name="claimAmounts"/> is <b> null </b>.
        /// </exception>
        ///
        public Service(
            CodableValue serviceType,
            DurationValue serviceDates,
            ClaimAmounts claimAmounts)
        {
            this.ServiceType = serviceType;
            this.ServiceDates = serviceDates;
            this.ClaimAmounts = claimAmounts;
        }

        /// <summary>
        /// Populates this <see cref="Service"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the Service data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.serviceType = XPathHelper.GetOptNavValue<CodableValue>(navigator, "service-type");
            this.diagnosis = XPathHelper.GetOptNavValue<CodableValue>(navigator, "diagnosis");
            this.billingCode = XPathHelper.GetOptNavValue<CodableValue>(navigator, "billing-code");
            this.serviceDates = XPathHelper.GetOptNavValue<DurationValue>(navigator, "service-dates");

            this.claimAmounts = new ClaimAmounts();
            this.claimAmounts.ParseXml(navigator.SelectSingleNode("claim-amounts"));

            this.notes.Clear();
            foreach (XPathNavigator notesNav in navigator.Select("notes"))
            {
                this.notes.Add(notesNav.Value);
            }
        }

        /// <summary>
        /// Writes the XML representation of the Service into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the Service.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the Service should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="ServiceType"/> is <b>null</b>.
        /// If <see cref="ServiceDates"/> is <b>null</b>.
        /// If <see cref="ClaimAmounts"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.serviceType, Resources.ServiceTypeNullValue);
            Validator.ThrowSerializationIfNull(this.serviceDates, Resources.ServiceDatesNullValue);
            Validator.ThrowSerializationIfNull(this.claimAmounts, Resources.ClaimAmountsNullValue);

            writer.WriteStartElement(nodeName);

            this.serviceType.WriteXml("service-type", writer);

            XmlWriterHelper.WriteOpt(writer, "diagnosis", this.diagnosis);
            XmlWriterHelper.WriteOpt(writer, "billing-code", this.billingCode);

            this.serviceDates.WriteXml("service-dates", writer);
            this.claimAmounts.WriteXml("claim-amounts", writer);

            foreach (string note in this.notes)
            {
                writer.WriteElementString("notes", note);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b> null </b>.
        /// </exception>
        ///
        public CodableValue ServiceType
        {
            get { return this.serviceType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ServiceType), Resources.ServiceTypeNullValue);
                this.serviceType = value;
            }
        }

        private CodableValue serviceType;

        /// <summary>
        /// Gets or sets the diagnosis.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the diagnosis the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue Diagnosis
        {
            get { return this.diagnosis; }
            set { this.diagnosis = value; }
        }

        private CodableValue diagnosis;

        /// <summary>
        /// Gets or sets the billing code.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the billing code the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public CodableValue BillingCode
        {
            get { return this.billingCode; }
            set { this.billingCode = value; }
        }

        private CodableValue billingCode;

        /// <summary>
        /// Gets or sets the dates for this service.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public DurationValue ServiceDates
        {
            get { return this.serviceDates; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ServiceDates), Resources.ServiceDatesNullValue);
                this.serviceDates = value;
            }
        }

        private DurationValue serviceDates;

        /// <summary>
        /// Gets or sets the financial information for this service.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ClaimAmounts ClaimAmounts
        {
            get { return this.claimAmounts; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ClaimAmounts), Resources.ClaimAmountsNullValue);
                this.claimAmounts = value;
            }
        }

        private ClaimAmounts claimAmounts;

        /// <summary>
        /// Gets a collection of additional information about this service.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the notes the collection should be empty.
        /// </remarks>
        ///
        public Collection<string> Notes => this.notes;

        private readonly Collection<string> notes = new Collection<string>();

        /// <summary>
        /// Gets a string representation of the Service.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Service.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.ServiceType.Text;
        }
    }
}
