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
    /// A single assessment of menstrual flow.
    /// </summary>
    public class Menstruation : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Menstruation"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Menstruation()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Menstruation"/> class.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time of the menstrual flow.
        /// </param>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public Menstruation(HealthServiceDateTime when)
            : base(TypeId)
        {
            Validator.ThrowIfArgumentNull(when, "When", "WhenNullValue");
            this.when = when;
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
            new Guid("caff3ff3-812f-44b1-9c9f-c1af13167705");

        /// <summary>
        /// Gets or sets the date/time of the menstrual flow.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance.
        /// The value defaults to the current year, month, and day.
        /// </value>
        ///
        /// <remarks>
        /// Menstrual flow is generally recorded once per day.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
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
        /// Gets or sets the amount of discharged fluid (e.g., light, medium, heavy or spotting).
        /// </summary>
        /// <remarks>
        /// The preferred vocabulary for route is "menstrual-flow-amount".
        /// </remarks>
        public CodableValue Amount
        {
            get { return this.amount; }
            set { this.amount = value; }
        }

        private CodableValue amount;

        /// <summary>
        /// Gets or sets the bool which indicates whether this instance represents the start of
        /// a new menstrual cycle, e.g., the first day of a period.
        /// </summary>
        public bool? IsNewCycle
        {
            get { return this.isNewCycle; }
            set { this.isNewCycle = value; }
        }

        private bool? isNewCycle;

        /// <summary>
        /// Populates this <see cref="Menstruation"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the menstrual flow data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// an "menstrual-flow" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("menstruation");

            Validator.ThrowInvalidIfNull(itemNav, "MenstruationUnexpectedNode");

            // when
            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // isNewCycle
            this.isNewCycle = XPathHelper.GetOptNavValueAsBool(itemNav, "is-new-cycle");

            // amount
            this.amount = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "amount");
        }

        /// <summary>
        /// Writes the mensrual flow data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the menstrual flow data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, "WhenNullValue");

            // <menstrual-flow>
            writer.WriteStartElement("menstruation");

            // <when>
            this.when.WriteXml("when", writer);

            // <is-new-cycle>
            XmlWriterHelper.WriteOptBool(writer, "is-new-cycle", this.isNewCycle);

            // <amount>
            XmlWriterHelper.WriteOpt(writer, "amount", this.amount);

            // </menstrual-flow>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets a string representation of the Menstruation item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Menstruation item.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.amount == null)
            {
                return null;
            }

            return this.amount.Text;
        }
    }
}