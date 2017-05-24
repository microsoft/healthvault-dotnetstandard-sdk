// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Describes the benefits received from an insurance plan.
    /// </summary>
    ///
    public class ExplanationOfBenefits : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ExplanationOfBenefits"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called
        /// </remarks>
        ///
        public ExplanationOfBenefits()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ExplanationOfBenefits"/> class specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        /// <param name="dateSubmitted">
        /// The date when the claim was submitted.
        /// </param>
        /// <param name="patient">
        /// The name of the patient.
        /// </param>
        /// <param name="plan">
        /// The plan covering this claim.
        /// </param>
        /// <param name="memberId">
        /// The member id of the plan member.
        /// </param>
        /// <param name="claimType">
        /// The type of the claim (medical, dental, etc.)
        /// </param>
        /// <param name="claimId">
        /// The claim id.
        /// </param>
        /// <param name="submittedBy">
        /// The organization that submitted this claim.
        /// </param>
        /// <param name="provider">
        /// The provider that performed the services.
        /// </param>
        /// <param name="currency">
        /// The currency used.
        /// </param>
        /// <param name="claimTotals">
        /// A summary of the financial information about this claim.
        /// </param>
        /// <param name="services">
        /// The service included in this claim.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="memberId"/> is empty or contains only whitespace.
        /// If <paramref name="claimId"/> is empty or contains only whitespace.
        /// If <paramref name="services"/> is <b>null</b> or doesn't contain any values.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="memberId"/> is <b>null</b>.
        /// If <paramref name="claimId"/> is <b>null</b>.
        /// If <paramref name="dateSubmitted"/> is <b>null</b>.
        /// If <paramref name="patient"/> is <b>null</b>.
        /// If <paramref name="plan"/> is <b>null</b>.
        /// If <paramref name="claimType"/> is <b>null</b>.
        /// If <paramref name="submittedBy"/> is <b>null</b>.
        /// If <paramref name="provider"/> is <b>null</b>.
        /// If <paramref name="currency"/> is <b>null</b>.
        /// If <paramref name="claimTotals"/> is <b>null</b>.
        /// </exception>
        ///
        public ExplanationOfBenefits(
            HealthServiceDateTime dateSubmitted,
            PersonItem patient,
            Organization plan,
            string memberId,
            CodableValue claimType,
            string claimId,
            Organization submittedBy,
            Organization provider,
            CodableValue currency,
            ClaimAmounts claimTotals,
            IEnumerable<Service> services)
            : base(TypeId)
        {
            DateSubmitted = dateSubmitted;
            Patient = patient;
            Plan = plan;
            MemberId = memberId;
            ClaimType = claimType;
            ClaimId = claimId;
            SubmittedBy = submittedBy;
            Provider = provider;
            Currency = currency;
            ClaimTotals = claimTotals;

            Validator.ThrowIfArgumentNull(services, nameof(services), Resources.ServicesMandatory);

            foreach (Service val in services)
            {
                Services.Add(val);
            }

            if (Services.Count == 0)
            {
                throw new ArgumentException(Resources.ServicesMandatory, nameof(services));
            }
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId = new Guid("356fbba9-e0c9-4f4f-b0d9-4594f2490d2f");

        /// <summary>
        /// Populates this <see cref="ExplanationOfBenefits"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the ExplanationOfBenefits data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a ExplanationOfBenefits node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("explanation-of-benefits");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ExplanationOfBenefitsUnexpectedNode);

            _dateSubmitted = XPathHelper.GetOptNavValue<HealthServiceDateTime>(itemNav, "date-submitted");
            _patient = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "patient");
            _relationshipToMember = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "relationship-to-member");
            _plan = XPathHelper.GetOptNavValue<Organization>(itemNav, "plan");
            _groupId = XPathHelper.GetOptNavValue(itemNav, "group-id");
            _memberId = itemNav.SelectSingleNode("member-id").Value;
            _claimType = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "claim-type");
            _claimId = itemNav.SelectSingleNode("claim-id").Value;
            _submittedBy = XPathHelper.GetOptNavValue<Organization>(itemNav, "submitted-by");
            _provider = XPathHelper.GetOptNavValue<Organization>(itemNav, "provider");
            _currency = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "currency");
            _claimTotals = XPathHelper.GetOptNavValue<ClaimAmounts>(itemNav, "claim-totals");

            _services.Clear();
            foreach (XPathNavigator servicesNav in itemNav.Select("services"))
            {
                Service service = new Service();
                service.ParseXml(servicesNav);
                _services.Add(service);
            }
        }

        /// <summary>
        /// Writes the XML representation of the ExplanationOfBenefits into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the ExplanationOfBenefits should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="DateSubmitted"/> is <b>null</b>.
        /// If <see cref="Patient"/> is <b>null</b>.
        /// If <see cref="Plan"/> is <b>null</b>.
        /// If <see cref="MemberId"/> is <b>null</b>.
        /// If <see cref="ClaimType"/> is <b>null</b>.
        /// If <see cref="ClaimId"/> is <b>null</b>.
        /// If <see cref="SubmittedBy"/> is <b>null</b>.
        /// If <see cref="Provider"/> is <b>null</b>.
        /// If <see cref="Currency"/> is <b>null</b>.
        /// If <see cref="ClaimTotals"/> is <b>null</b>.
        /// If <see cref="Services"/> collection is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_dateSubmitted, Resources.DateSubmittedNullValue);
            Validator.ThrowSerializationIfNull(_patient, Resources.PatientNullValue);
            Validator.ThrowSerializationIfNull(_plan, Resources.PlanNullValue);
            Validator.ThrowSerializationIfNull(_memberId, Resources.MemberIdNullOrEmptyValue);
            Validator.ThrowSerializationIfNull(_claimType, Resources.ClaimTypeNullValue);
            Validator.ThrowSerializationIfNull(_claimId, Resources.ClaimIdNullOrEmptyValue);
            Validator.ThrowSerializationIfNull(_submittedBy, Resources.SubmittedByNullValue);
            Validator.ThrowSerializationIfNull(_provider, Resources.ProviderNullValue);
            Validator.ThrowSerializationIfNull(_currency, Resources.CurrencyNullValue);
            Validator.ThrowSerializationIfNull(_claimTotals, Resources.ClaimTotalsNullValue);

            if (_services == null || _services.Count == 0)
            {
                throw new ThingSerializationException(Resources.ServicesMandatory);
            }

            writer.WriteStartElement("explanation-of-benefits");

            _dateSubmitted.WriteXml("date-submitted", writer);
            _patient.WriteXml("patient", writer);
            XmlWriterHelper.WriteOpt(writer, "relationship-to-member", _relationshipToMember);
            _plan.WriteXml("plan", writer);
            XmlWriterHelper.WriteOptString(writer, "group-id", _groupId);
            writer.WriteElementString("member-id", _memberId);
            _claimType.WriteXml("claim-type", writer);
            writer.WriteElementString("claim-id", _claimId);
            _submittedBy.WriteXml("submitted-by", writer);
            _provider.WriteXml("provider", writer);
            _currency.WriteXml("currency", writer);
            _claimTotals.WriteXml("claim-totals", writer);

            foreach (Service service in _services)
            {
                service.WriteXml("services", writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the claim was submitted.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime DateSubmitted
        {
            get { return _dateSubmitted; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(DateSubmitted), Resources.DateSubmittedNullValue);
                _dateSubmitted = value;
            }
        }

        private HealthServiceDateTime _dateSubmitted;

        /// <summary>
        /// Gets or sets the name of the patient.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public PersonItem Patient
        {
            get { return _patient; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Patient), Resources.PatientNullValue);
                _patient = value;
            }
        }

        private PersonItem _patient;

        /// <summary>
        /// Gets or sets the relationship of the patient to the primary plan member.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the relationshipToMember the value should be set to <b>null</b>.
        /// </remarks>
        public CodableValue RelationshipToMember
        {
            get { return _relationshipToMember; }
            set { _relationshipToMember = value; }
        }

        private CodableValue _relationshipToMember;

        /// <summary>
        /// Gets or sets the plan covering this claim.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Organization Plan
        {
            get { return _plan; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Plan), Resources.PlanNullValue);
                _plan = value;
            }
        }

        private Organization _plan;

        /// <summary>
        /// Gets or sets the group id for the member's plan.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the groupId the value should be set to <b>null</b>
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string GroupId
        {
            get { return _groupId; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "GroupId");
                _groupId = value;
            }
        }

        private string _groupId;

        /// <summary>
        /// Gets or sets the member id of the plan member.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> is empty or contains only whitespace.
        /// </exception>
        ///
        public string MemberId
        {
            get { return _memberId; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(MemberId), Resources.MemberIdNullOrEmptyValue);
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "MemberId");
                _memberId = value;
            }
        }

        private string _memberId;

        /// <summary>
        /// Gets or sets the type of the claim (medical, dental, etc.)
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue ClaimType
        {
            get { return _claimType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ClaimType), Resources.ClaimTypeNullValue);
                _claimType = value;
            }
        }

        private CodableValue _claimType;

        /// <summary>
        /// Gets or sets the claim id.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> is empty or contains only whitespace.
        /// </exception>
        ///
        public string ClaimId
        {
            get { return _claimId; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ClaimId), Resources.ClaimIdNullOrEmptyValue);
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "ClaimId");
                _claimId = value;
            }
        }

        private string _claimId;

        /// <summary>
        /// Gets or sets the organization that submitted this claim.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Organization SubmittedBy
        {
            get { return _submittedBy; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(SubmittedBy), Resources.SubmittedByNullValue);
                _submittedBy = value;
            }
        }

        private Organization _submittedBy;

        /// <summary>
        /// Gets or sets the provider that performed the services.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Organization Provider
        {
            get { return _provider; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Provider), Resources.ProviderNullValue);
                _provider = value;
            }
        }

        private Organization _provider;

        /// <summary>
        /// Gets or sets the currency used.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Currency
        {
            get { return _currency; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Currency), Resources.CurrencyNullValue);
                _currency = value;
            }
        }

        private CodableValue _currency;

        /// <summary>
        /// Gets or sets a summary of the financial information about this claim.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ClaimAmounts ClaimTotals
        {
            get { return _claimTotals; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(ClaimTotals), Resources.ClaimTotalsNullValue);
                _claimTotals = value;
            }
        }

        private ClaimAmounts _claimTotals;

        /// <summary>
        /// Gets a collection of the services included in this claim.
        /// </summary>
        ///
        public Collection<Service> Services => _services;

        private readonly Collection<Service> _services = new Collection<Service>();

        /// <summary>
        /// Gets a string representation of the ExplanationOfBenefits.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the ExplanationOfBenefits.
        /// </returns>
        ///
        public override string ToString()
        {
            string value =
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ExplanationOfBenefitsToStringFormat,
                    _provider.Name,
                    _claimType.Text,
                    _claimTotals.ChargedAmount,
                    _currency.Text);

            return value;
        }
    }
}
