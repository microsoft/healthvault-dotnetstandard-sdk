// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a single use of
    /// an asthma inhaler.
    /// </summary>
    ///
    /// <remarks>
    /// A single use can consist of multiple doses if the prescription
    /// calls for it.
    /// </remarks>
    ///
    public class AsthmaInhalerUse : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AsthmaInhalerUse"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public AsthmaInhalerUse()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AsthmaInhalerUse"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the inhaler was used.
        /// </param>
        ///
        /// <param name="drug">
        /// The name of the drug being used in the inhaler.
        /// </param>
        ///
        /// <param name="doseCount">
        /// The count of doses for each inhaler use.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="drug"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public AsthmaInhalerUse(HealthServiceDateTime when, CodableValue drug, int doseCount)
            : base(TypeId)
        {
            this.When = when;
            this.Drug = drug;
            this.DoseCount = doseCount;
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
            new Guid("03efe378-976a-42f8-ae1e-507c497a8c6d");

        /// <summary>
        /// Populates this <see cref="AsthmaInhalerUse"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the asthma inhaler use data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an asthma-inhaler-use node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator inhalerUseNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("asthma-inhaler-use");

            Validator.ThrowInvalidIfNull(inhalerUseNav, Resources.AsthmaInhalerUseUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(inhalerUseNav.SelectSingleNode("when"));

            this.drug = new CodableValue();
            this.drug.ParseXml(inhalerUseNav.SelectSingleNode("drug"));

            XPathNavigator strengthNav =
                inhalerUseNav.SelectSingleNode("strength");

            if (strengthNav != null)
            {
                this.strength = new CodableValue();
                this.strength.ParseXml(strengthNav);
            }

            this.doseCount = inhalerUseNav.SelectSingleNode("dose-count").ValueAsInt;

            XPathNavigator deviceIdNav = inhalerUseNav.SelectSingleNode("device-id");

            if (deviceIdNav != null)
            {
                this.deviceId = deviceIdNav.Value;
            }

            XPathNavigator purposeNav = inhalerUseNav.SelectSingleNode("dose-purpose");
            if (purposeNav != null)
            {
                this.dosePurpose = new CodableValue();
                this.dosePurpose.ParseXml(purposeNav);
            }
        }

        /// <summary>
        /// Writes the asthma inhaler use data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the asthma inhaler use data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/>, <see cref="DoseCount"/>, or
        /// <see cref="Drug"/> property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.AsthmaInhalerUseWhenNotSet);
            Validator.ThrowSerializationIfNull(this.drug, Resources.AsthmaInhalerDrugNotSet);
            Validator.ThrowSerializationIfNull(this.doseCount, Resources.AsthmaInhalerDoseCountNotSet);

            // <asthma-inhaler-use>
            writer.WriteStartElement("asthma-inhaler-use");

            // <when>
            this.when.WriteXml("when", writer);

            // <drug>
            this.drug.WriteXml("drug", writer);

            // <strength>
            this.strength?.WriteXml("strength", writer);

            // <dose-count>
            writer.WriteElementString(
                "dose-count",
                ((int)this.doseCount).ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrEmpty(this.deviceId))
            {
                // <device-id>
                writer.WriteElementString("device-id", this.deviceId);
            }

            // <dose-purpose>
            this.DosePurpose?.WriteXml("dose-purpose", writer);

            // </asthma-inhaler-use>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date when the inhaler use occurred.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date. The default value is the current year, month, and day.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthServiceDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the drug being used in the inhaler.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the
        /// drug.
        /// </value>
        ///
        /// <remarks>
        /// The name of the drug in the canister.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Drug
        {
            get { return this.drug; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Drug), Resources.AsthmaInhalerUseDrugMandatory);
                this.drug = value;
            }
        }

        private CodableValue drug;

        /// <summary>
        /// Gets or sets the strength of the dosage for each inhaler use.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the
        /// dosage strength.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the strength should not be stored.
        /// </remarks>
        ///
        public CodableValue Strength
        {
            get { return this.strength; }
            set { this.strength = value; }
        }

        private CodableValue strength;

        /// <summary>
        /// Gets or sets the count of doses for each inhaler use.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the count.
        /// </value>
        ///
        public int DoseCount
        {
            get
            {
                return this.doseCount ?? 0;
            }

            set { this.doseCount = value; }
        }

        private int? doseCount;

        /// <summary>
        /// Gets or sets the identifier for the device used.
        /// </summary>
        ///
        /// <value>
        /// A string representing the device identifier.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the device identifier should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string DeviceId
        {
            get { return this.deviceId; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "DeviceId");
                this.deviceId = value;
            }
        }

        private string deviceId;

        /// <summary>
        /// Gets or sets the target of the inhaler usage.
        /// </summary>
        /// A <see cref="CodableValue"/> instance representing the
        /// dosage purpose.
        /// <remarks>
        /// Examples include relief (the dose purpose is
        /// intended to relieve immediate asthma symptoms),
        /// prevention (the dose purpose is intended to
        /// prevent asthma symptoms), control (the dose
        /// purpose is intended to control the impact of
        /// current asthma symptoms), other (the dose
        /// purpose is known, but other than to relieve,
        /// prevent or control asthma symptoms), and
        /// undefined (there is not enough information
        /// about the inhaler usage to discern purpose).
        /// <br/><br/>
        /// Codes for this value are in the "dose-purpose"
        /// vocabulary.
        /// </remarks>
        ///
        public CodableValue DosePurpose
        {
            get { return this.dosePurpose; }
            set { this.dosePurpose = value; }
        }

        private CodableValue dosePurpose;

        /// <summary>
        /// Gets a string representation of the asthma inhaler use item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the asthma inhaler use item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.Drug != null)
            {
                return string.Format(
                        Resources.AsthmaInhalerUseToStringFormatWithDrug,
                        this.Drug.Text,
                        this.DoseCount);
            }

            return string.Format(
                        Resources.AsthmaInhalerUseToStringFormat,
                        this.DoseCount);
        }
    }
}
