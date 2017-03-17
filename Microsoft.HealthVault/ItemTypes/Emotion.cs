// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates an emotion.
    /// </summary>
    ///
    public class Emotion : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Emotion"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public Emotion()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Emotion"/> class with the
        /// specified date/time.
        /// </summary>
        ///
        /// <param name="when">
        /// The date/time when the emotion occurred.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public Emotion(HealthServiceDateTime when)
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
            new Guid("4b7971d6-e427-427d-bf2c-2fbcf76606b3");

        /// <summary>
        /// Populates this <see cref="Emotion"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the emotion data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a emotion node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator emotionNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("emotion");

            Validator.ThrowInvalidIfNull(emotionNav, Resources.EmotionUnexpectedNode);

            this.when = new HealthServiceDateTime();
            this.when.ParseXml(emotionNav.SelectSingleNode("when"));

            XPathNavigator moodNav =
                emotionNav.SelectSingleNode("mood");

            if (moodNav != null)
            {
                this.mood = (Mood)moodNav.ValueAsInt;
            }

            XPathNavigator stressNav =
                emotionNav.SelectSingleNode("stress");

            if (stressNav != null)
            {
                this.stress = (RelativeRating)stressNav.ValueAsInt;
            }

            XPathNavigator wellbeingNav =
                emotionNav.SelectSingleNode("wellbeing");

            if (wellbeingNav != null)
            {
                this.wellbeing = (Wellbeing)wellbeingNav.ValueAsInt;
            }
        }

        /// <summary>
        /// Writes the emotion data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the emotion data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <emotion>
            writer.WriteStartElement("emotion");

            // <when>
            this.when.WriteXml("when", writer);

            if (this.mood != Mood.None)
            {
                writer.WriteElementString(
                    "mood",
                    ((int)this.mood).ToString(CultureInfo.InvariantCulture));
            }

            if (this.stress != RelativeRating.None)
            {
                writer.WriteElementString(
                    "stress",
                    ((int)this.stress).ToString(CultureInfo.InvariantCulture));
            }

            if (this.wellbeing != Wellbeing.None)
            {
                writer.WriteElementString(
                    "wellbeing",
                    ((int)this.wellbeing).ToString(CultureInfo.InvariantCulture));
            }

            // </emotion>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date/time when the emotion occurred.
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
        /// Gets or sets the mood of the person.
        /// </summary>
        ///
        /// <value>
        /// A value representing the mood.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <see cref="ItemTypes.Mood.None"/> if
        /// the mood should not be stored.
        /// </remarks>
        ///
        public Mood Mood
        {
            get { return this.mood; }
            set { this.mood = value; }
        }

        private Mood mood;

        /// <summary>
        /// Gets or sets the stress level of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="RelativeRating"/> value representing the stress level.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <see cref="RelativeRating.None"/> if the stress
        /// level should not be stored.
        /// </remarks>
        ///
        public RelativeRating Stress
        {
            get { return this.stress; }
            set { this.stress = value; }
        }

        private RelativeRating stress;

        /// <summary>
        /// Gets or sets the wellbeing of the person.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Wellbeing"/> value.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <see cref="ItemTypes.Wellbeing.None"/>
        /// if the wellbeing should not be stored.
        /// </remarks>
        ///
        public Wellbeing Wellbeing
        {
            get { return this.wellbeing; }
            set { this.wellbeing = value; }
        }

        private Wellbeing wellbeing;

        /// <summary>
        /// Gets a string representation of the emotion item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the emotion item.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(100);

            if (this.Mood != Mood.None)
            {
                result.AppendFormat(
                    Resources.EmotionToStringFormatMood,
                    (int)this.Mood);
            }

            if (this.Stress != RelativeRating.None)
            {
                result.AppendFormat(
                    Resources.EmotionToStringFormatStress,
                    (int)this.Stress);
            }

            if (this.Wellbeing != Wellbeing.None)
            {
                result.AppendFormat(
                    Resources.EmotionToStringFormatWellbeing,
                    (int)this.Wellbeing);
            }

            return result.ToString();
        }
    }
}
