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
    /// Represents a thing type that encapsulates a single
    /// allergic episode.
    /// </summary>
    ///
    /// <remarks>
    /// An allergic episode is an occurrence of an allergy which is defined by
    /// the <see cref="Allergy"/> type.
    /// symptoms.
    /// </remarks>
    ///
    public class AllergicEpisode : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AllergicEpisode"/> class
        /// with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public AllergicEpisode()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AllergicEpisode"/> class
        /// with the specified date and name.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the allergic episode occurred.
        /// </param>
        ///
        /// <param name="name">
        /// The name of the allergy.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> or <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public AllergicEpisode(HealthServiceDateTime when, CodableValue name)
            : base(TypeId)
        {
            this.When = when;
            this.Name = name;
        }

        /// <summary>
        /// The unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("d65ad514-c492-4b59-bd05-f3f6cb43ceb3");

        /// <summary>
        /// Populates this <see cref="AllergicEpisode"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the allergic episode data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an allergic-episode node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            this.name.Clear();
            XPathNavigator allergyNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("allergic-episode");

            Validator.ThrowInvalidIfNull(allergyNav, "AllergicEpisodeUnexpectedNode");

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(allergyNav.SelectSingleNode("when"));
            this.name.ParseXml(allergyNav.SelectSingleNode("name"));

            XPathNavigator reactionNav =
                allergyNav.SelectSingleNode("reaction");

            if (reactionNav != null)
            {
                this.reaction = new CodableValue();
                this.reaction.ParseXml(reactionNav);
            }

            XPathNavigator treatmentNav =
                allergyNav.SelectSingleNode("treatment");
            if (treatmentNav != null)
            {
                this.treatment = new CodableValue();
                this.treatment.ParseXml(treatmentNav);
            }
        }

        /// <summary>
        /// Writes the allergic episode data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the allergic episode data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> has an <b>null</b> Text property.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            Validator.ThrowSerializationIfNull(this.name.Text, "AllergyNameMandatory");

            // <allergic-episode>
            writer.WriteStartElement("allergic-episode");

            this.when.WriteXml("when", writer);
            this.name.WriteXml("name", writer);

            this.Reaction?.WriteXml("reaction", writer);

            this.Treatment?.WriteXml("treatment", writer);

            // </allergic-episode>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of the allergy for which this episode is
        /// occurring.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the name.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> on set.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "AllergyNameMandatory");
                this.name = value;
            }
        }

        private CodableValue name = new CodableValue();

        /// <summary>
        /// Gets or sets the date/time when the allergic episode occurred.
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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                this.when = value;
            }
        }

        private HealthServiceDateTime when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the reaction the person has.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the reaction.
        /// </value>
        ///
        public CodableValue Reaction
        {
            get { return this.reaction; }
            set { this.reaction = value; }
        }

        private CodableValue reaction;

        /// <summary>
        /// Gets or sets a possible treatment method for this allergy.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> representing the
        /// treatment method.
        /// </value>
        ///
        public CodableValue Treatment
        {
            get { return this.treatment; }
            set { this.treatment = value; }
        }

        private CodableValue treatment;

        /// <summary>
        /// Gets a string representation of the allergic episode item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the allergic episode item.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Name.Text;
        }
    }
}
