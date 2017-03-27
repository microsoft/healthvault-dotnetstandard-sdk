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
    /// Represents a thing type that encapsulates a person's weight.
    /// </summary>
    ///
    public class Weight : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Weight"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Weight()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Weight"/> class with the
        /// specified date/time and weight.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the weight measurement occurred.
        /// </param>
        ///
        /// <param name="weight">
        /// The person's weight.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="weight"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Weight(HealthServiceDateTime when, WeightValue weight)
            : base(TypeId)
        {
            this.When = when;
            this.Value = weight;
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
            new Guid("3d34d87e-7fc1-4153-800f-f56592cb0d17");

        /// <summary>
        /// Populates this <see cref="Weight"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the weight data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a weight node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator weightNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("weight");

            Validator.ThrowInvalidIfNull(weightNav, Resources.WeightUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(weightNav.SelectSingleNode("when"));

            this.value = new WeightValue();
            this.value.ParseXml(weightNav.SelectSingleNode("value"));
        }

        /// <summary>
        /// Writes the weight data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the weight data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Value"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.value, Resources.WeightValueNotSet);

            // <weight>
            writer.WriteStartElement("weight");

            // <when>
            this.when.WriteXml("when", writer);

            this.value.WriteXml("value", writer);

            // </weight>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the object.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="Value"/> if set; otherwise, or String.Empty.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Value != null)
            {
                result = this.Value.ToString();
            }

            return result;
        }

        /// <summary>
        /// Gets or sets the date/time when the weight measurement
        /// was taken.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> for the weight measurement.
        /// The default value is the current year, month, and day.
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
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the person's weight.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="WeightValue"/>.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> during set.
        /// </exception>
        ///
        public WeightValue Value
        {
            get { return this.value; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Value), Resources.WeightValueMandatory);
                this.value = value;
            }
        }

        private WeightValue value = new WeightValue();
    }
}
