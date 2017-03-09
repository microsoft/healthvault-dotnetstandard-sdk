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
    /// Represents a health record item type that encapsulates a person's
    /// cholesterol profile (Lipid profile) at a single point in time.
    /// </summary>
    ///
    public class CholesterolProfileV2 : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfileV2"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/>
        /// method is called.
        /// </remarks>
        ///
        public CholesterolProfileV2()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfileV2"/> class
        /// with the specified date.
        /// </summary>
        ///
        /// <param name="when">
        /// The date when the cholesterol profile was take.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CholesterolProfileV2(HealthServiceDateTime when)
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
            new Guid("98F76958-E34F-459B-A760-83C1699ADD38");

        /// <summary>
        /// Populates this <see cref="CholesterolProfile"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the cholesterol profile data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a cholesterol-profile node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "cholesterol-profile");

            Validator.ThrowInvalidIfNull(itemNav, "CholesterolProfileV2UnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.ldl = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "ldl");

            this.hdl = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "hdl");

            this.totalCholesterol = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "total-cholesterol");

            this.triglyceride = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "triglyceride");
        }

        /// <summary>
        /// Writes the cholesterol profile data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the cholesterol profile data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <cholesterol-profile>
            writer.WriteStartElement("cholesterol-profile");

            // <when>
            this.when.WriteXml("when", writer);

            XmlWriterHelper.WriteOpt(writer, "ldl", this.ldl);

            XmlWriterHelper.WriteOpt(writer, "hdl", this.hdl);

            XmlWriterHelper.WriteOpt(writer, "total-cholesterol", this.totalCholesterol);

            XmlWriterHelper.WriteOpt(writer, "triglyceride", this.triglyceride);

            // </cholesterol-profile>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the cholesterol profile was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> representing the date.
        /// The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return this.when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's low density lipoprotein.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's LDL measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public ConcentrationMeasurement LDL
        {
            get { return this.ldl; }
            set { this.ldl = value; }
        }

        private ConcentrationMeasurement ldl;

        /// <summary>
        /// Gets or sets the person's high density lipoprotein.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's HDL measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public ConcentrationMeasurement HDL
        {
            get { return this.hdl; }
            set { this.hdl = value; }
        }

        private ConcentrationMeasurement hdl;

        /// <summary>
        /// Gets or sets the person's total cholesterol.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's total cholesterol measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public ConcentrationMeasurement TotalCholesterol
        {
            get { return this.totalCholesterol; }
            set { this.totalCholesterol = value; }
        }

        private ConcentrationMeasurement totalCholesterol;

        /// <summary>
        /// Gets or sets the person's triglyceride.
        /// </summary>
        ///
        /// <value>
        /// The value of the person's triglyceride measurement or <b>null</b> if unknown.
        /// </value>
        ///
        public ConcentrationMeasurement Triglyceride
        {
            get { return this.triglyceride; }
            set { this.triglyceride = value; }
        }

        private ConcentrationMeasurement triglyceride;

        /// <summary>
        /// Gets a string representation of the cholesterol profile.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the cholesterol profile.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.TotalCholesterol != null)
            {
                result = this.TotalCholesterol.ToString();
            }
            else if (this.LDL != null && this.HDL != null)
            {
                result = string.Format(
                    ResourceRetriever.GetResourceString("CholesterolProfileV2ToStringFormatLDLAndHDL"),
                    this.LDL.ToString(),
                    this.HDL.ToString());
            }
            else if (this.LDL != null)
            {
                result = this.LDL.ToString();
            }
            else if (this.HDL != null)
            {
                result = this.HDL.ToString();
            }
            else if (this.Triglyceride != null)
            {
                result = this.Triglyceride.ToString();
            }

            return result;
        }
    }
}