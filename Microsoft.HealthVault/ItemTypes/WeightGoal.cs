// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
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
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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

            Validator.ThrowInvalidIfNull(weightGoalNav, Resources.WeightGoalUnexpectedNode);

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
            string result = string.Empty;

            if (Goal == null || Goal.TargetDate == null)
            {
                if (MinimumWeight != null)
                {
                    if (MaximumWeight != null)
                    {
                        result =
                            string.Format(
                                Resources.MeasurementRange,
                                MinimumWeight.ToString(),
                                MaximumWeight.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                Resources.WeightGoalToStringFormatMin,
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
                            string.Format(
                                Resources.MeasurementRangeWithDate,
                                MinimumWeight.ToString(),
                                MaximumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                Resources.WeightGoalToStringFormatMinWithDate,
                                MinimumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                }
                else
                {
                    if (MaximumWeight != null)
                    {
                        result =
                            string.Format(
                                Resources.WeightGoalToStringFormatMaxWithDate,
                                MaximumWeight.ToString(),
                                Goal.TargetDate.ToString());
                    }
                    else
                    {
                        result =
                            string.Format(
                                Resources.WeightGoalToStringFormatMaxWithDate,
                                string.Empty,
                                Goal.TargetDate.ToString());
                    }
                }
            }

            return result;
        }
    }
}
