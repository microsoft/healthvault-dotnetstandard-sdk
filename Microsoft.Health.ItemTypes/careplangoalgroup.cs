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
    /// Goal groups are used to group related measurement goals together.
    /// </summary>
    /// <remarks>
    /// For example, blood pressure has two individual measurement goals (systolic and diastolic) but are grouped 
    /// together under blood pressure.
    /// </remarks>
    ///
    public class CarePlanGoalGroup : HealthRecordItemData
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
            Name = name;

            Validator.ThrowArgumentExceptionIf(goals == null, "goals", "CarePlanGoalGroupGroupsNull");
            Validator.ThrowArgumentExceptionIf(goals.Count == 0, "goals", "CarePlanGoalGroupGroupsNull");

            _goals = goals;
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

            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));
            _description = XPathHelper.GetOptNavValue(navigator, "description");
            _goals = XPathHelper.ParseXmlCollection<CarePlanGoal>(navigator, "goals/goal");
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

            Validator.ThrowSerializationIfNull(_name, "CarePlanGoalGroupNameNull");
            Validator.ThrowSerializationIfNull(_goals, "CarePlanGoalGroupGroupsNull");
            Validator.ThrowSerializationIf(_goals.Count == 0, "CarePlanGoalGroupGroupsEmpty");

            writer.WriteStartElement("goal-group");
            {
                _name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", _description);

                XmlWriterHelper.WriteXmlCollection<CarePlanGoal>(writer, "goals", _goals, "goal");
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
                return _name;
            }
            
            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "CarePlanGoalGroupNameNull");
                _name = value;
            }
        }
        
        private CodableValue _name;
        
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
                return _description;
            }
            
            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Description");
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Description");

                _description = value;
            }
        }
        
        private string _description;
        
        /// <summary>
        /// Gets or sets list of care plan goals associated with this goal group.
        /// </summary>
        /// 
        public Collection<CarePlanGoal> Goals
        {
            get
            {
                return _goals;
            }
        }

        private Collection<CarePlanGoal> _goals =
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
            if (_name != null)
            {
                if (_description == null)
                {
                    return _name.Text;
                }
                else
                {
                    return String.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("CarePlanGoalGroupFormat"),
                        _name.Text,
                        _description);
                }
            }
            else
            {
                string listSeparator = ResourceRetriever.GetResourceString("ListSeparator");

                List<string> goalStrings = new List<string>();

                foreach (CarePlanGoal goal in _goals)
                {
                    goalStrings.Add(goal.ToString());
                }

                return String.Join(listSeparator, goalStrings.ToArray());
            }
        }
    }
}
