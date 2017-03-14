// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a weight goal.
    /// </summary>
    ///
    public class WeightGoal : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WeightGoal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(ThingBase)"/> method
        /// is called.
        /// </remarks>
        ///
        public WeightGoal()
            : base(TypeId)
        {
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
            new Guid("b7925180-d69e-48fa-ae1d-cb3748ca170e");

        /// <summary>
        /// Populates this <see cref="WeightGoal"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the weight goal data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a weight-goal node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator weightGoalNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("weight-goal");

            Validator.ThrowInvalidIfNull(weightGoalNav, "WeightGoalUnexpectedNode");

            XPathNavigator initialNav =
                weightGoalNav.SelectSingleNode("initial");

            if (initialNav != null)
            {
                this.initialWeight = new WeightValue();
                this.initialWeight.ParseXml(initialNav);
            }

            XPathNavigator minNav =
                weightGoalNav.SelectSingleNode("minimum");

            if (minNav != null)
            {
                this.minWeight = new WeightValue();
                this.minWeight.ParseXml(minNav);
            }

            XPathNavigator maxNav =
                weightGoalNav.SelectSingleNode("maximum");

            if (maxNav != null)
            {
                this.maxWeight = new WeightValue();
                this.maxWeight.ParseXml(maxNav);
            }

            XPathNavigator goalNav =
                weightGoalNav.SelectSingleNode("goal-info");

            if (goalNav != null)
            {
                this.goal = new Goal();
                this.goal.ParseXml(goalNav);
            }
        }

        /// <summary>
        /// Writes the weight goal data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the weight goal data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            // <weight-goal>
            writer.WriteStartElement("weight-goal");

            if (this.initialWeight != null)
            {
                this.initialWeight.WriteXml("initial", writer);
            }

            if (this.minWeight != null)
            {
                this.minWeight.WriteXml("minimum", writer);
            }

            if (this.maxWeight != null)
            {
                this.maxWeight.WriteXml("maximum", writer);
            }

            if (this.goal != null)
            {
                this.goal.WriteXml("goal-info", writer);
            }

            // </weight-goal>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the person's initial weight.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="WeightValue"/> representing the initial weight.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the initial weight should not be stored.
        /// </remarks>
        ///
        public WeightValue InitialWeight
        {
            get { return this.initialWeight; }
            set { this.initialWeight = value; }
        }

        private WeightValue initialWeight;

        /// <summary>
        /// Gets or sets the person's minimum weight.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="WeightValue"/> representing the minimum weight.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the minimum weight should not be stored.
        /// </remarks>
        ///
        public WeightValue MinimumWeight
        {
            get { return this.minWeight; }
            set { this.minWeight = value; }
        }

        private WeightValue minWeight;

        /// <summary>
        /// Gets or sets the person's maximum weight.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="WeightValue"/> representing the maximum weight.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the maximum weight should not be stored.
        /// </remarks>
        ///
        public WeightValue MaximumWeight
        {
            get { return this.maxWeight; }
            set { this.maxWeight = value; }
        }

        private WeightValue maxWeight;

        /// <summary>
        /// Gets or sets the goal information.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="Goal"/> value containing the information.
        /// </value>
        ///
        /// <remarks>
        /// Set the value to <b>null</b> if the goal information should not be stored.
        /// </remarks>
        ///
        public Goal Goal
        {
            get { return this.goal; }
            set { this.goal = value; }
        }

        private Goal goal;

        /// <summary>
        /// Gets a string representation of the weight goal.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the value of the weight goal.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = string.Empty;

            if (this.Goal == null || this.Goal.TargetDate == null)
            {
                if (this.MinimumWeight != null)
                {
                    if (this.MaximumWeight != null)
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "MeasurementRange"),
                                this.MinimumWeight.ToString(),
                                this.MaximumWeight.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMin"),
                                this.MinimumWeight.ToString());
                    }
                }
                else
                {
                    if (this.MaximumWeight != null)
                    {
                        result = this.MaximumWeight.ToString();
                    }
                }
            }
            else
            {
                if (this.MinimumWeight != null)
                {
                    if (this.MaximumWeight != null)
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "MeasurementRangeWithDate"),
                                this.MinimumWeight.ToString(),
                                this.MaximumWeight.ToString(),
                                this.Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMinWithDate"),
                                this.MinimumWeight.ToString(),
                                this.Goal.TargetDate.ToString());
                    }
                }
                else
                {
                    if (this.MaximumWeight != null)
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMaxWithDate"),
                                this.MaximumWeight.ToString(),
                                this.Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMaxWithDate"),
                                string.Empty,
                                this.Goal.TargetDate.ToString());
                    }
                }
            }

            return result;
        }
    }
}
