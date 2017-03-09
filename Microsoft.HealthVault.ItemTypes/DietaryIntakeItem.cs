// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A Dietary Intake Item represents the amount of dietary nutrients and minerals consumed.
    /// </summary>
    ///
    public class DietaryIntakeItem : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DietaryIntakeItem"/> class with default values.
        /// </summary>
        ///
        public DietaryIntakeItem()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DietaryIntake"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        /// <param name="foodItem">
        /// Represents the food item that was eaten.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="foodItem"/> is <b>null</b>.
        /// </exception>
        ///
        public DietaryIntakeItem(CodableValue foodItem)
        {
            this.FoodItem = foodItem;
        }

        /// <summary>
        /// Populates this <see cref="DietaryIntakeItem"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the DietaryIntake data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a DietaryIntake node.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.foodItem = new CodableValue();
            this.foodItem.ParseXml(navigator.SelectSingleNode("food-item"));
            this.servingSize = XPathHelper.GetOptNavValue<CodableValue>(navigator, "serving-size");
            this.servingsConsumed = XPathHelper.GetOptNavValueAsDouble(navigator, "servings-consumed");
            this.meal = XPathHelper.GetOptNavValue<CodableValue>(navigator, "meal");
            this.when = XPathHelper.GetOptNavValue<HealthServiceDateTime>(navigator, "when");
            this.energy = XPathHelper.GetOptNavValue<FoodEnergyValue>(navigator, "energy");
            this.energyFromFat = XPathHelper.GetOptNavValue<FoodEnergyValue>(navigator, "energy-from-fat");
            this.totalFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "total-fat");
            this.saturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "saturated-fat");
            this.transFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "trans-fat");
            this.monounsaturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "monounsaturated-fat");
            this.polyunsaturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "polyunsaturated-fat");
            this.protein = XPathHelper.GetOptNavValue<WeightValue>(navigator, "protein");
            this.carbohydrates = XPathHelper.GetOptNavValue<WeightValue>(navigator, "carbohydrates");
            this.dietaryFiber = XPathHelper.GetOptNavValue<WeightValue>(navigator, "dietary-fiber");
            this.sugars = XPathHelper.GetOptNavValue<WeightValue>(navigator, "sugars");
            this.sodium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "sodium");
            this.cholesterol = XPathHelper.GetOptNavValue<WeightValue>(navigator, "cholesterol");
            this.calcium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "calcium");
            this.iron = XPathHelper.GetOptNavValue<WeightValue>(navigator, "iron");
            this.magnesium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "magnesium");
            this.phosphorus = XPathHelper.GetOptNavValue<WeightValue>(navigator, "phosphorus");
            this.potassium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "potassium");
            this.zinc = XPathHelper.GetOptNavValue<WeightValue>(navigator, "zinc");
            this.vitaminARAE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-A-RAE");
            this.vitaminE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-E");
            this.vitaminD = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-D");
            this.vitaminC = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-C");
            this.thiamin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "thiamin");
            this.riboflavin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "riboflavin");
            this.niacin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "niacin");
            this.vitaminB6 = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-B-6");
            this.folateDFE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "folate-DFE");
            this.vitaminB12 = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-B-12");
            this.vitaminK = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-K");

            this.additionalNutritionFacts.Clear();
            XPathNavigator additionalFactsNav = navigator.SelectSingleNode("additional-nutrition-facts");

            if (additionalFactsNav != null)
            {
                foreach (XPathNavigator nav in additionalFactsNav.Select("nutrition-fact"))
                {
                    NutritionFact nutritionFact = new NutritionFact();
                    nutritionFact.ParseXml(nav);
                    this.additionalNutritionFacts.Add(nutritionFact);
                }
            }
        }

        /// <summary>
        /// Writes the XML representation of the DietaryIntake into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the dietary intake item.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the DietaryIntake should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="FoodItem"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "WriteXmlEmptyNodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.foodItem, "FoodItemNullValue");

            writer.WriteStartElement(nodeName);

            this.foodItem.WriteXml("food-item", writer);
            XmlWriterHelper.WriteOpt(writer, "serving-size", this.servingSize);
            XmlWriterHelper.WriteOptDouble(writer, "servings-consumed", this.servingsConsumed);
            XmlWriterHelper.WriteOpt(writer, "meal", this.meal);
            XmlWriterHelper.WriteOpt(writer, "when", this.when);
            XmlWriterHelper.WriteOpt(writer, "energy", this.energy);
            XmlWriterHelper.WriteOpt(writer, "energy-from-fat", this.energyFromFat);
            XmlWriterHelper.WriteOpt(writer, "total-fat", this.totalFat);
            XmlWriterHelper.WriteOpt(writer, "saturated-fat", this.saturatedFat);
            XmlWriterHelper.WriteOpt(writer, "trans-fat", this.transFat);
            XmlWriterHelper.WriteOpt(writer, "monounsaturated-fat", this.monounsaturatedFat);
            XmlWriterHelper.WriteOpt(writer, "polyunsaturated-fat", this.polyunsaturatedFat);
            XmlWriterHelper.WriteOpt(writer, "protein", this.protein);
            XmlWriterHelper.WriteOpt(writer, "carbohydrates", this.carbohydrates);
            XmlWriterHelper.WriteOpt(writer, "dietary-fiber", this.dietaryFiber);
            XmlWriterHelper.WriteOpt(writer, "sugars", this.sugars);
            XmlWriterHelper.WriteOpt(writer, "sodium", this.sodium);
            XmlWriterHelper.WriteOpt(writer, "cholesterol", this.cholesterol);
            XmlWriterHelper.WriteOpt(writer, "calcium", this.calcium);
            XmlWriterHelper.WriteOpt(writer, "iron", this.iron);
            XmlWriterHelper.WriteOpt(writer, "magnesium", this.magnesium);
            XmlWriterHelper.WriteOpt(writer, "phosphorus", this.phosphorus);
            XmlWriterHelper.WriteOpt(writer, "potassium", this.potassium);
            XmlWriterHelper.WriteOpt(writer, "zinc", this.zinc);
            XmlWriterHelper.WriteOpt(writer, "vitamin-A-RAE", this.vitaminARAE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-E", this.vitaminE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-D", this.vitaminD);
            XmlWriterHelper.WriteOpt(writer, "vitamin-C", this.vitaminC);
            XmlWriterHelper.WriteOpt(writer, "thiamin", this.thiamin);
            XmlWriterHelper.WriteOpt(writer, "riboflavin", this.riboflavin);
            XmlWriterHelper.WriteOpt(writer, "niacin", this.niacin);
            XmlWriterHelper.WriteOpt(writer, "vitamin-B-6", this.vitaminB6);
            XmlWriterHelper.WriteOpt(writer, "folate-DFE", this.folateDFE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-B-12", this.vitaminB12);
            XmlWriterHelper.WriteOpt(writer, "vitamin-K", this.vitaminK);

            if (this.additionalNutritionFacts.Count != 0)
            {
                writer.WriteStartElement("additional-nutrition-facts");

                foreach (NutritionFact nutritionFact in this.additionalNutritionFacts)
                {
                    nutritionFact.WriteXml("nutrition-fact", writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the food item that was eaten.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about foodItem the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue FoodItem
        {
            get
            {
                return this.foodItem;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value",
                        ResourceRetriever.GetResourceString("errors", "FoodItemNullValue"));
                }

                this.foodItem = value;
            }
        }

        private CodableValue foodItem;

        /// <summary>
        /// Gets or sets the serving size.
        /// </summary>
        ///
        /// <remarks>
        /// In the USDA food vocab a serving size is defined within the context of a particular food and represents how a food is commonly consumed. For example: cups of strawberries, number of dates, number of table spoons or sticks of butter.
        /// If there is no information about servingSize the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue ServingSize
        {
            get
            {
                return this.servingSize;
            }

            set
            {
                this.servingSize = value;
            }
        }

        private CodableValue servingSize;

        /// <summary>
        /// Gets or sets the number of servings consumed.
        /// </summary>
        ///
        public double? ServingsConsumed
        {
            get
            {
                return this.servingsConsumed;
            }

            set
            {
                this.servingsConsumed = value;
            }
        }

        private double? servingsConsumed;

        /// <summary>
        /// Gets or sets a textual description of a meal.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about meal the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Meal
        {
            get
            {
                return this.meal;
            }

            set
            {
                this.meal = value;
            }
        }

        private CodableValue meal;

        /// <summary>
        /// Gets or sets the date time of consumption.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about when the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public HealthServiceDateTime When
        {
            get
            {
                return this.when;
            }

            set
            {
                this.when = value;
            }
        }

        private HealthServiceDateTime when;

        /// <summary>
        /// Gets or sets the amount of food energy consumed.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about energy the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public FoodEnergyValue Energy
        {
            get
            {
                return this.energy;
            }

            set
            {
                this.energy = value;
            }
        }

        private FoodEnergyValue energy;

        /// <summary>
        /// Gets or sets the amount of food energy consumed that came from fat.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about energyFromFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public FoodEnergyValue EnergyFromFat
        {
            get
            {
                return this.energyFromFat;
            }

            set
            {
                this.energyFromFat = value;
            }
        }

        private FoodEnergyValue energyFromFat;

        /// <summary>
        /// Gets or sets the total amount of fat consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Fat is commonly displayed in grams (g) in UI.
        /// If there is no information about totalFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue TotalFat
        {
            get
            {
                return this.totalFat;
            }

            set
            {
                this.totalFat = value;
            }
        }

        private WeightValue totalFat;

        /// <summary>
        /// Gets or sets the amount of saturated fat consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Fat is commonly displayed in grams (g) in UI.
        /// If there is no information about saturatedFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue SaturatedFat
        {
            get
            {
                return this.saturatedFat;
            }

            set
            {
                this.saturatedFat = value;
            }
        }

        private WeightValue saturatedFat;

        /// <summary>
        /// Gets or sets the amount of trans fat consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Fat is commonly displayed in grams (g) in UI.
        /// If there is no information about transFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue TransFat
        {
            get
            {
                return this.transFat;
            }

            set
            {
                this.transFat = value;
            }
        }

        private WeightValue transFat;

        /// <summary>
        /// Gets or sets the amount of mono unsaturated fat consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Fat is commonly displayed in grams (g) in UI.
        /// If there is no information about monounsaturatedFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue MonounsaturatedFat
        {
            get
            {
                return this.monounsaturatedFat;
            }

            set
            {
                this.monounsaturatedFat = value;
            }
        }

        private WeightValue monounsaturatedFat;

        /// <summary>
        /// Gets or sets the amount of poly unsaturated fat consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Fat is commonly displayed in grams (g) in UI.
        /// If there is no information about polyunsaturatedFat the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue PolyunsaturatedFat
        {
            get
            {
                return this.polyunsaturatedFat;
            }

            set
            {
                this.polyunsaturatedFat = value;
            }
        }

        private WeightValue polyunsaturatedFat;

        /// <summary>
        /// Gets or sets the amount of protein consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Protein is commonly displayed in grams (g) in UI.
        /// If there is no information about protein the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Protein
        {
            get
            {
                return this.protein;
            }

            set
            {
                this.protein = value;
            }
        }

        private WeightValue protein;

        /// <summary>
        /// Gets or sets the amount of carbohydrates consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Carbohydrates are commonly displayed in grams (g) in UI.
        /// If there is no information about carbohydrates the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Carbohydrates
        {
            get
            {
                return this.carbohydrates;
            }

            set
            {
                this.carbohydrates = value;
            }
        }

        private WeightValue carbohydrates;

        /// <summary>
        /// Gets or sets the amount of dietary fiber consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Dietary fiber is commonly displayed in grams (g) in UI.
        /// If there is no information about dietaryFiber the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue DietaryFiber
        {
            get
            {
                return this.dietaryFiber;
            }

            set
            {
                this.dietaryFiber = value;
            }
        }

        private WeightValue dietaryFiber;

        /// <summary>
        /// Gets or sets the total amount of sugars consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Sugar is commonly displayed in grams (g) in UI.
        /// If there is no information about sugars the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Sugars
        {
            get
            {
                return this.sugars;
            }

            set
            {
                this.sugars = value;
            }
        }

        private WeightValue sugars;

        /// <summary>
        /// Gets or sets the amount of sodium consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Sodium is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about sodium the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Sodium
        {
            get
            {
                return this.sodium;
            }

            set
            {
                this.sodium = value;
            }
        }

        private WeightValue sodium;

        /// <summary>
        /// Gets or sets the amount of cholesterol consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Cholesterol is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about cholesterol the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Cholesterol
        {
            get
            {
                return this.cholesterol;
            }

            set
            {
                this.cholesterol = value;
            }
        }

        private WeightValue cholesterol;

        /// <summary>
        /// Gets or sets the amount of calcium consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Calcium is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about calcium the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Calcium
        {
            get
            {
                return this.calcium;
            }

            set
            {
                this.calcium = value;
            }
        }

        private WeightValue calcium;

        /// <summary>
        /// Gets or sets the amount of iron consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Iron is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about iron the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Iron
        {
            get
            {
                return this.iron;
            }

            set
            {
                this.iron = value;
            }
        }

        private WeightValue iron;

        /// <summary>
        /// Gets or sets the amount of magnesium consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Magnesium is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about magnesium the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Magnesium
        {
            get
            {
                return this.magnesium;
            }

            set
            {
                this.magnesium = value;
            }
        }

        private WeightValue magnesium;

        /// <summary>
        /// Gets or sets the amount of phosphorus consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Phosphorus is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about phosphorus the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Phosphorus
        {
            get
            {
                return this.phosphorus;
            }

            set
            {
                this.phosphorus = value;
            }
        }

        private WeightValue phosphorus;

        /// <summary>
        /// Gets or sets the amount of potassium consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Potassium is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about potassium the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Potassium
        {
            get
            {
                return this.potassium;
            }

            set
            {
                this.potassium = value;
            }
        }

        private WeightValue potassium;

        /// <summary>
        /// Gets or sets the amount of zinc consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Zinc is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about zinc the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Zinc
        {
            get
            {
                return this.zinc;
            }

            set
            {
                this.zinc = value;
            }
        }

        private WeightValue zinc;

        /// <summary>
        /// Gets or sets this value is the retinol activity equivalent (RAE) weight of vitamin A consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin A-RAE is commonly displayed in micrograms RAE in UI.
        /// If there is no information about vitaminARAE the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminARAE
        {
            get
            {
                return this.vitaminARAE;
            }

            set
            {
                this.vitaminARAE = value;
            }
        }

        private WeightValue vitaminARAE;

        /// <summary>
        /// Gets or sets the amount of vitamin E consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin E is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminE the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminE
        {
            get
            {
                return this.vitaminE;
            }

            set
            {
                this.vitaminE = value;
            }
        }

        private WeightValue vitaminE;

        /// <summary>
        /// Gets or sets the amount of vitamin D consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin D is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminD the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminD
        {
            get
            {
                return this.vitaminD;
            }

            set
            {
                this.vitaminD = value;
            }
        }

        private WeightValue vitaminD;

        /// <summary>
        /// Gets or sets the amount of vitamin C consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin C is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminC the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminC
        {
            get
            {
                return this.vitaminC;
            }

            set
            {
                this.vitaminC = value;
            }
        }

        private WeightValue vitaminC;

        /// <summary>
        /// Gets or sets the amount of thiamin consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Thiamin is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about thiamin the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Thiamin
        {
            get
            {
                return this.thiamin;
            }

            set
            {
                this.thiamin = value;
            }
        }

        private WeightValue thiamin;

        /// <summary>
        /// Gets or sets the amount of riboflavin consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Riboflavin is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about riboflavin the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Riboflavin
        {
            get
            {
                return this.riboflavin;
            }

            set
            {
                this.riboflavin = value;
            }
        }

        private WeightValue riboflavin;

        /// <summary>
        /// Gets or sets the amount of niacin consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Niacin is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about niacin the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue Niacin
        {
            get
            {
                return this.niacin;
            }

            set
            {
                this.niacin = value;
            }
        }

        private WeightValue niacin;

        /// <summary>
        /// Gets or sets the amount of vitamin B-6 consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin B-6 is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminB6 the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminB6
        {
            get
            {
                return this.vitaminB6;
            }

            set
            {
                this.vitaminB6 = value;
            }
        }

        private WeightValue vitaminB6;

        /// <summary>
        /// Gets or sets this value is the dietary folate equivalent (DFE) of Folate consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Folate is commonly displayed in micrograms DFE in UI.
        /// If there is no information about folateDFE the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue FolateDFE
        {
            get
            {
                return this.folateDFE;
            }

            set
            {
                this.folateDFE = value;
            }
        }

        private WeightValue folateDFE;

        /// <summary>
        /// Gets or sets the amount of vitamin B-12 consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin B-12 is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminB12 the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminB12
        {
            get
            {
                return this.vitaminB12;
            }

            set
            {
                this.vitaminB12 = value;
            }
        }

        private WeightValue vitaminB12;

        /// <summary>
        /// Gets or sets the amount of vitamin K consumed.
        /// </summary>
        ///
        /// <remarks>
        /// Vitamin K is commonly displayed in milligrams (mg) in UI.
        /// If there is no information about vitaminK the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public WeightValue VitaminK
        {
            get
            {
                return this.vitaminK;
            }

            set
            {
                this.vitaminK = value;
            }
        }

        private WeightValue vitaminK;

        /// <summary>
        /// Gets the additional nutritional fact data.
        /// </summary>
        ///
        /// <remarks>
        /// For instance, this could contain the amount of caffeine, or the amount of Omega-3 fatty acids consumed.
        /// If there is no information about additionalNutritionFacts the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Collection<NutritionFact> AdditionalNutritionFacts => this.additionalNutritionFacts;

        private readonly Collection<NutritionFact> additionalNutritionFacts = new Collection<NutritionFact>();

        /// <summary>
        /// Gets a string representation of the DietaryIntake.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the DietaryIntake.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.ServingSize != null)
            {
                if (this.ServingsConsumed.HasValue)
                {
                    return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("DietaryIntakeServingSizeServingConsumedFormat"),
                        this.FoodItem.Text,
                        this.ServingsConsumed.Value,
                        this.ServingSize.Text);
                }

                return string.Format(
                    CultureInfo.CurrentUICulture,
                    ResourceRetriever.GetResourceString("DietaryIntakeServingSizeFormat"),
                    this.FoodItem.Text,
                    this.ServingSize.Text);
            }

            return this.FoodItem.Text;
        }
    }
}
