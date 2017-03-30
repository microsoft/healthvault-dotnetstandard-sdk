// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a person's
    /// dietary intake for a day.
    /// </summary>
    ///
    public class DietaryDailyIntake : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DietaryDailyIntake"/> class with
        /// default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        public static new readonly Guid TypeId =
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

            Validator.ThrowInvalidIfNull(intakeNav, Resources.DietaryDailyIntakeUnexpectedNode);

            this.when = new HealthServiceDate();
            this.when.ParseXml(intakeNav.SelectSingleNode("when"));

            XPathNavigator caloriesNav =
                intakeNav.SelectSingleNode("calories");

            if (caloriesNav != null)
            {
                this.calories = caloriesNav.ValueAsInt;
            }

            XPathNavigator totalFatNav =
                intakeNav.SelectSingleNode("total-fat");

            if (totalFatNav != null)
            {
                this.totalFat = new WeightValue();
                this.totalFat.ParseXml(totalFatNav);
            }

            XPathNavigator satFatNav =
                intakeNav.SelectSingleNode("saturated-fat");

            if (satFatNav != null)
            {
                this.saturatedFat = new WeightValue();
                this.saturatedFat.ParseXml(satFatNav);
            }

            XPathNavigator transFatNav =
                intakeNav.SelectSingleNode("trans-fat");

            if (transFatNav != null)
            {
                this.transFat = new WeightValue();
                this.transFat.ParseXml(transFatNav);
            }

            XPathNavigator proteinNav =
                intakeNav.SelectSingleNode("protein");

            if (proteinNav != null)
            {
                this.protein = new WeightValue();
                this.protein.ParseXml(proteinNav);
            }

            XPathNavigator totalCarbsNav =
                intakeNav.SelectSingleNode("total-carbohydrates");

            if (totalCarbsNav != null)
            {
                this.totalCarbs = new WeightValue();
                this.totalCarbs.ParseXml(totalCarbsNav);
            }

            XPathNavigator fiberNav =
                intakeNav.SelectSingleNode("dietary-fiber");

            if (fiberNav != null)
            {
                this.fiber = new WeightValue();
                this.fiber.ParseXml(fiberNav);
            }

            XPathNavigator sugarsNav =
                intakeNav.SelectSingleNode("sugars");

            if (sugarsNav != null)
            {
                this.sugars = new WeightValue();
                this.sugars.ParseXml(sugarsNav);
            }

            XPathNavigator sodiumNav =
                intakeNav.SelectSingleNode("sodium");

            if (sodiumNav != null)
            {
                this.sodium = new WeightValue();
                this.sodium.ParseXml(sodiumNav);
            }

            XPathNavigator cholesterolNav =
                intakeNav.SelectSingleNode("cholesterol");

            if (cholesterolNav != null)
            {
                this.cholesterol = new WeightValue();
                this.cholesterol.ParseXml(cholesterolNav);
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
            this.when.WriteXml("when", writer);

            // <calories>
            if (this.calories != null)
            {
                writer.WriteElementString(
                    "calories",
                    this.calories.Value.ToString(CultureInfo.InvariantCulture));
            }

            // <total-fat>
            this.totalFat?.WriteXml("total-fat", writer);

            // <saturated-fat>
            this.saturatedFat?.WriteXml("saturated-fat", writer);

            // <trans-fat>
            this.transFat?.WriteXml("trans-fat", writer);

            // <protein>
            this.protein?.WriteXml("protein", writer);

            // <total-carbohydrates>
            this.totalCarbs?.WriteXml("total-carbohydrates", writer);

            // <dietary-fiber>
            this.fiber?.WriteXml("dietary-fiber", writer);

            // <sugars>
            this.sugars?.WriteXml("sugars", writer);

            // <sodium>
            this.sodium?.WriteXml("sodium", writer);

            // <cholesterol>
            this.cholesterol?.WriteXml("cholesterol", writer);

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
            get { return this.when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(this.When), Resources.WhenNullValue);
                this.when = value;
            }
        }

        private HealthServiceDate when = new HealthServiceDate();

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
            get { return this.calories; }
            set { this.calories = value; }
        }

        private int? calories;

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
            get { return this.totalFat; }
            set { this.totalFat = value; }
        }

        private WeightValue totalFat;

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
            get { return this.saturatedFat; }
            set { this.saturatedFat = value; }
        }

        private WeightValue saturatedFat;

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
            get { return this.transFat; }
            set { this.transFat = value; }
        }

        private WeightValue transFat;

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
            get { return this.protein; }
            set { this.protein = value; }
        }

        private WeightValue protein;

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
            get { return this.totalCarbs; }
            set { this.totalCarbs = value; }
        }

        private WeightValue totalCarbs;

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
            get { return this.fiber; }
            set { this.fiber = value; }
        }

        private WeightValue fiber;

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
            get { return this.sugars; }
            set { this.sugars = value; }
        }

        private WeightValue sugars;

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
            get { return this.sodium; }
            set { this.sodium = value; }
        }

        private WeightValue sodium;

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
            get { return this.cholesterol; }
            set { this.cholesterol = value; }
        }

        private WeightValue cholesterol;

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
                Resources.ListSeparator;

            StringBuilder result = new StringBuilder(100);

            if (this.Calories != null)
            {
                result.AppendFormat(
                    Resources.DietaryDailyIntakeToStringFormatCalories,
                    this.Calories.Value);
            }

            if (this.TotalFat != null)
            {
                if (this.Calories != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(
                    result,
                    this.TotalFat,
                    Resources.DietaryDailyIntakeToStringFormatTotalFat);
            }

            if (this.Protein != null)
            {
                if (this.Calories != null || this.TotalFat != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(
                    result,
                    this.Protein,
                    Resources.DietaryDailyIntakeToStringFormatProtein);
            }

            if (this.TotalCarbohydrates != null)
            {
                if (this.Calories != null || this.TotalFat != null || this.Protein != null)
                {
                    result.Append(listSeparator);
                }

                AppendWeightValue(
                    result,
                    this.TotalCarbohydrates,
                    Resources.DietaryDailyIntakeToStringFormatTotalCarbs);
            }

            return result.ToString();
        }

        private static void AppendWeightValue(StringBuilder result, WeightValue value, string labelFormat)
        {
            string unitsString = string.Empty;
            string valueString;

            if (value.DisplayValue != null)
            {
                valueString = value.DisplayValue.ToString();
            }
            else
            {
                valueString = (value.Value * 1000).ToString(CultureInfo.InvariantCulture);
                unitsString = Resources.DietaryDailyIntakeToStringUnitGrams;
            }

            result.AppendFormat(
                labelFormat,
                valueString,
                unitsString);
        }
    }
}
