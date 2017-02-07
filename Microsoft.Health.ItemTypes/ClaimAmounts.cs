// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// The financial information related to a claim on an explanation of benefits.
    /// </summary>
    public class ClaimAmounts : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ClaimAmounts"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called
        /// </remarks>
        public ClaimAmounts()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="ClaimAmounts"/> class specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called
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
            ChargedAmount = chargedAmount;
            NegotiatedAmount = negotiatedAmount;
            CoPayment = coPayment;
            Deductible = deductible;
            AmountNotCovered = amountNotCovered;
            EligibleForBenefits = eligibleForBenefits;
            Coinsurance = coinsurance;
            MiscellaneousAdjustments = miscellaneousAdjustments;
            BenefitsPaid = benefitsPaid;
            PatientResponsibility = patientResponsibility;
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

            _chargedAmount = XPathHelper.GetDecimal(navigator, "charged-amount");
            _negotiatedAmount = XPathHelper.GetDecimal(navigator, "negotiated-amount");
            _coPayment = XPathHelper.GetDecimal(navigator, "copay");
            _deductible = XPathHelper.GetDecimal(navigator, "deductible");
            _amountNotCovered = XPathHelper.GetDecimal(navigator, "amount-not-covered");
            _eligibleForBenefits = XPathHelper.GetDecimal(navigator, "eligible-for-benefits");

            _percentageCovered = XPathHelper.GetOptNavValueAsDouble(navigator, "percentage-covered");

            _coinsurance = XPathHelper.GetDecimal(navigator, "coinsurance");
            _miscellaneousAdjustments = XPathHelper.GetDecimal(navigator, "miscellaneous-adjustments");
            _benefitsPaid = XPathHelper.GetDecimal(navigator, "benefits-paid");
            _patientResponsibility = XPathHelper.GetDecimal(navigator, "patient-responsibility");
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

            XmlWriterHelper.WriteDecimal(writer, "charged-amount", _chargedAmount);
            XmlWriterHelper.WriteDecimal(writer, "negotiated-amount", _negotiatedAmount);
            XmlWriterHelper.WriteDecimal(writer, "copay", _coPayment);
            XmlWriterHelper.WriteDecimal(writer, "deductible", _deductible);
            XmlWriterHelper.WriteDecimal(writer, "amount-not-covered", _amountNotCovered);
            XmlWriterHelper.WriteDecimal(writer, "eligible-for-benefits", _eligibleForBenefits);

            XmlWriterHelper.WriteOptDouble(writer, "percentage-covered", _percentageCovered);

            XmlWriterHelper.WriteDecimal(writer, "coinsurance", _coinsurance);
            XmlWriterHelper.WriteDecimal(writer, "miscellaneous-adjustments", _miscellaneousAdjustments);
            XmlWriterHelper.WriteDecimal(writer, "benefits-paid", _benefitsPaid);
            XmlWriterHelper.WriteDecimal(writer, "patient-responsibility", _patientResponsibility);

            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets the amount charged.
        /// </summary>
        ///
        public decimal ChargedAmount
        {
            get { return _chargedAmount; }
            set { _chargedAmount = value; }
        }
            
        private decimal _chargedAmount;
                
        /// <summary>
        /// Gets or sets the amount negotiated between the provider and the payer.
        /// </summary>
        ///
        public decimal NegotiatedAmount
        {
            get { return _negotiatedAmount; }
            set { _negotiatedAmount = value; }
        }
            
        private decimal _negotiatedAmount;
                
        /// <summary>
        /// Gets or sets the copayment amount.
        /// </summary>
        ///
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly")]
        public decimal CoPayment
        {
            get { return _coPayment; }
            set { _coPayment = value; }
        }

        private decimal _coPayment;
                
        /// <summary>
        /// Gets or sets the deductible amount.
        /// </summary>
        ///
        public decimal Deductible
        {
            get { return _deductible; }
            set { _deductible = value; }
        }

        private decimal _deductible;
                
        /// <summary>
        /// Gets or sets the amount for services not covered by the plan.
        /// </summary>
        ///
        public decimal AmountNotCovered
        {
            get { return _amountNotCovered; }
            set { _amountNotCovered = value; }
        }
            
        private decimal _amountNotCovered;
                
        /// <summary>
        /// Gets or sets the amount that is eligible for benefits.
        /// </summary>
        ///
        public decimal EligibleForBenefits
        {
            get { return _eligibleForBenefits; }
            set { _eligibleForBenefits = value; }
        }
            
        private decimal _eligibleForBenefits;
                
        /// <summary>
        /// Gets or sets the percentage of the eligable amount that is covered by the payer.
        /// </summary>
        /// 
        /// <remarks>
        /// The value will be set to null if unknown.
        /// </remarks>
        ///
        public double? PercentageCovered
        {
            get { return _percentageCovered; }
            set { _percentageCovered = value; }
        }
            
        private double? _percentageCovered;
                
        /// <summary>
        /// Gets or sets the amount paid by the person.
        /// </summary>
        ///
        public decimal Coinsurance
        {
            get { return _coinsurance; }
            set { _coinsurance = value; }
        }
            
        private decimal _coinsurance;
                
        /// <summary>
        /// Gets or sets the adjustments that may affect the amount paid on the claim.
        /// </summary>
        ///
        public decimal MiscellaneousAdjustments
        {
            get { return _miscellaneousAdjustments; }
            set { _miscellaneousAdjustments = value; }
        }

        private decimal _miscellaneousAdjustments;
                
        /// <summary>
        /// Gets or sets the amount paid by the payer.
        /// </summary>
        ///
        public decimal BenefitsPaid
        {
            get { return _benefitsPaid; }
            set { _benefitsPaid = value; }
        }
            
        private decimal _benefitsPaid;
                
        /// <summary>
        /// Gets or sets the total amount paid by the patient.
        /// </summary>
        ///
        public decimal PatientResponsibility
        {
            get { return _patientResponsibility; }
            set { _patientResponsibility = value; }
        }

        private decimal _patientResponsibility;

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
                String.Format(
                    CultureInfo.CurrentCulture,
                    ResourceRetriever.GetResourceString("ClaimAmountsToStringFormat"),
                _chargedAmount,
                _negotiatedAmount,
                _coPayment,
                _deductible,
                _amountNotCovered,
                _eligibleForBenefits,
                (_percentageCovered != null) ? _percentageCovered.ToString() : String.Empty,
                _coinsurance,
                _miscellaneousAdjustments,
                _benefitsPaid,
                _patientResponsibility);

            return value;
        }
    }
}
    