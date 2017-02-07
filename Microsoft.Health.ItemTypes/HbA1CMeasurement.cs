// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents the result of an HBA1C assay in mmol/mol.
    /// </summary>
    /// 
    /// <remarks>
    /// Represents HBA1C results using the International Federation of Clinical Chemistry and
    /// Laboratory Medicine (IFCC) standard units of millimoles per mole of unglycated
    /// hemoglobin in the blood.
    /// </remarks>
    /// 
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1709:IdentifiersShouldBeCasedCorrectly",
        Justification = "Hb is the correct capitalization here.")]
    public class HbA1CMeasurement : Measurement<double>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CMeasurement"/> 
        /// class with empty values.
        /// </summary>
        /// 
        public HbA1CMeasurement()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CMeasurement"/> 
        /// class with the specified value in millimoles per mole (mmol/mol).
        /// </summary>
        /// 
        /// <param name="millimolesPerMole">
        /// The concentration of unglycated hemoglobin in the blood in millimoles per mole.
        /// </param>
        /// 
        public HbA1CMeasurement(double millimolesPerMole)
            : base(millimolesPerMole)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HbA1CMeasurement"/> 
        /// class with the specified value in millimoles per mole (mmol/mol) 
        /// and display value.
        /// </summary>
        /// 
        /// <param name="millimolesPerMole">
        /// The concentration value in millimoles per mole.
        /// </param>
        /// 
        /// <param name="displayValue">
        /// The display value of the HbA1C measurement. This should 
        /// contain the exact measurement as entered by the user, even if it 
        /// uses some other unit of measure besides mmol/mol. The display value
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.Units"/> and 
        /// <see cref="Microsoft.Health.ItemTypes.DisplayValue.UnitsCode"/> 
        /// represents the unit of measure for the user-entered value.
        /// </param>
        /// 
        public HbA1CMeasurement(
            double millimolesPerMole,
            DisplayValue displayValue)
            : base(millimolesPerMole, displayValue)
        {
        }

        /// <summary>
        /// Verifies that the value is a legal HbA1C value.
        /// </summary>
        /// 
        /// <param name="value">
        /// The HbA1C measurement.
        /// </param>
        /// 
        protected override void AssertMeasurementValue(double value)
        {
        }

        /// <summary> 
        /// Populates the data for the HbA1C value from the XML.
        /// </summary>
        /// 
        /// <param name="navigator"> 
        /// The XML node representing the HbA1C value.
        /// </param>
        /// 
        protected override void ParseValueXml(XPathNavigator navigator)
        {
            Value = navigator.SelectSingleNode("mmol-per-mol").ValueAsDouble;
        }

        /// <summary> 
        /// Writes the HbA1C value to the specified XML writer.
        /// </summary>
        /// 
        /// <param name="writer"> 
        /// The XmlWriter to write the HbA1C value to.
        /// </param>
        /// 
        protected override void WriteValueXml(XmlWriter writer)
        {
            writer.WriteElementString(
                "mmol-per-mol", XmlConvert.ToString(Value));
        }

        /// <summary>
        /// Gets a string representation of the HbA1C value in the base units.
        /// </summary>
        /// <returns>
        /// The HbA1C value as a string in the base units.
        /// </returns>
        /// 
        protected override string GetValueString(double value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }
    }
}