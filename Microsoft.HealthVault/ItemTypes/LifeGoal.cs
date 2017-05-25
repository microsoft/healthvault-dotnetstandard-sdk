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
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a life goal.
    /// </summary>
    ///
    public class LifeGoal : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LifeGoal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public LifeGoal()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LifeGoal"/> class with the
        /// specified description.
        /// </summary>
        ///
        /// <param name="description">
        /// The description that defines the life goal.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="description"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        public LifeGoal(string description)
            : base(TypeId)
        {
            Description = description;
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
            new Guid("609319bf-35cc-40a4-b9d7-1b329679baaa");

        /// <summary>
        /// Populates this <see cref="LifeGoal"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the life goal data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// a life-goal node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator lifeGoalNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("life-goal");

            Validator.ThrowInvalidIfNull(lifeGoalNav, Resources.LifeGoalUnexpectedNode);

            _description = lifeGoalNav.SelectSingleNode("description").Value;

            XPathNavigator goalNav =
                lifeGoalNav.SelectSingleNode("goal-info");

            if (goalNav != null)
            {
                _goal = new Goal();
                _goal.ParseXml(goalNav);
            }
        }

        /// <summary>
        /// Writes the life goal data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the life goal data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// The <see cref="Description"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_description, Resources.LifeGoalDescriptionNotSet);

            // <life-goal>
            writer.WriteStartElement("life-goal");

            writer.WriteElementString("description", _description);

            if (_goal != null)
            {
                _goal.WriteXml("goal-info", writer);
            }

            // </life-goal>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the description of the goal.
        /// </summary>
        ///
        /// <value>
        /// A string representing the goal description.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null</b>, empty, or contains only
        /// whitespace during set.
        /// </exception>
        ///
        public string Description
        {
            get { return _description; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Description");
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }

        private string _description;

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
        /// Gets a string representation of the life goal.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the life goal.
        /// </returns>
        ///
        public override string ToString()
        {
            string result = Description;

            if (Goal != null && Goal.TargetDate != null)
            {
                result =
                    string.Format(
                        Resources.LifeGoalToStringFormat,
                        Description,
                        Goal.TargetDate.ToString());
            }

            return result;
        }
    }
}
