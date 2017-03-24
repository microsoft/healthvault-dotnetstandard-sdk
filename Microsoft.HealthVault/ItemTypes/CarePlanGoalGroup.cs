// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Goal groups are used to group related measurement goals together.
    /// </summary>
    /// <remarks>
    /// For example, blood pressure has two individual measurement goals (systolic and diastolic) but are grouped
    /// together under blood pressure.
    /// </remarks>
    ///
    public class CarePlanGoalGroup : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoalGroup"/> class with default values.
        /// </summary>
        ///
        public CarePlanGoalGroup()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoalGroup"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// Name of the goal group.
        /// </param>
        /// <param name="goals">
        /// List of care plan goals associated with this goal group.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// If <paramref name="goals"/> is <b>null</b>.
        /// </exception>
        ///
        public CarePlanGoalGroup(
            CodableValue name,
            Collection<CarePlanGoal> goals)
        {
            this.Name = name;

            if (goals == null)
            {
                throw new ArgumentException(Resources.CarePlanGoalGroupGroupsNull, nameof(goals));
            }

            if (goals.Count == 0)
            {
                throw new ArgumentException(Resources.CarePlanGoalGroupGroupsEmpty, nameof(goals));
            }

            this.goals = goals;
        }

        /// <summary>
        /// Populates this <see cref="CarePlanGoalGroup"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the CarePlanGoalGroup data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            this.name = new CodableValue();
            this.name.ParseXml(navigator.SelectSingleNode("name"));
            this.description = XPathHelper.GetOptNavValue(navigator, "description");
            this.goals = XPathHelper.ParseXmlCollection<CarePlanGoal>(navigator, "goals/goal");
        }

        /// <summary>
        /// Writes the XML representation of the CarePlanGoalGroup into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlanGoalGroup should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// If <see cref="Goals"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIfNull(this.name, Resources.CarePlanGoalGroupNameNull);
            Validator.ThrowSerializationIfNull(this.goals, Resources.CarePlanGoalGroupGroupsNull);

            if (this.goals.Count == 0)
            {
                throw new ThingSerializationException(Resources.CarePlanGoalGroupGroupsEmpty);
            }

            writer.WriteStartElement("goal-group");
            {
                this.name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", this.description);

                XmlWriterHelper.WriteXmlCollection(writer, "goals", this.goals, "goal");
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets name of the goal group.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about name the value should be set to <b>null</b>.
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
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.CarePlanGoalGroupNameNull);
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets description of the goal group.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about description the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Description");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Description");

                this.description = value;
            }
        }

        private string description;

        /// <summary>
        /// Gets or sets list of care plan goals associated with this goal group.
        /// </summary>
        ///
        public Collection<CarePlanGoal> Goals => this.goals;

        private Collection<CarePlanGoal> goals =
            new Collection<CarePlanGoal>();

        /// <summary>
        /// Gets a string representation of the CarePlanGoalGroup.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CarePlanGoalGroup.
        /// </returns>
        ///
        public override string ToString()
        {
            if (this.name != null)
            {
                if (this.description == null)
                {
                    return this.name.Text;
                }

                return string.Format(
                    CultureInfo.CurrentUICulture,
                    Resources.CarePlanGoalGroupFormat,
                    this.name.Text,
                    this.description);
            }

            string listSeparator = Resources.ListSeparator;

            List<string> goalStrings = new List<string>();

            foreach (CarePlanGoal goal in this.goals)
            {
                goalStrings.Add(goal.ToString());
            }

            return string.Join(listSeparator, goalStrings.ToArray());
        }
    }
}
