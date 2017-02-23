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
    /// Represents a health record item type that encapsulates a person's height.
    /// </summary>
    ///
    public class Height : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Height()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified height in meters.
        /// </summary>
        ///
        /// <param name="meters">
        /// The height in meters.
        /// </param>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Height(double meters)
            : base(TypeId)
        {
            this.value.Value = meters;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified date and time of measurement and the height in meters.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the height measurement occurred.
        /// </param>
        ///
        /// <param name="height">
        /// The person's height.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="height"/> parameter is
        /// <b>null</b>.
        /// </exception>
        ///
        public Height(HealthServiceDateTime when, Length height)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(when, "when", "WhenNullValue");
            Validator.ThrowIfArgumentNull(height, "height", "HeightValueMandatory");

            this.when = when;
            this.value = height;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Height"/> class with the
        /// specified height in meters and display value.
        /// </summary>
        ///
        /// <param name="meters">
        /// The height in meters.
        /// </param>
        ///
        /// <param name="displayValue">
        /// The height value as entered by the user. Typically, the display value is
        /// used when the user-entered height units are not in meters.
        /// </param>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Height(double meters, DisplayValue displayValue)
            : base(TypeId)
        {
            this.value.Value = meters;
            this.value.DisplayValue = displayValue;
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
            new Guid("40750a6a-89b2-455c-bd8d-b420a4cb500b");

        /// <summary>
        /// Populates this <see cref="Height"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the height data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a height node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator heightNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("height");

            Validator.ThrowInvalidIfNull(heightNav, "HeightUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(heightNav.SelectSingleNode("when"));

            this.value = new Length();
            this.value.ParseXml(heightNav.SelectSingleNode("value"));
        }

        /// <summary>
        /// Writes the height data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the height data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Value"/> has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.value, "HeightValueNotSet");

            // <height>
            writer.WriteStartElement("height");

            // <when>
            this.when.WriteXml("when", writer);

            this.value.WriteXml("value", writer);

            // </height>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the height measurement
        /// was taken.
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
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's height.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> value representing the person's height.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public Length Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Value", "HeightValueMandatory");
                this.value = value;
            }
        }

        private Length value = new Length();

        /// <summary>
        /// Gets a string representation of the height item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the height item.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
