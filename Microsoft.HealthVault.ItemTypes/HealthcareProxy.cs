// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a healthcare proxy.
    /// </summary>
    ///
    public class HealthcareProxy : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthcareProxy"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item isn't added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public HealthcareProxy()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthcareProxy"/> class
        /// with the specified date and time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time for the healthcare proxy.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public HealthcareProxy(HealthServiceDateTime when)
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
            new Guid("7EA47715-CBA4-47F0-99D2-EB0A9FB4A85C");

        /// <summary>
        /// Populates this healthcare proxy instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the healthcare proxy data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a healthcare proxy node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("healthcare-proxy");

            Validator.ThrowInvalidIfNull(itemNav, "HealthcareProxyUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // <proxy>
            this.proxy =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "proxy");

            // <alternate>
            this.alternate =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "alternate");

            // <primary-witness>
            this.primaryWitness =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "primary-witness");

            // <secondary-witness>
            this.secondaryWitness =
                XPathHelper.GetOptNavValue<PersonItem>(
                    itemNav,
                    "secondary-witness");

            // <content>
            this.content =
                XPathHelper.GetOptNavValue(itemNav, "content");
        }

        /// <summary>
        /// Writes the healthcare proxy data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the healthcare proxy data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="When"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, "HealthcareProxyWhenNotSet");

            // <healthcare-proxy>
            writer.WriteStartElement("healthcare-proxy");

            // <when>
            this.when.WriteXml("when", writer);

            // <proxy>
            XmlWriterHelper.WriteOpt(
                writer,
                "proxy",
                this.proxy);

            // <alternate>
            XmlWriterHelper.WriteOpt(
                writer,
                "alternate",
                this.alternate);

            // <primary-witness>
            XmlWriterHelper.WriteOpt(
                writer,
                "primary-witness",
                this.primaryWitness);

            // <secondary-witness>
            XmlWriterHelper.WriteOpt(
                writer,
                "secondary-witness",
                this.secondaryWitness);

            // <content>
            XmlWriterHelper.WriteOptString(
                writer,
                "content",
                this.content);

            // </healthcare-proxy>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time for the medical equipment.
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
        /// Gets or sets the proxy contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the proxy contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Proxy
        {
            get { return this.proxy; }
            set { this.proxy = value; }
        }

        private PersonItem proxy;

        /// <summary>
        /// Gets or sets the alternate contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the alternate contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem Alternate
        {
            get { return this.alternate; }
            set { this.alternate = value; }
        }

        private PersonItem alternate;

        /// <summary>
        /// Gets or sets the primary witness contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the primary witness contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem PrimaryWitness
        {
            get { return this.primaryWitness; }
            set { this.primaryWitness = value; }
        }

        private PersonItem primaryWitness;

        /// <summary>
        /// Gets or sets the secondary witness contact information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="PersonItem"/> representing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the secondary witness contact information
        /// should not be stored.
        /// </remarks>
        ///
        public PersonItem SecondaryWitness
        {
            get { return this.secondaryWitness; }
            set { this.secondaryWitness = value; }
        }

        private PersonItem secondaryWitness;

        /// <summary>
        /// Gets or sets the content of what is being proxied.
        /// </summary>
        ///
        /// <value>
        /// A string representing the content.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the content should not be
        /// stored.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Content
        {
            get { return this.content; }

            set
            {
                Validator.ThrowIfStringIsWhitespace(value, "Content");
                this.content = value;
            }
        }

        private string content;

        /// <summary>
        /// Gets a string representation of the healthcare proxy item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the healthcare proxy item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(200);

            result.Append(this.When);

            if (this.Proxy != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Proxy.ToString());
            }

            if (this.Content != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "ListFormat"),
                    this.Content);
            }

            return result.ToString();
        }
    }
}
