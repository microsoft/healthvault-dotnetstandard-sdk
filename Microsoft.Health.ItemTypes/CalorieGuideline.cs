// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;


namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Stores suggested calorie intak guidelines. 
    /// </summary>
    /// 
    public class CalorieGuideline : HealthRecordItem
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
        /// The name definies the guideline. 
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
            When = when;
            Name = name;
            Calories = calories;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        /// 
        public new static readonly Guid TypeId =
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
        /// a "carolie-guideline" node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("calorie-guideline");

            Validator.ThrowInvalidIfNull(itemNav, "CalorieGuidelineUnexpectedNode");

            // when (approxi-date-time, mandatory) 
            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));

            // measurement-name (codable-value, mandatory)
            _name = new CodableValue();
            _name.ParseXml(itemNav.SelectSingleNode("name"));

            // calories (general-measurement, mandatory)
            _calories = new GeneralMeasurement();
            _calories.ParseXml(itemNav.SelectSingleNode("calories"));

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
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="When"/>, <see cref="Name"/> or <see cref="Calories"/>
        /// is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, "WhenNotSet");
            Validator.ThrowSerializationIfNull(_name, "CalorieGuidelineNameNotSet");
            Validator.ThrowSerializationIfNull(_calories, "CalorieGuidelineCaloriesNotSet");

            // <calorie-guideline>
            writer.WriteStartElement("calorie-guideline");

            // <when>
            _when.WriteXml("when", writer);

            // <name>
            _name.WriteXml("name", writer);

            // <calories>
            _calories.WriteXml("calories", writer);

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
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private ApproximateDateTime _when;

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
            get { return _name; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "CalorieGuidelineNameNullValue");
                _name = value;
            }
        }
        private CodableValue _name;

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
            get { return _calories; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "Calories", "CalorieGuidelineCaloriesNullValue");
                _calories = value;
            }
        }
        private GeneralMeasurement _calories;

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
                String.Format(
                    ResourceRetriever.GetResourceString(
                        "NameAndValue"),
                    _name.ToString(),
                    _calories.ToString());
        }
    }
}
