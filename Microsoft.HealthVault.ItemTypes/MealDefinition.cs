// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Information related to a meal.
    /// </summary>
    public class MealDefinition : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="MealDefinition"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public MealDefinition()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MealDefinition"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        /// <param name="name">
        /// Name of the meal.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public MealDefinition(CodableValue name)
        : base(TypeId)
        {
            this.Name = name;
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
            new Guid("074e122a-335a-4a47-a63d-00a8f3e79e60");

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
        /// a MealDefinition node.
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
                typeSpecificXml.CreateNavigator().SelectSingleNode("meal-definition");

            if (itemNav == null)
            {
                throw new InvalidOperationException(
                    ResourceRetriever.GetResourceString(
                        "errors", "MealDefinitionUnexpectedNode"));
            }

            this.name = new CodableValue();
            this.name.ParseXml(itemNav.SelectSingleNode("name"));

            this.mealType = new CodableValue();
            this.mealType = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "meal-type");

            XPathNavigator descriptionValueNav = itemNav.SelectSingleNode("description");
            if (descriptionValueNav != null)
            {
                this.description = descriptionValueNav.Value;
            }

            this.dietaryIntakeItems = XPathHelper.ParseXmlCollection<DietaryIntakeItem>(itemNav, "dietary-items/dietary-item");
        }

        /// <summary>
        /// Writes the XML representation of the MealDefinition into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the MealDefinition should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(this.name, "MealDefinitionNameNullValue");

            writer.WriteStartElement("meal-definition");
            this.name.WriteXml("name", writer);
            XmlWriterHelper.WriteOpt(writer, "meal-type", this.mealType);
            XmlWriterHelper.WriteOptString(writer, "description", this.description);
            XmlWriterHelper.WriteXmlCollection(writer, "dietary-items", this.dietaryIntakeItems, "dietary-item");
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the name of a meal definition.
        /// </summary>
        ///
        /// <remarks>
        /// This can refer to a user-defined name or specific meals from standard meal plans.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value",
                        ResourceRetriever.GetResourceString("errors", "MealDefinitionNameNullValue"));
                }

                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Represents the type of meal, such as breakfast or lunch.
        /// </summary>
        /// <remarks>
        /// If there is no information about meal the value should be set to <b>null</b>.
        /// </remarks>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue MealType
        {
            get
            {
                return this.mealType;
            }

            set
            {
                this.mealType = value;
            }
        }

        private CodableValue mealType;

        /// <summary>
        /// Textual description of the meal.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets the collection of Dietary Intake items included in the meal definition.
        /// </summary>
        /// <remarks>
        /// This section does not contain consumption information. To record meal consumption, an application should store DietaryIntake on the record for each item included in this section.
        /// </remarks>
        public Collection<DietaryIntakeItem> DietaryIntakeItems => this.dietaryIntakeItems;

        private Collection<DietaryIntakeItem> dietaryIntakeItems =
            new Collection<DietaryIntakeItem>();

        /// <summary>
        /// Gets a string representation of the Meal Definition.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the Meal Definition.
        /// </returns>
        ///
        public override string ToString()
        {
            return this.Name.Text;
        }
    }
}