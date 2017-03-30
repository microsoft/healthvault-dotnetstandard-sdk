// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's
    /// cholesterol profile (Lipid profile) at a single point in time.
    /// </summary>
    ///
    public class CholesterolProfile : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CholesterolProfile"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(itemNav, Resources.CholesterolProfileUnexpectedNode);

            this.when = new HealthServiceDate();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            this.ldl = XPathHelper.GetOptNavValueAsInt(itemNav, "ldl");

            this.hdl = XPathHelper.GetOptNavValueAsInt(itemNav, "hdl");

            this.totalCholesterol = XPathHelper.GetOptNavValueAsInt(itemNav, "total-cholesterol");

            this.triglyceride = XPathHelper.GetOptNavValueAsInt(itemNav, "triglyceride");
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

            XmlWriterHelper.WriteOptInt(
                writer,
                "ldl",
                this.ldl);

            XmlWriterHelper.WriteOptInt(
                writer,
                "hdl",
                this.hdl);

            XmlWriterHelper.WriteOptInt(
                writer,
                "total-cholesterol",
                this.totalCholesterol);

            XmlWriterHelper.WriteOptInt(
                writer,
                "triglyceride",
                this.triglyceride);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDate when = new HealthServiceDate();

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
            get { return this.ldl; }
            set { this.ldl = value; }
        }

        private int? ldl;

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
            get { return this.hdl; }
            set { this.hdl = value; }
        }

        private int? hdl;

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
            get { return this.totalCholesterol; }
            set { this.totalCholesterol = value; }
        }

        private int? totalCholesterol;

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
            get { return this.triglyceride; }
            set { this.triglyceride = value; }
        }

        private int? triglyceride;

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
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatTotal,
                        this.TotalCholesterol.Value);
            }
            else if (this.LDL != null && this.HDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatLDLAndHDL,
                        this.LDL.Value,
                        this.HDL.Value);
            }
            else if (this.LDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatLDL,
                        this.LDL.Value);
            }
            else if (this.HDL != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatHDL,
                        this.HDL.Value);
            }
            else if (this.Triglyceride != null)
            {
                result =
                    string.Format(
                        Resources.CholesterolProfileToStringFormatTriglyceride,
                        this.Triglyceride.Value);
            }

            return result;
        }
    }
}
