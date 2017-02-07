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

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a person's 
    /// cholesterol profile (Lipid profile) at a single point in time.
    /// </summary>
    /// 
    public class CholesterolProfile : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfile"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> 
        /// method is called.
        /// </remarks>
        /// 
        public CholesterolProfile()
            : base(TypeId)
        {
        }
 
        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfile"/> class 
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
        public CholesterolProfile(HealthServiceDate when)
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
            new Guid("796C186F-B874-471c-8468-3EEFF73BF66E");

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

            Validator.ThrowInvalidIfNull(itemNav, "CholesterolProfileUnexpectedNode");

            _when = new HealthServiceDate();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            _ldl = XPathHelper.GetOptNavValueAsInt(itemNav, "ldl");

            _hdl = XPathHelper.GetOptNavValueAsInt(itemNav, "hdl");

            _totalCholesterol = XPathHelper.GetOptNavValueAsInt(itemNav, "total-cholesterol");

            _triglyceride = XPathHelper.GetOptNavValueAsInt(itemNav, "triglyceride");
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

            XmlWriterHelper.WriteOptInt(
                writer, 
                "ldl", 
                _ldl);

            XmlWriterHelper.WriteOptInt(
                writer, 
                "hdl", 
                _hdl);

            XmlWriterHelper.WriteOptInt(
                writer, 
                "total-cholesterol", 
                _totalCholesterol);

            XmlWriterHelper.WriteOptInt(
                writer, 
                "triglyceride", 
                _triglyceride);

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
        public HealthServiceDate When
        {
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDate _when = new HealthServiceDate();

        /// <summary>
        /// Gets or sets the person's low density lipoprotein in mg/dL.
        /// </summary>
        /// 
        /// <value>
        /// The value of the person's LDL measurement or <b>null</b> if unknown.
        /// </value>
        /// 
        public int? LDL
        {
            get { return _ldl; }
            set { _ldl = value; }
        }
        private int? _ldl;


        /// <summary>
        /// Gets or sets the person's high density lipoprotein in mg/dL.
        /// </summary>
        /// 
        /// <value>
        /// The value of the person's HDL measurement or <b>null</b> if unknown.
        /// </value>
        /// 
        public int? HDL
        {
            get { return _hdl; }
            set { _hdl = value; }
        }
        private int? _hdl;

        /// <summary>
        /// Gets or sets the person's total cholesterol in mg/dL.
        /// </summary>
        /// 
        /// <value>
        /// The value of the person's total cholesterol measurement or <b>null</b> if unknown.
        /// </value>
        /// 
        public int? TotalCholesterol
        {
            get { return _totalCholesterol; }
            set { _totalCholesterol = value; }
        }
        private int? _totalCholesterol;

        /// <summary>
        /// Gets or sets the person's triglyceride in mg/dL.
        /// </summary>
        /// 
        /// <value>
        /// The value of the person's triglyceride measurement or <b>null</b> if unknown.
        /// </value>
        /// 
        public int? Triglyceride
        {
            get { return _triglyceride; }
            set { _triglyceride = value; }
        }
        private int? _triglyceride;

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
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "CholesterolProfileToStringFormatTotal"),
                        TotalCholesterol.Value);
            }
            else if (LDL != null && HDL != null)
            {
                result =
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "CholesterolProfileToStringFormatLDLAndHDL"),
                        LDL.Value,
                        HDL.Value);
            }
            else if (LDL != null)
            {
                result = 
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "CholesterolProfileToStringFormatLDL"),
                        LDL.Value);
            }
            else if (HDL != null)
            {
                result = 
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "CholesterolProfileToStringFormatHDL"),
                        HDL.Value);
            }
            else if (Triglyceride != null)
            {
                result = 
                    String.Format(
                        ResourceRetriever.GetResourceString(
                            "CholesterolProfileToStringFormatTriglyceride"),
                        Triglyceride.Value);
            }


            return result;
        }

    }

}
