// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates an emotion.
    /// </summary>
    /// 
    public class Emotion : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Emotion"/> class with default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(emotionNav, "EmotionUnexpectedNode");

            _when = new HealthServiceDateTime();
            _when.ParseXml(emotionNav.SelectSingleNode("when"));

            XPathNavigator moodNav =
                emotionNav.SelectSingleNode("mood");

            if (moodNav != null)
            {
                _mood = (Mood)moodNav.ValueAsInt;
            }

            XPathNavigator stressNav =
                emotionNav.SelectSingleNode("stress");

            if (stressNav != null)
            {
                _stress = (RelativeRating)stressNav.ValueAsInt;
            }

            XPathNavigator wellbeingNav =
                emotionNav.SelectSingleNode("wellbeing");

            if (wellbeingNav != null)
            {
                _wellbeing = (Wellbeing)wellbeingNav.ValueAsInt;
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
            _when.WriteXml("when", writer);

            if (_mood != Mood.None)
            {
                writer.WriteElementString(
                    "mood",
                    ((int)_mood).ToString(CultureInfo.InvariantCulture));
            }

            if (_stress != RelativeRating.None)
            {
                writer.WriteElementString(
                    "stress",
                    ((int)_stress).ToString(CultureInfo.InvariantCulture));
            }

            if (_wellbeing != Wellbeing.None)
            {
                writer.WriteElementString(
                    "wellbeing",
                    ((int)_wellbeing).ToString(CultureInfo.InvariantCulture));
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
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDateTime _when = new HealthServiceDateTime();

        /// <summary>
        /// Gets or sets the mood of the person.
        /// </summary>
        /// 
        /// <value>
        /// A value representing the mood.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <see cref="Microsoft.Health.ItemTypes.Mood.None"/> if 
        /// the mood should not be stored.
        /// </remarks>
        /// 
        public Mood Mood
        {
            get { return _mood; }
            set { _mood = value; }
        }
        private Mood _mood;

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
            get { return _stress; }
            set { _stress = value; }
        }
        private RelativeRating _stress;

        /// <summary>
        /// Gets or sets the wellbeing of the person.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="Wellbeing"/> value.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <see cref="Microsoft.Health.ItemTypes.Wellbeing.None"/> 
        /// if the wellbeing should not be stored.
        /// </remarks>
        /// 
        public Wellbeing Wellbeing
        {
            get { return _wellbeing; }
            set { _wellbeing = value; }
        }
        private Wellbeing _wellbeing;

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

            if (Mood != Mood.None)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "EmotionToStringFormatMood"),
                    (int)Mood);
            }

            if (Stress != RelativeRating.None)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "EmotionToStringFormatStress"),
                    (int)Stress);
            }


            if (Wellbeing != Wellbeing.None)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "EmotionToStringFormatWellbeing"),
                    (int)Wellbeing);
            }
            return result.ToString();
        }
    }
}
