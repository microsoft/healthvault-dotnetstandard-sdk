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
            this.DateSubmitted = dateSubmitted;
            this.Patient = patient;
            this.Plan = plan;
            this.MemberId = memberId;
            this.ClaimType = claimType;
            this.ClaimId = claimId;
            this.SubmittedBy = submittedBy;
            this.Provider = provider;
            this.Currency = currency;
            this.ClaimTotals = claimTotals;

            Validator.ThrowIfArgumentNull(services, nameof(services), Resources.ServicesMandatory);

            foreach (Service val in services)
            {
                this.Services.Add(val);
            }

            if (this.Services.Count == 0)
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

            this.dateSubmitted = XPathHelper.GetOptNavValue<HealthServiceDateTime>(itemNav, "date-submitted");
            this.patient = XPathHelper.GetOptNavValue<PersonItem>(itemNav, "patient");
            this.relationshipToMember = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "relationship-to-member");
            this.plan = XPathHelper.GetOptNavValue<Organization>(itemNav, "plan");
            this.groupId = XPathHelper.GetOptNavValue(itemNav, "group-id");
            this.memberId = itemNav.SelectSingleNode("member-id").Value;
            this.claimType = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "claim-type");
            this.claimId = itemNav.SelectSingleNode("claim-id").Value;
            this.submittedBy = XPathHelper.GetOptNavValue<Organization>(itemNav, "submitted-by");
            this.provider = XPathHelper.GetOptNavValue<Organization>(itemNav, "provider");
            this.currency = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "currency");
            this.claimTotals = XPathHelper.GetOptNavValue<ClaimAmounts>(itemNav, "claim-totals");

            this.services.Clear();
            foreach (XPathNavigator servicesNav in itemNav.Select("services"))
            {
                Service service = new Service();
                service.ParseXml(servicesNav);
                this.services.Add(service);
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
            Validator.ThrowSerializationIfNull(this.dateSubmitted, Resources.DateSubmittedNullValue);
            Validator.ThrowSerializationIfNull(this.patient, Resources.PatientNullValue);
            Validator.ThrowSerializationIfNull(this.plan, Resources.PlanNullValue);
            Validator.ThrowSerializationIfNull(this.memberId, Resources.MemberIdNullOrEmptyValue);
            Validator.ThrowSerializationIfNull(this.claimType, Resources.ClaimTypeNullValue);
            Validator.ThrowSerializationIfNull(this.claimId, Resources.ClaimIdNullOrEmptyValue);
            Validator.ThrowSerializationIfNull(this.submittedBy, Resources.SubmittedByNullValue);
            Validator.ThrowSerializationIfNull(this.provider, Resources.ProviderNullValue);
            Validator.ThrowSerializationIfNull(this.currency, Resources.CurrencyNullValue);
            Validator.ThrowSerializationIfNull(this.claimTotals, Resources.ClaimTotalsNullValue);

            if (this.services == null || this.services.Count == 0)
            {
                throw new ThingSerializationException(Resources.ServicesMandatory);
            }

            writer.WriteStartElement("explanation-of-benefits");

            this.dateSubmitted.WriteXml("date-submitted", writer);
            this.patient.WriteXml("patient", writer);
            XmlWriterHelper.WriteOpt(writer, "relationship-to-member", this.relationshipToMember);
            this.plan.WriteXml("plan", writer);
            XmlWriterHelper.WriteOptString(writer, "group-id", this.groupId);
            writer.WriteElementString("member-id", this.memberId);
            this.claimType.WriteXml("claim-type", writer);
            writer.WriteElementString("claim-id", this.claimId);
            this.submittedBy.WriteXml("submitted-by", writer);
            this.provider.WriteXml("provider", writer);
            this.currency.WriteXml("currency", writer);
            this.claimTotals.WriteXml("claim-totals", writer);

            foreach (Service service in this.services)
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
            get { return this.dateSubmitted; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.DateSubmitted), Resources.DateSubmittedNullValue);
                this.dateSubmitted = value;
            }
        }

        private HealthServiceDateTime dateSubmitted;

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
            get { return this.patient; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Patient), Resources.PatientNullValue);
                this.patient = value;
            }
        }

        private PersonItem patient;

        /// <summary>
        /// Gets or sets the relationship of the patient to the primary plan member.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about the relationshipToMember the value should be set to <b>null</b>.
        /// </remarks>
        public CodableValue RelationshipToMember
        {
            get { return this.relationshipToMember; }
            set { this.relationshipToMember = value; }
        }

        private CodableValue relationshipToMember;

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
            get { return this.plan; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Plan), Resources.PlanNullValue);
                this.plan = value;
            }
        }

        private Organization plan;

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
            get { return this.groupId; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "GroupId");
                this.groupId = value;
            }
        }

        private string groupId;

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
            get { return this.memberId; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.MemberId), Resources.MemberIdNullOrEmptyValue);
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "MemberId");
                this.memberId = value;
            }
        }

        private string memberId;

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
            get { return this.claimType; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ClaimType), Resources.ClaimTypeNullValue);
                this.claimType = value;
            }
        }

        private CodableValue claimType;

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
            get { return this.claimId; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ClaimId), Resources.ClaimIdNullOrEmptyValue);
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "ClaimId");
                this.claimId = value;
            }
        }

        private string claimId;

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
            get { return this.submittedBy; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.SubmittedBy), Resources.SubmittedByNullValue);
                this.submittedBy = value;
            }
        }

        private Organization submittedBy;

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
            get { return this.provider; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Provider), Resources.ProviderNullValue);
                this.provider = value;
            }
        }

        private Organization provider;

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
            get { return this.currency; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Currency), Resources.CurrencyNullValue);
                this.currency = value;
            }
        }

        private CodableValue currency;

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
            get { return this.claimTotals; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.ClaimTotals), Resources.ClaimTotalsNullValue);
                this.claimTotals = value;
            }
        }

        private ClaimAmounts claimTotals;

        /// <summary>
        /// Gets a collection of the services included in this claim.
        /// </summary>
        ///
        public Collection<Service> Services => this.services;

        private readonly Collection<Service> services = new Collection<Service>();

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
                    this.provider.Name,
                    this.claimType.Text,
                    this.claimTotals.ChargedAmount,
                    this.currency.Text);

            return value;
        }
    }
}
