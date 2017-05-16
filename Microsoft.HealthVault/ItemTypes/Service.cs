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
            ServiceType = serviceType;
            ServiceDates = serviceDates;
            ClaimAmounts = claimAmounts;
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

            _serviceType = XPathHelper.GetOptNavValue<CodableValue>(navigator, "service-type");
            _diagnosis = XPathHelper.GetOptNavValue<CodableValue>(navigator, "diagnosis");
            _billingCode = XPathHelper.GetOptNavValue<CodableValue>(navigator, "billing-code");
            _serviceDates = XPathHelper.GetOptNavValue<DurationValue>(navigator, "service-dates");

            _claimAmounts = new ClaimAmounts();
            _claimAmounts.ParseXml(navigator.SelectSingleNode("claim-amounts"));

            _notes.Clear();
            foreach (XPathNavigator notesNav in navigator.Select("notes"))
            {
                _notes.Add(notesNav.Value);
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
            Validator.ThrowSerializationIfNull(_serviceType, Resources.ServiceTypeNullValue);
            Validator.ThrowSerializationIfNull(_serviceDates, Resources.ServiceDatesNullValue);
            Validator.ThrowSerializationIfNull(_claimAmounts, Resources.ClaimAmountsNullValue);

            writer.WriteStartElement(nodeName);

            _serviceType.WriteXml("service-type", writer);

            XmlWriterHelper.WriteOpt(writer, "diagnosis", _diagnosis);
            XmlWriterHelper.WriteOpt(writer, "billing-code", _billingCode);

            _serviceDates.WriteXml("service-dates", writer);
            _claimAmounts.WriteXml("claim-amounts", writer);

            foreach (string note in _notes)
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
            get { return _serviceType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ServiceType), Resources.ServiceTypeNullValue);
                _serviceType = value;
            }
        }

        private CodableValue _serviceType;

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
            get { return _diagnosis; }
            set { _diagnosis = value; }
        }

        private CodableValue _diagnosis;

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
            get { return _billingCode; }
            set { _billingCode = value; }
        }

        private CodableValue _billingCode;

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
            get { return _serviceDates; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ServiceDates), Resources.ServiceDatesNullValue);
                _serviceDates = value;
            }
        }

        private DurationValue _serviceDates;

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
            get { return _claimAmounts; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ClaimAmounts), Resources.ClaimAmountsNullValue);
                _claimAmounts = value;
            }
        }

        private ClaimAmounts _claimAmounts;

        /// <summary>
        /// Gets a collection of additional information about this service.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the notes the collection should be empty.
        /// </remarks>
        ///
        public Collection<string> Notes => _notes;

        private readonly Collection<string> _notes = new Collection<string>();

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
            return ServiceType.Text;
        }
    }
}
