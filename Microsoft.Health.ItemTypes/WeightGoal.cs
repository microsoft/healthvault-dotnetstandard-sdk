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
    /// Represents a health record item type that encapsulates a weight goal.
    /// </summary>
    /// 
    public class WeightGoal : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WeightGoal"/> class with default 
        /// values.
        /// </summary>
        /// 
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="Microsoft.Health.HealthRecordAccessor.NewItem(HealthRecordItem)"/> method 
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
        public new static readonly Guid TypeId =
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
                _initialWeight = new WeightValue();
                _initialWeight.ParseXml(initialNav);
            }

            XPathNavigator minNav =
                weightGoalNav.SelectSingleNode("minimum");

            if (minNav != null)
            {
                _minWeight = new WeightValue();
                _minWeight.ParseXml(minNav);
            }

            XPathNavigator maxNav =
                weightGoalNav.SelectSingleNode("maximum");

            if (maxNav != null)
            {
                _maxWeight = new WeightValue();
                _maxWeight.ParseXml(maxNav);
            }


            XPathNavigator goalNav =
                weightGoalNav.SelectSingleNode("goal-info");

            if (goalNav != null)
            {
                _goal = new Goal();
                _goal.ParseXml(goalNav);
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

            if (_initialWeight != null)
            {
                _initialWeight.WriteXml("initial", writer);
            }

            if (_minWeight != null)
            {
                _minWeight.WriteXml("minimum", writer);
            }

            if (_maxWeight != null)
            {
                _maxWeight.WriteXml("maximum", writer);
            }

            if (_goal != null)
            {
                _goal.WriteXml("goal-info", writer);
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
            get { return _initialWeight; }
            set { _initialWeight = value; }
        }
        private WeightValue _initialWeight;


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
            get { return _minWeight; }
            set { _minWeight = value; }
        }
        private WeightValue _minWeight;


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
            get { return _maxWeight; }
            set { _maxWeight = value; }
        }
        private WeightValue _maxWeight;

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
            get { return _goal; }
            set { _goal = value; }
        }
        private Goal _goal;

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
            string result = String.Empty;

            if (Goal == null || Goal.TargetDate == null)
            {
                if (MinimumWeight != null)
                {
                    if (MaximumWeight != null)
                    {
                        result =
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "MeasurementRange"),
                                MinimumWeight.ToString(),
                                MaximumWeight.ToString());
                    }
                    else
                    {
                        result =
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMin"),
                                MinimumWeight.ToString());
                    }
                }
                else
                {
                    if (MaximumWeight != null)
                    {
                        result = MaximumWeight.ToString();
                    }
                }
            }
            else
            {
                if (MinimumWeight != null)
                {
                    if (MaximumWeight != null)
                    {
                        result = 
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "MeasurementRangeWithDate"),
                                MinimumWeight.ToString(),
                                MaximumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result = 
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMinWithDate"),
                                MinimumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                }
                else
                {
                    if (MaximumWeight != null)
                    {
                        result =
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMaxWithDate"),
                                MaximumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result =
                            String.Format(
                                ResourceRetriever.GetResourceString(
                                    "WeightGoalToStringFormatMaxWithDate"),
                                String.Empty,
                                Goal.TargetDate.ToString());
                    }
                }
            }
            return result;
        }
    }

}
