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
    /// Represents a health record item type that encapsulates a person's 
    /// dietary intake for a day.
    /// </summary>
    /// 
    public class DietaryDailyIntake : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DietaryDailyIntake"/> class with 
        /// default values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> 
        /// method is called.
        /// </remarks>
        /// 
        public DietaryDailyIntake()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DietaryDailyIntake"/> class 
        /// specifying the mandatory values.
        /// </summary>
        /// 
        /// <param name="when">
        /// The date when the food was consumed.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public DietaryDailyIntake(HealthServiceDate when)
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
            new Guid("9c29c6b9-f40e-44ff-b24e-fba6f3074638");

        /// <summary>
        /// Populates this <see cref="DietaryDailyIntake"/> instance from the 
        /// data in the XML.
        /// </summary>
        /// 
        /// <param name="typeSpecificXml">
        /// The XML to get the dietary intake data from.
        /// </param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a <see cref="DietaryDailyIntake"/> node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator intakeNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "dietary-intake-daily");

            Validator.ThrowInvalidIfNull(intakeNav, "DietaryDailyIntakeUnexpectedNode");

            _when = new HealthServiceDate();
            _when.ParseXml(intakeNav.SelectSingleNode("when"));

            XPathNavigator caloriesNav = 
                intakeNav.SelectSingleNode("calories");

            if (caloriesNav != null)
            {
                _calories = caloriesNav.ValueAsInt;
            }

            XPathNavigator totalFatNav =
                intakeNav.SelectSingleNode("total-fat");

            if (totalFatNav != null)
            {
                _totalFat = new WeightValue();
                _totalFat.ParseXml(totalFatNav);
            }

            XPathNavigator satFatNav =
                intakeNav.SelectSingleNode("saturated-fat");

            if (satFatNav != null)
            {
                _saturatedFat = new WeightValue();
                _saturatedFat.ParseXml(satFatNav);
            }

            XPathNavigator transFatNav =
                intakeNav.SelectSingleNode("trans-fat");

            if (transFatNav != null)
            {
                _transFat = new WeightValue();
                _transFat.ParseXml(transFatNav);
            }

            XPathNavigator proteinNav =
                intakeNav.SelectSingleNode("protein");

            if (proteinNav != null)
            {
                _protein = new WeightValue();
                _protein.ParseXml(proteinNav);
            }

            XPathNavigator totalCarbsNav =
                intakeNav.SelectSingleNode("total-carbohydrates");

            if (totalCarbsNav != null)
            {
                _totalCarbs = new WeightValue();
                _totalCarbs.ParseXml(totalCarbsNav);
            }

            XPathNavigator fiberNav =
                intakeNav.SelectSingleNode("dietary-fiber");

            if (fiberNav != null)
            {
                _fiber = new WeightValue();
                _fiber.ParseXml(fiberNav);
            }

            XPathNavigator sugarsNav =
                intakeNav.SelectSingleNode("sugars");

            if (sugarsNav != null)
            {
                _sugars = new WeightValue();
                _sugars.ParseXml(sugarsNav);
            }

            XPathNavigator sodiumNav =
                intakeNav.SelectSingleNode("sodium");

            if (sodiumNav != null)
            {
                _sodium = new WeightValue();
                _sodium.ParseXml(sodiumNav);
            }

            XPathNavigator cholesterolNav =
                intakeNav.SelectSingleNode("cholesterol");

            if (cholesterolNav != null)
            {
                _cholesterol = new WeightValue();
                _cholesterol.ParseXml(cholesterolNav);
            }
        }

        /// <summary>
        /// Writes the dietary daily intake data to the specified XmlWriter.
        /// </summary>
        /// 
        /// <param name="writer">
        /// The XmlWriter to write the dietary daily intake data to.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <dietary-intake-daily>
            writer.WriteStartElement("dietary-intake-daily");

            // <when>
            _when.WriteXml("when", writer);

            // <calories>
            if (_calories != null)
            {
                writer.WriteElementString(
                    "calories", 
                    _calories.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <total-fat>
            if (_totalFat != null)
            {
                _totalFat.WriteXml("total-fat", writer);
            }

            // <saturated-fat>
            if (_saturatedFat != null)
            {
                _saturatedFat.WriteXml("saturated-fat", writer);
            }

            // <trans-fat>
            if (_transFat != null)
            {
                _transFat.WriteXml("trans-fat", writer);
            }

            // <protein>
            if (_protein != null)
            {
                _protein.WriteXml("protein", writer);
            }

            // <total-carbohydrates>
            if (_totalCarbs != null)
            {
                _totalCarbs.WriteXml("total-carbohydrates", writer);
            }

            // <dietary-fiber>
            if (_fiber != null)
            {
                _fiber.WriteXml("dietary-fiber", writer);
            }

            // <sugars>
            if (_sugars != null)
            {
                _sugars.WriteXml("sugars", writer);
            }

            // <sodium>
            if (_sodium != null)
            {
                _sodium.WriteXml("sodium", writer);
            }

            // <cholesterol>
            if (_cholesterol != null)
            {
                _cholesterol.WriteXml("cholesterol", writer);
            }

            // </dietary-intake-daily>
            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the date of the dietary intake.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="HealthServiceDate"/> representing the date.
        /// </value>
        /// 
        /// <remarks>
        /// The value defaults to the current year, month, and day.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public HealthServiceDate When
        {
            get { return _when; }
            set
            {
                Validator.ThrowIfArgumentNull(value, "When", "WhenNullValue");
                _when = value;
            }
        }
        private HealthServiceDate _when = new HealthServiceDate();

        /// <summary>
        /// Gets or sets the caloric intake for the day.
        /// </summary>
        /// 
        /// <value>
        /// An integer representing the intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the caloric intake is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public int? Calories
        {
            get { return _calories; }
            set { _calories = value; }
        }
        private int? _calories;


        /// <summary>
        /// Gets or sets the total intake of fat for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the fat intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the total fat is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue TotalFat
        {
            get { return _totalFat; }
            set { _totalFat = value; }
        }
        private WeightValue _totalFat;

        /// <summary>
        /// Gets or sets the intake of saturated fat for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the fat intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the saturated fat is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue SaturatedFat
        {
            get { return _saturatedFat; }
            set { _saturatedFat = value; }
        }
        private WeightValue _saturatedFat;

        /// <summary>
        /// Gets or sets the intake of trans fat for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the fat intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the trans fat is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue TransFat
        {
            get { return _transFat; }
            set { _transFat = value; }
        }
        private WeightValue _transFat;

        /// <summary>
        /// Gets or sets the intake of protein for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the protein intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the protein intake is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue Protein
        {
            get { return _protein; }
            set { _protein = value; }
        }
        private WeightValue _protein;

        /// <summary>
        /// Gets or sets the total intake of carbohydrates for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the 
        /// carbohydrate intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the total carbohydrates intake is not known, the value can be 
        /// set to <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue TotalCarbohydrates
        {
            get { return _totalCarbs; }
            set { _totalCarbs = value; }
        }
        private WeightValue _totalCarbs;

        /// <summary>
        /// Gets or sets the intake of dietary fiber for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the fiber intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the dietary fiber intake is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue DietaryFiber
        {
            get { return _fiber; }
            set { _fiber = value; }
        }
        private WeightValue _fiber;

        /// <summary>
        /// Gets or sets the intake of sugars for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the sugar intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the sugar intake is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue Sugars
        {
            get { return _sugars; }
            set { _sugars = value; }
        }
        private WeightValue _sugars;

        /// <summary>
        /// Gets or sets the intake of sodium for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the sodium intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the sodium intake is not known the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue Sodium
        {
            get { return _sodium; }
            set { _sodium = value; }
        }
        private WeightValue _sodium;

        /// <summary>
        /// Gets or sets the intake of cholesterol for the day.
        /// </summary>
        /// 
        /// <value>
        /// An instance of <see cref="WeightValue"/> representing the cholesterol intake.
        /// </value> 
        /// 
        /// <remarks>
        /// If the cholesterol intake is not known, the value can be set to
        /// <b>null</b>.
        /// </remarks>
        /// 
        public WeightValue Cholesterol
        {
            get { return _cholesterol; }
            set { _cholesterol = value; }
        }
        private WeightValue _cholesterol;

        /// <summary>
        /// Gets a string representation of the dietary daily intake item.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the dietary daily intake item.
        /// </returns>
        /// 
        public override string ToString()
        {
            string listSeparator = 
                ResourceRetriever.GetResourceString(
                        "ListSeparator");

            StringBuilder result = new StringBuilder(100);

            if (Calories != null)
            {
                result.AppendFormat(
                    ResourceRetriever.GetResourceString(
                        "DietaryDailyIntakeToStringFormatCalories"),
                    Calories.Value);
            }

            if (TotalFat != null)
            {
                if (Calories != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(  result,
                                    TotalFat,
                                    "DietaryDailyIntakeToStringFormatTotalFat");
            }

            if (Protein != null)
            {
                if (Calories != null || TotalFat != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(  result,
                                    Protein,
                                    "DietaryDailyIntakeToStringFormatProtein");
            }

            if (TotalCarbohydrates != null)
            {
                if (Calories != null || TotalFat != null || Protein != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(  result, 
                                    TotalCarbohydrates, 
                                    "DietaryDailyIntakeToStringFormatTotalCarbs");
            }
            return result.ToString();
        }

        private static void AppendWeightValue(StringBuilder result, WeightValue value, string labelStringId)
        {
            string unitsString = String.Empty;
            string valueString;

            if (value.DisplayValue != null)
            {
                valueString = value.DisplayValue.ToString();
            }
            else
            {
                valueString = (value.Value * 1000).ToString(CultureInfo.InvariantCulture);
                unitsString = ResourceRetriever.GetResourceString(
                                    "DietaryDailyIntakeToStringUnitGrams");
            }

            result.AppendFormat(
                ResourceRetriever.GetResourceString(labelStringId),
                valueString,
                unitsString);
        }
    }

}
