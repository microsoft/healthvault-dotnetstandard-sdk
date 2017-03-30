// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Stores suggested calorie intake guidelines.
    /// </summary>
    ///
    public class CalorieGuideline : ThingBase
    {
        /// <summary>
        /// Constructs an instance of suggested calorie intake guidelines with default values.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Daily calories suggested for weight loss, calories needed for weight
        /// maintenance, BMR.
        /// </remarks>
        ///
        public CalorieGuideline()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Constructs an instance of suggested calorie intake guideline with specified values.
        /// </summary>
        ///
        /// <remarks>
        /// Examples: Daily calories suggested for weight loss, calories needed for weight
        /// maintenance, BMR.
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time the guidelines were created.
        /// </param>
        ///
        /// <param name="name">
        /// The name defines the guideline.
        /// </param>
        ///
        /// <param name="calories">
        /// The number of calories to support the guideline.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/>, <paramref name="name"/> or <paramref name="calories"/>
        /// is <b>null</b>.
        /// </exception>
        ///
        public CalorieGuideline(
            ApproximateDateTime when,
            CodableValue name,
            GeneralMeasurement calories)
            : base(TypeId)
        {
            this.When = when;
            this.Name = name;
            this.Calories = calories;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("d3170d30-a41b-4bde-a116-87698c8a001a");

        /// <summary>
        /// Populates this <see cref="CalorieGuideline"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the calorie guideline data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a "calorie-guideline" node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("calorie-guideline");

            Validator.ThrowInvalidIfNull(itemNav, Resources.CalorieGuidelineUnexpectedNode);

            // when (approxi-date-time, mandatory)
            this.when = new ApproximateDateTime();
            this.when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            this.name = new CodableValue();
            this.name.ParseXml(itemNav.SelectSingleNode("name"));

            // calories (general-measurement, mandatory)
            this.calories = new GeneralMeasurement();
            this.calories.ParseXml(itemNav.SelectSingleNode("calories"));
        }

        /// <summary>
        /// Writes the calorie guideline data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the calorie guideline data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/>, <see cref="Name"/> or <see cref="Calories"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.when, Resources.WhenNotSet);
            Validator.ThrowSerializationIfNull(this.name, Resources.CalorieGuidelineNameNotSet);
            Validator.ThrowSerializationIfNull(this.calories, Resources.CalorieGuidelineCaloriesNotSet);

            // <calorie-guideline>
            writer.WriteStartElement("calorie-guideline");

            // <when>
            this.when.WriteXml("when", writer);

            // <name>
            this.name.WriteXml("name", writer);

            // <calories>
            this.calories.WriteXml("calories", writer);

            // </calorie-guideline>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time the guidelines were created.
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
        /// Gets or sets the name of the guideline.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the guideline definition.
        /// </value>
        ///
        /// <remarks>
        /// Examples: Maintain weight, Lose weight slowly, Gain weight slowly, Basal metabolic
        /// rate (BMR). The preferred vocabulary is "calorie-guideline-names".
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public CodableValue Name
        {
            get { return this.name; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.CalorieGuidelineNameNullValue);
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets the number of calories to support the guideline.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="CodableValue"/> instance representing the number of
        /// calories to support the guideline.
        /// </value>
        ///
        /// <remarks>
        /// This value expresses the number of calories for a specific time period.
        /// For example, 2716 calories per day could be coded as follows:
        /// Display = "2716 calories/day"
        /// Structured Value = "2716"
        /// Structured Units = "calories-per-day"
        /// using the calorie-guideline-units vocabulary.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public GeneralMeasurement Calories
        {
            get { return this.calories; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.Calories), Resources.CalorieGuidelineCaloriesNullValue);
                this.calories = value;
            }
        }

        private GeneralMeasurement calories;

        /// <summary>
        /// Gets the representation of a CalorieGuideline instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CalorieGuideline instance.
        /// </returns>
        ///
        public override string ToString()
        {
            return
                string.Format(
                    Resources.NameAndValue,
                    this.name.ToString(),
                    this.calories.ToString());
        }
    }
}
