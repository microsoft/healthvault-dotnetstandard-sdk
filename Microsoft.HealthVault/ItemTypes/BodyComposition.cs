// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information about the body composition of the record owner.
    /// </summary>
    ///
    public class BodyComposition : ThingBase
    {
        /// <summary>
        /// Creates an instance of information about the body composition of the record owner
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: % body fat, lean muscle mass.
        /// </remarks>
        ///
        public BodyComposition()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates an instance of information about the body composition of the record owner
        /// with specified time, measurement name, and value.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: % body fat, lean muscle mass.
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time of the measurement.
        /// </param>
        ///
        /// <param name="measurementName">
        /// The name of the measurement.
        /// </param>
        ///
        /// <param name="compositionValue">
        /// The value of the measurement.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/>, <paramref name="measurementName"/> or
        /// <paramref name="compositionValue"/> is <b>null</b>.
        /// </exception>
        ///
        public BodyComposition(
            ApproximateDateTime when,
            CodableValue measurementName,
            BodyCompositionValue compositionValue)
            : base(TypeId)
        {
            this.When = when;
            this.MeasurementName = measurementName;
            this.Value = compositionValue;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("18adc276-5144-4e7e-bf6c-e56d8250adf8");

        /// <summary>
        /// Populates this <see cref="BodyComposition"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the body composition data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "body-composition" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("body-composition");

            Validator.ThrowInvalidIfNull(itemNav, Resources.BodyCompositionUnexpectedNode);

            // when (approxi-date-time, mandatory)
            this.when = new ApproximateDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            this.measurementName = new CodableValue();
            this.measurementName.ParseXml(itemNav.SelectSingleNode("measurement-name"));

            // value (BodyCompositionValue, mandatory)
            this.value = new BodyCompositionValue();
            this.value.ParseXml(itemNav.SelectSingleNode("value"));

            // measurement-method (codable value )
            this.measurementMethod =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "measurement-method");

            // site
            this.site =
                XPathHelper.GetOptNavValue<CodableValue>(itemNav, "site");
        }

        /// <summary>
        /// Writes the body composition data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the body composition data to.
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
            Validator.ThrowSerializationIfNull(this.measurementName, Resources.BodyCompositionMeasurementNameNotSet);
            Validator.ThrowSerializationIfNull(this.value, Resources.BodyCompositionValueNotSet);

            // <body-composition>
            writer.WriteStartElement("body-composition");

            // <when>
            this.when.WriteXml("when", writer);

            // <measurement-name>
            this.measurementName.WriteXml("measurement-name", writer);

            // <value>
            this.value.WriteXml("value", writer);

            // <measurement-method>
            XmlWriterHelper.WriteOpt(
                writer,
                "measurement-method",
                this.measurementMethod);

            // <site>
            XmlWriterHelper.WriteOpt(
                writer,
                "site",
                this.site);

            // </body-composition>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time of this measurement.
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
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.WhenNullValue);
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
        /// Examples: Body fat, lean muscle. The preferred vocabulary is
        /// "body-composition-measurement-names".
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
                Validator.ThrowIfArgumentNull(value, nameof(MeasurementName), Resources.BodyCompositionMeasurementNameNullValue);
                this.measurementName = value;
            }
        }

        private CodableValue measurementName;

        /// <summary>
        /// Gets or sets the value of this measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="BodyCompositionValue"/> instance representing the value of
        /// this measurement.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public BodyCompositionValue Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Value), Resources.BodyCompositionMeasurementNameNullValue);
                this.value = value;
            }
        }

        private BodyCompositionValue value;

        /// <summary>
        /// Gets or sets the technique used to obtain the measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the technique used
        /// to obtain the measurement.
        /// </value>
        ///
        /// <remarks>
        /// Examples: Bioelectrical impedance, DXA, Skinfold (calipers). If the measurement method
        /// is missing, the value should be set to <b>null</b>. The preferred vocabulary is
        /// "body-composition-measurement-methods".
        /// </remarks>
        ///
        public CodableValue MeasurementMethod
        {
            get { return this.measurementMethod; }
            set { this.measurementMethod = value; }
        }

        private CodableValue measurementMethod;

        /// <summary>
        /// Gets or sets the body part that is the subject of the measurement.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the site
        /// </value>
        ///
        /// <remarks>
        /// Examples: Left arm, head, torso. If the site is absent, the measurement is for
        /// the whole body. The preferred vocabulary is "body-composition-sites".
        /// </remarks>
        ///
        public CodableValue Site
        {
            get { return this.site; }
            set { this.site = value; }
        }

        private CodableValue site;

        /// <summary>
        /// Gets the representation of a body composition instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the body composition item.
        /// </returns>
        ///
        public override string ToString()
        {
            List<string> elements = new List<string>();

            if (this.measurementName != null)
            {
                elements.Add(this.measurementName.ToString());
            }

            string valueString = this.value?.ToString();
            if (!string.IsNullOrEmpty(valueString))
            {
                elements.Add(valueString);
            }

            if (this.measurementMethod != null)
            {
                elements.Add(this.measurementMethod.ToString());
            }

            string separator =
                Resources.ListSeparator;

            return string.Join(separator, elements.ToArray());
        }
    }
}
