// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// The financial information related to a claim on an explanation of benefits.
    /// </summary>
    public class ClaimAmounts : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ClaimAmounts"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called
        /// </remarks>
        public ClaimAmounts()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ClaimAmounts"/> class specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called
        /// </remarks>
        ///
        /// <param name="chargedAmount">
        /// The amount charged.
        /// </param>
        /// <param name="negotiatedAmount">
        /// The amount negotiated between the provider and the payer.
        /// </param>
        /// <param name="coPayment">
        /// The copayment amount.
        /// </param>
        /// <param name="deductible">
        /// The deductible amount.
        /// </param>
        /// <param name="amountNotCovered">
        /// Amount for services not covered by the plan.
        /// </param>
        /// <param name="eligibleForBenefits">
        /// The amount that is eligible for benefits.
        /// </param>
        /// <param name="coinsurance">
        /// The amount paid by the person.
        /// </param>
        /// <param name="miscellaneousAdjustments">
        /// Adjustments that may affect the amount paid on the claim.
        /// </param>
        /// <param name="benefitsPaid">
        /// The amount paid by the payer.
        /// </param>
        /// <param name="patientResponsibility">
        /// The total amount paid by the patient.
        /// </param>
        ///
        public ClaimAmounts(
            decimal chargedAmount,
            decimal negotiatedAmount,
            decimal coPayment,
            decimal deductible,
            decimal amountNotCovered,
            decimal eligibleForBenefits,
            decimal coinsurance,
            decimal miscellaneousAdjustments,
            decimal benefitsPaid,
            decimal patientResponsibility)
        {
            this.ChargedAmount = chargedAmount;
            this.NegotiatedAmount = negotiatedAmount;
            this.CoPayment = coPayment;
            this.Deductible = deductible;
            this.AmountNotCovered = amountNotCovered;
            this.EligibleForBenefits = eligibleForBenefits;
            this.Coinsurance = coinsurance;
            this.MiscellaneousAdjustments = miscellaneousAdjustments;
            this.BenefitsPaid = benefitsPaid;
            this.PatientResponsibility = patientResponsibility;
        }

        /// <summary>
        /// Populates this <see cref="ClaimAmounts"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the ClaimAmounts data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.chargedAmount = XPathHelper.GetDecimal(navigator, "charged-amount");
            this.negotiatedAmount = XPathHelper.GetDecimal(navigator, "negotiated-amount");
            this.coPayment = XPathHelper.GetDecimal(navigator, "copay");
            this.deductible = XPathHelper.GetDecimal(navigator, "deductible");
            this.amountNotCovered = XPathHelper.GetDecimal(navigator, "amount-not-covered");
            this.eligibleForBenefits = XPathHelper.GetDecimal(navigator, "eligible-for-benefits");

            this.percentageCovered = XPathHelper.GetOptNavValueAsDouble(navigator, "percentage-covered");

            this.coinsurance = XPathHelper.GetDecimal(navigator, "coinsurance");
            this.miscellaneousAdjustments = XPathHelper.GetDecimal(navigator, "miscellaneous-adjustments");
            this.benefitsPaid = XPathHelper.GetDecimal(navigator, "benefits-paid");
            this.patientResponsibility = XPathHelper.GetDecimal(navigator, "patient-responsibility");
        }

        /// <summary>
        /// Writes the XML representation of the ClaimAmounts into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the ClaimAmounts.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the ClaimAmounts should be
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
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            XmlWriterHelper.WriteDecimal(writer, "charged-amount", this.chargedAmount);
            XmlWriterHelper.WriteDecimal(writer, "negotiated-amount", this.negotiatedAmount);
            XmlWriterHelper.WriteDecimal(writer, "copay", this.coPayment);
            XmlWriterHelper.WriteDecimal(writer, "deductible", this.deductible);
            XmlWriterHelper.WriteDecimal(writer, "amount-not-covered", this.amountNotCovered);
            XmlWriterHelper.WriteDecimal(writer, "eligible-for-benefits", this.eligibleForBenefits);

            XmlWriterHelper.WriteOptDouble(writer, "percentage-covered", this.percentageCovered);

            XmlWriterHelper.WriteDecimal(writer, "coinsurance", this.coinsurance);
            XmlWriterHelper.WriteDecimal(writer, "miscellaneous-adjustments", this.miscellaneousAdjustments);
            XmlWriterHelper.WriteDecimal(writer, "benefits-paid", this.benefitsPaid);
            XmlWriterHelper.WriteDecimal(writer, "patient-responsibility", this.patientResponsibility);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the amount charged.
        /// </summary>
        ///
        public decimal ChargedAmount
        {
            get { return this.chargedAmount; }
            set { this.chargedAmount = value; }
        }

        private decimal chargedAmount;

        /// <summary>
        /// Gets or sets the amount negotiated between the provider and the payer.
        /// </summary>
        ///
        public decimal NegotiatedAmount
        {
            get { return this.negotiatedAmount; }
            set { this.negotiatedAmount = value; }
        }

        private decimal negotiatedAmount;

        /// <summary>
        /// Gets or sets the copayment amount.
        /// </summary>
        ///
        public decimal CoPayment
        {
            get { return this.coPayment; }
            set { this.coPayment = value; }
        }

        private decimal coPayment;

        /// <summary>
        /// Gets or sets the deductible amount.
        /// </summary>
        ///
        public decimal Deductible
        {
            get { return this.deductible; }
            set { this.deductible = value; }
        }

        private decimal deductible;

        /// <summary>
        /// Gets or sets the amount for services not covered by the plan.
        /// </summary>
        ///
        public decimal AmountNotCovered
        {
            get { return this.amountNotCovered; }
            set { this.amountNotCovered = value; }
        }

        private decimal amountNotCovered;

        /// <summary>
        /// Gets or sets the amount that is eligible for benefits.
        /// </summary>
        ///
        public decimal EligibleForBenefits
        {
            get { return this.eligibleForBenefits; }
            set { this.eligibleForBenefits = value; }
        }

        private decimal eligibleForBenefits;

        /// <summary>
        /// Gets or sets the percentage of the eligible amount that is covered by the payer.
        /// </summary>
        ///
        /// <remarks>
        /// The value will be set to null if unknown.
        /// </remarks>
        ///
        public double? PercentageCovered
        {
            get { return this.percentageCovered; }
            set { this.percentageCovered = value; }
        }

        private double? percentageCovered;

        /// <summary>
        /// Gets or sets the amount paid by the person.
        /// </summary>
        ///
        public decimal Coinsurance
        {
            get { return this.coinsurance; }
            set { this.coinsurance = value; }
        }

        private decimal coinsurance;

        /// <summary>
        /// Gets or sets the adjustments that may affect the amount paid on the claim.
        /// </summary>
        ///
        public decimal MiscellaneousAdjustments
        {
            get { return this.miscellaneousAdjustments; }
            set { this.miscellaneousAdjustments = value; }
        }

        private decimal miscellaneousAdjustments;

        /// <summary>
        /// Gets or sets the amount paid by the payer.
        /// </summary>
        ///
        public decimal BenefitsPaid
        {
            get { return this.benefitsPaid; }
            set { this.benefitsPaid = value; }
        }

        private decimal benefitsPaid;

        /// <summary>
        /// Gets or sets the total amount paid by the patient.
        /// </summary>
        ///
        public decimal PatientResponsibility
        {
            get { return this.patientResponsibility; }
            set { this.patientResponsibility = value; }
        }

        private decimal patientResponsibility;

        /// <summary>
        /// Gets a string representation of the ClaimAmounts.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the ClaimAmounts.
        /// </returns>
        ///
        public override string ToString()
        {
            string value =
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ClaimAmountsToStringFormat,
                this.chargedAmount,
                this.negotiatedAmount,
                this.coPayment,
                this.deductible,
                this.amountNotCovered,
                this.eligibleForBenefits,
                this.percentageCovered != null ? this.percentageCovered.ToString() : string.Empty,
                this.coinsurance,
                this.miscellaneousAdjustments,
                this.benefitsPaid,
                this.patientResponsibility);

            return value;
        }
    }
}
