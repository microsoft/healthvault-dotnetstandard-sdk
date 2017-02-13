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
            FoodItem = foodItem;
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

            _foodItem = new CodableValue();
            _foodItem.ParseXml(navigator.SelectSingleNode("food-item"));
            _servingSize = XPathHelper.GetOptNavValue<CodableValue>(navigator, "serving-size");
            _servingsConsumed = XPathHelper.GetOptNavValueAsDouble(navigator, "servings-consumed");
            _meal = XPathHelper.GetOptNavValue<CodableValue>(navigator, "meal");
            _when = XPathHelper.GetOptNavValue<HealthServiceDateTime>(navigator, "when");
            _energy = XPathHelper.GetOptNavValue<FoodEnergyValue>(navigator, "energy");
            _energyFromFat = XPathHelper.GetOptNavValue<FoodEnergyValue>(navigator, "energy-from-fat");
            _totalFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "total-fat");
            _saturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "saturated-fat");
            _transFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "trans-fat");
            _monounsaturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "monounsaturated-fat");
            _polyunsaturatedFat = XPathHelper.GetOptNavValue<WeightValue>(navigator, "polyunsaturated-fat");
            _protein = XPathHelper.GetOptNavValue<WeightValue>(navigator, "protein");
            _carbohydrates = XPathHelper.GetOptNavValue<WeightValue>(navigator, "carbohydrates");
            _dietaryFiber = XPathHelper.GetOptNavValue<WeightValue>(navigator, "dietary-fiber");
            _sugars = XPathHelper.GetOptNavValue<WeightValue>(navigator, "sugars");
            _sodium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "sodium");
            _cholesterol = XPathHelper.GetOptNavValue<WeightValue>(navigator, "cholesterol");
            _calcium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "calcium");
            _iron = XPathHelper.GetOptNavValue<WeightValue>(navigator, "iron");
            _magnesium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "magnesium");
            _phosphorus = XPathHelper.GetOptNavValue<WeightValue>(navigator, "phosphorus");
            _potassium = XPathHelper.GetOptNavValue<WeightValue>(navigator, "potassium");
            _zinc = XPathHelper.GetOptNavValue<WeightValue>(navigator, "zinc");
            _vitaminARAE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-A-RAE");
            _vitaminE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-E");
            _vitaminD = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-D");
            _vitaminC = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-C");
            _thiamin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "thiamin");
            _riboflavin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "riboflavin");
            _niacin = XPathHelper.GetOptNavValue<WeightValue>(navigator, "niacin");
            _vitaminB6 = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-B-6");
            _folateDFE = XPathHelper.GetOptNavValue<WeightValue>(navigator, "folate-DFE");
            _vitaminB12 = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-B-12");
            _vitaminK = XPathHelper.GetOptNavValue<WeightValue>(navigator, "vitamin-K");

            _additionalNutritionFacts.Clear();
            XPathNavigator additionalFactsNav = navigator.SelectSingleNode("additional-nutrition-facts");

            if (additionalFactsNav != null)
            {
                foreach (XPathNavigator nav in additionalFactsNav.Select("nutrition-fact"))
                {
                    NutritionFact nutritionFact = new NutritionFact();
                    nutritionFact.ParseXml(nav);
                    _additionalNutritionFacts.Add(nutritionFact);
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
            Validator.ThrowSerializationIfNull(_foodItem, "FoodItemNullValue");

            writer.WriteStartElement(nodeName);
            
            _foodItem.WriteXml("food-item", writer);
            XmlWriterHelper.WriteOpt(writer, "serving-size", _servingSize);
            XmlWriterHelper.WriteOptDouble(writer, "servings-consumed", _servingsConsumed);
            XmlWriterHelper.WriteOpt(writer, "meal", _meal);
            XmlWriterHelper.WriteOpt(writer, "when", _when);
            XmlWriterHelper.WriteOpt(writer, "energy", _energy);
            XmlWriterHelper.WriteOpt(writer, "energy-from-fat", _energyFromFat);
            XmlWriterHelper.WriteOpt(writer, "total-fat", _totalFat);
            XmlWriterHelper.WriteOpt(writer, "saturated-fat", _saturatedFat);
            XmlWriterHelper.WriteOpt(writer, "trans-fat", _transFat);
            XmlWriterHelper.WriteOpt(writer, "monounsaturated-fat", _monounsaturatedFat);
            XmlWriterHelper.WriteOpt(writer, "polyunsaturated-fat", _polyunsaturatedFat);
            XmlWriterHelper.WriteOpt(writer, "protein", _protein);
            XmlWriterHelper.WriteOpt(writer, "carbohydrates", _carbohydrates);
            XmlWriterHelper.WriteOpt(writer, "dietary-fiber", _dietaryFiber);
            XmlWriterHelper.WriteOpt(writer, "sugars", _sugars);
            XmlWriterHelper.WriteOpt(writer, "sodium", _sodium);
            XmlWriterHelper.WriteOpt(writer, "cholesterol", _cholesterol);
            XmlWriterHelper.WriteOpt(writer, "calcium", _calcium);
            XmlWriterHelper.WriteOpt(writer, "iron", _iron);
            XmlWriterHelper.WriteOpt(writer, "magnesium", _magnesium);
            XmlWriterHelper.WriteOpt(writer, "phosphorus", _phosphorus);
            XmlWriterHelper.WriteOpt(writer, "potassium", _potassium);
            XmlWriterHelper.WriteOpt(writer, "zinc", _zinc);
            XmlWriterHelper.WriteOpt(writer, "vitamin-A-RAE", _vitaminARAE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-E", _vitaminE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-D", _vitaminD);
            XmlWriterHelper.WriteOpt(writer, "vitamin-C", _vitaminC);
            XmlWriterHelper.WriteOpt(writer, "thiamin", _thiamin);
            XmlWriterHelper.WriteOpt(writer, "riboflavin", _riboflavin);
            XmlWriterHelper.WriteOpt(writer, "niacin", _niacin);
            XmlWriterHelper.WriteOpt(writer, "vitamin-B-6", _vitaminB6);
            XmlWriterHelper.WriteOpt(writer, "folate-DFE", _folateDFE);
            XmlWriterHelper.WriteOpt(writer, "vitamin-B-12", _vitaminB12);
            XmlWriterHelper.WriteOpt(writer, "vitamin-K", _vitaminK);

            if (_additionalNutritionFacts.Count != 0)
            {
                writer.WriteStartElement("additional-nutrition-facts");

                foreach (NutritionFact nutritionFact in _additionalNutritionFacts)
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
                return _foodItem;
            }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value", 
                        ResourceRetriever.GetResourceString("errors", "FoodItemNullValue"));
                }
                
                _foodItem = value;
            }
        }
        
        private CodableValue _foodItem;
        
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
                return _servingSize;
            }
            
            set
            {
                _servingSize = value;
            }
        }
        
        private CodableValue _servingSize;
        
        /// <summary>
        /// Gets or sets the number of servings consumed.
        /// </summary>
        /// 
        public double? ServingsConsumed
        {
            get
            {
                return _servingsConsumed;
            }
            
            set
            {
                _servingsConsumed = value;
            }
        }
        
        private double? _servingsConsumed;
        
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
                return _meal;
            }
            
            set
            {
                _meal = value;
            }
        }
        
        private CodableValue _meal;
        
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
                return _when;
            }
            
            set
            {
                _when = value;
            }
        }

        private HealthServiceDateTime _when;
        
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
                return _energy;
            }
            
            set
            {
                _energy = value;
            }
        }
        
        private FoodEnergyValue _energy;
        
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
                return _energyFromFat;
            }
            
            set
            {
                _energyFromFat = value;
            }
        }
        
        private FoodEnergyValue _energyFromFat;
        
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
                return _totalFat;
            }
            
            set
            {
                _totalFat = value;
            }
        }
        
        private WeightValue _totalFat;
        
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
                return _saturatedFat;
            }
            
            set
            {
                _saturatedFat = value;
            }
        }
        
        private WeightValue _saturatedFat;
        
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
                return _transFat;
            }
            
            set
            {
                _transFat = value;
            }
        }
        
        private WeightValue _transFat;
        
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
                return _monounsaturatedFat;
            }
            
            set
            {
                _monounsaturatedFat = value;
            }
        }
        
        private WeightValue _monounsaturatedFat;
        
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
                return _polyunsaturatedFat;
            }
            
            set
            {
                _polyunsaturatedFat = value;
            }
        }
        
        private WeightValue _polyunsaturatedFat;
        
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
                return _protein;
            }
            
            set
            {
                _protein = value;
            }
        }
        
        private WeightValue _protein;
        
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
                return _carbohydrates;
            }
            
            set
            {
                _carbohydrates = value;
            }
        }
        
        private WeightValue _carbohydrates;
        
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
                return _dietaryFiber;
            }
            
            set
            {
                _dietaryFiber = value;
            }
        }
        
        private WeightValue _dietaryFiber;
        
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
                return _sugars;
            }
            
            set
            {
                _sugars = value;
            }
        }
        
        private WeightValue _sugars;
        
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
                return _sodium;
            }
            
            set
            {
                _sodium = value;
            }
        }
        
        private WeightValue _sodium;
        
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
                return _cholesterol;
            }
            
            set
            {
                _cholesterol = value;
            }
        }
        
        private WeightValue _cholesterol;
        
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
                return _calcium;
            }
            
            set
            {
                _calcium = value;
            }
        }
        
        private WeightValue _calcium;
        
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
                return _iron;
            }
            
            set
            {
                _iron = value;
            }
        }
        
        private WeightValue _iron;
        
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
                return _magnesium;
            }
            
            set
            {
                _magnesium = value;
            }
        }
        
        private WeightValue _magnesium;
        
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
                return _phosphorus;
            }
            
            set
            {
                _phosphorus = value;
            }
        }
        
        private WeightValue _phosphorus;
        
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
                return _potassium;
            }
            
            set
            {
                _potassium = value;
            }
        }
        
        private WeightValue _potassium;
        
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
                return _zinc;
            }
            
            set
            {
                _zinc = value;
            }
        }
        
        private WeightValue _zinc;
        
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
                return _vitaminARAE;
            }
            
            set
            {
                _vitaminARAE = value;
            }
        }
        
        private WeightValue _vitaminARAE;
        
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
                return _vitaminE;
            }
            
            set
            {
                _vitaminE = value;
            }
        }
        
        private WeightValue _vitaminE;
        
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
                return _vitaminD;
            }
            
            set
            {
                _vitaminD = value;
            }
        }
        
        private WeightValue _vitaminD;
        
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
                return _vitaminC;
            }
            
            set
            {
                _vitaminC = value;
            }
        }
        
        private WeightValue _vitaminC;
        
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
                return _thiamin;
            }
            
            set
            {
                _thiamin = value;
            }
        }
        
        private WeightValue _thiamin;
        
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
                return _riboflavin;
            }
            
            set
            {
                _riboflavin = value;
            }
        }
        
        private WeightValue _riboflavin;
        
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
                return _niacin;
            }
            
            set
            {
                _niacin = value;
            }
        }
        
        private WeightValue _niacin;
        
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
                return _vitaminB6;
            }
            
            set
            {
                _vitaminB6 = value;
            }
        }
        
        private WeightValue _vitaminB6;
        
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
                return _folateDFE;
            }
            
            set
            {
                _folateDFE = value;
            }
        }
        
        private WeightValue _folateDFE;
        
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
                return _vitaminB12;
            }
            
            set
            {
                _vitaminB12 = value;
            }
        }
        
        private WeightValue _vitaminB12;
        
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
                return _vitaminK;
            }
            
            set
            {
                _vitaminK = value;
            }
        }
        
        private WeightValue _vitaminK;
        
        /// <summary>
        /// Gets the additional nutritional fact data.
        /// </summary>
        /// 
        /// <remarks>
        /// For instance, this could contain the amount of caffeine, or the amount of Omega-3 fatty acids consumed.
        /// If there is no information about additionalNutritionFacts the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public Collection<NutritionFact> AdditionalNutritionFacts
        {
            get
            {
                return _additionalNutritionFacts;
            }
        }

        private Collection<NutritionFact> _additionalNutritionFacts = new Collection<NutritionFact>();

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
            if (ServingSize != null)
            {
                if (ServingsConsumed.HasValue)
                {
                    return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("DietaryIntakeServingSizeServingConsumedFormat"),
                        FoodItem.Text,
                        ServingsConsumed.Value,
                        ServingSize.Text);
                }
                else
                {
                    return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("DietaryIntakeServingSizeFormat"),
                        FoodItem.Text,
                        ServingSize.Text);
                }
            }

            return FoodItem.Text;
        }
    }
}
