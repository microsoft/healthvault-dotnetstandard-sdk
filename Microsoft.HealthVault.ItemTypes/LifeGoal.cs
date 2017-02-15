// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a life goal.
    /// </summary>
    ///
    public class LifeGoal : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LifeGoal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
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
            this.Description = description;
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

            Validator.ThrowInvalidIfNull(lifeGoalNav, "LifeGoalUnexpectedNode");

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
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="Description"/> property has not been set.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_description, "LifeGoalDescriptionNotSet");

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
                        ResourceRetriever.GetResourceString(
                            "LifeGoalToStringFormat"),
                        Description,
                        Goal.TargetDate.ToString());
            }
            return result;
        }
    }
}
