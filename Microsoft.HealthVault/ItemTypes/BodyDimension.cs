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
    /// Stores a body dimension.
    /// </summary>
    ///
    /// <remarks>
    /// Examples: Waist size, head circumference, length (pediatric).
    /// </remarks>
    ///
    public class BodyDimension : ThingBase
    {
        /// <summary>
        /// Stores a body dimension.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Waist size, head circumference, length (pediatric).
        /// </remarks>
        ///
        public BodyDimension()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Stores a body dimension.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Waist size, head circumference, length (pediatric).
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time of the body dimension measurement.
        /// </param>
        ///
        /// <param name="measurementName">
        /// The name of the body dimension measurement.
        /// </param>
        ///
        /// <param name="value">
        /// The value of the body dimension measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/>, <paramref name="measurementName"/> or
        /// <paramref name="value"/> is <b>null</b>.
        /// </exception>
        ///
        public BodyDimension(
            ApproximateDateTime when,
            CodableValue measurementName,
            Length value)
            : base(TypeId)
        {
            this.When = when;
            this.MeasurementName = measurementName;
            this.Value = value;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("dd710b31-2b6f-45bd-9552-253562b9a7c1");

        /// <summary>
        /// Populates this <see cref="BodyDimension"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the body dimension data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "body-dimension" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("body-dimension");

            Validator.ThrowInvalidIfNull(itemNav, Resources.BodyDimensionUnexpectedNode);

            // when (approxi-date-time, mandatory)
            this.when = new ApproximateDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            this.measurementName = new CodableValue();
            this.measurementName.ParseXml(itemNav.SelectSingleNode("measurement-name"));

            // value (Length, mandatory)
            this.value = new Length();
            this.value.ParseXml(itemNav.SelectSingleNode("value"));
        }

        /// <summary>
        /// Writes the body dimension data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the body dimension data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/>, <see cref="MeasurementName"/> or <see cref="Value"/>
        /// is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(this.measurementName, Resources.BodyDimensionMeasurementNameNotSet);
            Validator.ThrowSerializationIfNull(this.value, Resources.BodyDimensionValueNotSet);

            // <body-dimension>
            writer.WriteStartElement("body-dimension");

            // <when>
            this.when.WriteXml("when", writer);

            // <measurement-name>
            this.measurementName.WriteXml("measurement-name", writer);

            // <value>
            this.value.WriteXml("value", writer);

            // </body-dimension>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of the body dimension measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing the date
        /// and time.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private ApproximateDateTime when;

        /// <summary>
        /// Gets or sets the name of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the name
        /// of this measurement.
        /// </value>
        ///
        /// <remarks>
        /// Choose the appropriate preferred vocabulary based on your scenario.
        /// The preferred vocabularies are "body-dimension-measurement-names"
        /// and "body-dimension-measurement-names-pediatrics".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue MeasurementName
        {
            get { return this.measurementName; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.MeasurementName), Resources.BodyDimensionMeasurementNameNullValue);
                this.measurementName = value;
            }
        }

        private CodableValue measurementName;

        /// <summary>
        /// Gets or sets the value of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Length"/> instance representing the value of
        /// this measurement.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Length Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Value), Resources.BodyDimensionValueNullValue);
                this.value = value;
            }
        }

        private Length value;

        /// <summary>
        /// Gets the representation of a body dimension instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the body dimension instance.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.measurementName != null && this.value != null)
            {
                return string.Format(
                        Resources.NameAndValue,
                        this.measurementName.ToString(),
                        this.value.ToString());
            }

            if (this.measurementName != null)
            {
                return this.measurementName.ToString();
            }

            if (this.value != null)
            {
                return this.value.ToString();
            }

            return string.Empty;
        }
    }
}
