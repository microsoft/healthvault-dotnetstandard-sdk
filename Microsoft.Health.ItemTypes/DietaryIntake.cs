// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// The amount of dietary nutrients and minerals consumed.
    /// </summary>
    ///
    public class DietaryIntake : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DietaryIntake"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public DietaryIntake()
            : base(TypeId)
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="DietaryIntake"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
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
        public DietaryIntake(CodableValue foodItem)
        : base(TypeId)
        {
            FoodItem = foodItem;
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
            new Guid("089646a6-7e25-4495-ad15-3e28d4c1a71d");
        
        /// <summary>
        /// Populates this <see cref="DietaryIntake"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the DietaryIntake data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a DietaryIntake node.
        /// </exception>
        /// 
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            if (typeSpecificXml == null)
            {
                throw new ArgumentNullException(
                    "typeSpecificXml",
                    ResourceRetriever.GetResourceString(
                        "errors", "ParseXmlNavNull"));
            }
            
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("dietary-intake");
            
            if (itemNav == null)
            {
                throw new InvalidOperationException(
                    ResourceRetriever.GetResourceString(
                        "errors", "DietaryIntakeUnexpectedNode"));
            }

            _dietaryIntakeItem.ParseXml(itemNav);
        }
        
        /// <summary>
        /// Writes the XML representation of the DietaryIntake into
        /// the specified XML writer.
        /// </summary>
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
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(
                    "writer",
                    ResourceRetriever.GetResourceString(
                        "errors", "WriteXmlNullWriter"));
            }

            if (_dietaryIntakeItem.FoodItem == null)
            {
                throw new HealthRecordItemSerializationException(
                    ResourceRetriever.GetResourceString(
                        "errors", "FoodItemNullValue"));
            }

            _dietaryIntakeItem.WriteXml("dietary-intake", writer);
        }

        private DietaryIntakeItem _dietaryIntakeItem = new DietaryIntakeItem();
        
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
                return _dietaryIntakeItem.FoodItem;
            }
            
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value", 
                        ResourceRetriever.GetResourceString("errors", "FoodItemNullValue"));
                }

                _dietaryIntakeItem.FoodItem = value;
            }
        }
        
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
                return _dietaryIntakeItem.ServingSize;
            }
            
            set
            {
                _dietaryIntakeItem.ServingSize = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the number of servings consumed.
        /// </summary>
        /// 
        public double? ServingsConsumed
        {
            get
            {
                return _dietaryIntakeItem.ServingsConsumed;
            }
            
            set
            {
                _dietaryIntakeItem.ServingsConsumed = value;
            }
        }
        
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
                return _dietaryIntakeItem.Meal;
            }
            
            set
            {
                _dietaryIntakeItem.Meal = value;
            }
        }
        
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
                return _dietaryIntakeItem.When;
            }
            
            set
            {
                _dietaryIntakeItem.When = value;
            }
        }

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
                return _dietaryIntakeItem.Energy;
            }
            
            set
            {
                _dietaryIntakeItem.Energy = value;
            }
        }
        
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
                return _dietaryIntakeItem.EnergyFromFat;
            }
            
            set
            {
                _dietaryIntakeItem.EnergyFromFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.TotalFat;
            }
            
            set
            {
                _dietaryIntakeItem.TotalFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.SaturatedFat;
            }
            
            set
            {
                _dietaryIntakeItem.SaturatedFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.TransFat;
            }
            
            set
            {
                _dietaryIntakeItem.TransFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.MonounsaturatedFat;
            }
            
            set
            {
                _dietaryIntakeItem.MonounsaturatedFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.PolyunsaturatedFat;
            }
            
            set
            {
                _dietaryIntakeItem.PolyunsaturatedFat = value;
            }
        }
        
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
                return _dietaryIntakeItem.Protein;
            }
            
            set
            {
                _dietaryIntakeItem.Protein = value;
            }
        }
        
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
                return _dietaryIntakeItem.Carbohydrates;
            }
            
            set
            {
                _dietaryIntakeItem.Carbohydrates = value;
            }
        }
        
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
                return _dietaryIntakeItem.DietaryFiber;
            }
            
            set
            {
                _dietaryIntakeItem.DietaryFiber = value;
            }
        }
        
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
                return _dietaryIntakeItem.Sugars;
            }
            
            set
            {
                _dietaryIntakeItem.Sugars = value;
            }
        }
        
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
                return _dietaryIntakeItem.Sodium;
            }
            
            set
            {
                _dietaryIntakeItem.Sodium = value;
            }
        }
        
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
                return _dietaryIntakeItem.Cholesterol;
            }
            
            set
            {
                _dietaryIntakeItem.Cholesterol = value;
            }
        }
        
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
                return _dietaryIntakeItem.Calcium;
            }
            
            set
            {
                _dietaryIntakeItem.Calcium = value;
            }
        }
        
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
                return _dietaryIntakeItem.Iron;
            }
            
            set
            {
                _dietaryIntakeItem.Iron = value;
            }
        }
        
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
                return _dietaryIntakeItem.Magnesium;
            }
            
            set
            {
                _dietaryIntakeItem.Magnesium = value;
            }
        }
        
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
                return _dietaryIntakeItem.Phosphorus;
            }
            
            set
            {
                _dietaryIntakeItem.Phosphorus = value;
            }
        }
        
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
                return _dietaryIntakeItem.Potassium;
            }
            
            set
            {
                _dietaryIntakeItem.Potassium = value;
            }
        }
        
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
                return _dietaryIntakeItem.Zinc;
            }
            
            set
            {
                _dietaryIntakeItem.Zinc = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminARAE;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminARAE = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminE;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminE = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminD;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminD = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminC;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminC = value;
            }
        }
        
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
                return _dietaryIntakeItem.Thiamin;
            }
            
            set
            {
                _dietaryIntakeItem.Thiamin = value;
            }
        }
        
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
                return _dietaryIntakeItem.Riboflavin;
            }
            
            set
            {
                _dietaryIntakeItem.Riboflavin = value;
            }
        }
        
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
                return _dietaryIntakeItem.Niacin;
            }
            
            set
            {
                _dietaryIntakeItem.Niacin = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminB6;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminB6 = value;
            }
        }
        
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
                return _dietaryIntakeItem.FolateDFE;
            }
            
            set
            {
                _dietaryIntakeItem.FolateDFE = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminB12;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminB12 = value;
            }
        }
        
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
                return _dietaryIntakeItem.VitaminK;
            }
            
            set
            {
                _dietaryIntakeItem.VitaminK = value;
            }
        }
        
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
                return _dietaryIntakeItem.AdditionalNutritionFacts;
            }
        }

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
            if (_dietaryIntakeItem != null)
            {
                return _dietaryIntakeItem.ToString();
            }

            return String.Empty;
        }
    }
}
