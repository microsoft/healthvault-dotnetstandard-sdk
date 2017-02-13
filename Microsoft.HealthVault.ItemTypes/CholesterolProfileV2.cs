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
            When = when;
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

            _when = new HealthServiceDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _ldl = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "ldl");

            _hdl = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "hdl");

            _totalCholesterol = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "total-cholesterol");

            _triglyceride = XPathHelper.GetOptNavValue<ConcentrationMeasurement>(itemNav, "triglyceride");
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
            _when.WriteXml("when", writer);

            XmlWriterHelper.WriteOpt<ConcentrationMeasurement>(writer, "ldl", _ldl);

            XmlWriterHelper.WriteOpt<ConcentrationMeasurement>(writer, "hdl", _hdl);

            XmlWriterHelper.WriteOpt<ConcentrationMeasurement>(writer, "total-cholesterol", _totalCholesterol);

            XmlWriterHelper.WriteOpt<ConcentrationMeasurement>(writer, "triglyceride", _triglyceride);

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
                return _when; 
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }

        private HealthServiceDateTime _when = new HealthServiceDateTime();

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
            get { return _ldl; }
            set { _ldl = value; }
        }

        private ConcentrationMeasurement _ldl;

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
            get { return _hdl; }
            set { _hdl = value; }
        }

        private ConcentrationMeasurement _hdl;

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
            get { return _totalCholesterol; }
            set { _totalCholesterol = value; }
        }

        private ConcentrationMeasurement _totalCholesterol;

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
            get { return _triglyceride; }
            set { _triglyceride = value; }
        }

        private ConcentrationMeasurement _triglyceride;

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
            string result = String.Empty;

            if (TotalCholesterol != null)
            {
                result = TotalCholesterol.ToString();
            }
            else if (LDL != null && HDL != null)
            {
                result = String.Format(
                    ResourceRetriever.GetResourceString("CholesterolProfileV2ToStringFormatLDLAndHDL"), 
                    LDL.ToString(),
                    HDL.ToString());
            }
            else if (LDL != null)
            {
                result = LDL.ToString();
            }
            else if (HDL != null)
            {
                result = HDL.ToString();
            }
            else if (Triglyceride != null)
            {
                result = Triglyceride.ToString();
            }

            return result;
        }
    }
}