// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Exceptions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A health goal defines a target for a measurement or action to be performed by a user.
    /// </summary>
    ///
    /// <remarks>
    /// Example goals: maintain average blood glucose level below 90 mg/dl, walk 1000 steps per day.
    /// </remarks>
    ///
    public class HealthGoal : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthGoal"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItem(HealthRecordItem)"/> method
        /// is called.
        /// </remarks>
        ///
        public HealthGoal()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthGoal"/> class
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
        /// Name of the goal.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public HealthGoal(CodableValue name)
        : base(TypeId)
        {
            Name = name;
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
            new Guid("dad8bb47-9ad0-4f09-a020-0ff051d1d0f7");

        /// <summary>
        /// Populates this <see cref="HealthGoal"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the HealthGoal data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a HealthGoal node.
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
                typeSpecificXml.CreateNavigator().SelectSingleNode("health-goal");

            if (itemNav == null)
            {
                throw new InvalidOperationException(
                    ResourceRetriever.GetResourceString(
                        "errors", "HealthGoalUnexpectedNode"));
            }

            _name = new CodableValue();
            _name.ParseXml(itemNav.SelectSingleNode("name"));
            _description = XPathHelper.GetOptNavValue(itemNav, "description");
            _startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "start-date");
            _endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(itemNav, "end-date");
            _associatedTypeInfo = XPathHelper.GetOptNavValue<AssociatedTypeInfo>(itemNav, "associated-type-info");
            _targetRange = XPathHelper.GetOptNavValue<GoalRange>(itemNav, "target-range");

            _goalAdditionalRanges.Clear();
            foreach (XPathNavigator nav in itemNav.Select("goal-additional-ranges"))
            {
                GoalRange goalRange = new GoalRange();
                goalRange.ParseXml(nav);
                _goalAdditionalRanges.Add(goalRange);
            }

            _recurrence = XPathHelper.GetOptNavValue<GoalRecurrence>(itemNav, "recurrence");
        }

        /// <summary>
        /// Writes the XML representation of the HealthGoal into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the HealthGoal should be
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
            if (writer == null)
            {
                throw new ArgumentNullException(
                    "writer",
                    ResourceRetriever.GetResourceString(
                        "errors", "WriteXmlNullWriter"));
            }

            if (_name == null)
            {
                throw new HealthRecordItemSerializationException(
                    ResourceRetriever.GetResourceString(
                        "errors", "GoalNameNullValue"));
            }

            writer.WriteStartElement("health-goal");

            _name.WriteXml("name", writer);
            XmlWriterHelper.WriteOptString(writer, "description", _description);
            XmlWriterHelper.WriteOpt(writer, "start-date", _startDate);
            XmlWriterHelper.WriteOpt(writer, "end-date", _endDate);
            XmlWriterHelper.WriteOpt(writer, "associated-type-info", _associatedTypeInfo);
            XmlWriterHelper.WriteOpt(writer, "target-range", _targetRange);

            if (_goalAdditionalRanges != null && _goalAdditionalRanges.Count != 0)
            {
                foreach (GoalRange goalRange in _goalAdditionalRanges)
                {
                    goalRange.WriteXml("goal-additional-ranges", writer);
                }
            }

            XmlWriterHelper.WriteOpt(writer, "recurrence", _recurrence);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets name of the goal.
        /// </summary>
        ///
        /// <remarks>
        /// Example: average blood glucose.
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
                if (value == null)
                {
                    throw new ArgumentNullException(
                        "value",
                        ResourceRetriever.GetResourceString("errors", "GoalNameNullValue"));
                }

                _name = value;
            }
        }

        private CodableValue _name;

        /// <summary>
        /// Gets or sets description of the goal.
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
                if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
                {
                    throw new ArgumentException(
                        ResourceRetriever.GetResourceString("errors", "WhitespaceOnlyValue"), "value");
                }

                _description = value;
            }
        }

        private string _description;

        /// <summary>
        /// Gets or sets the start date of the goal.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about startDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime StartDate
        {
            get
            {
                return _startDate;
            }

            set
            {
                _startDate = value;
            }
        }

        private ApproximateDateTime _startDate;

        /// <summary>
        /// Gets or sets the end date of the goal.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about endDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime EndDate
        {
            get
            {
                return _endDate;
            }

            set
            {
                _endDate = value;
            }
        }

        private ApproximateDateTime _endDate;

        /// <summary>
        /// Gets or sets specifies HealthVault type information related to this goal.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about associatedTypeInfo the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public AssociatedTypeInfo AssociatedTypeInfo
        {
            get
            {
                return _associatedTypeInfo;
            }

            set
            {
                _associatedTypeInfo = value;
            }
        }

        private AssociatedTypeInfo _associatedTypeInfo;

        /// <summary>
        /// Gets or sets the target range for the goal.
        /// </summary>
        ///
        /// <remarks>
        /// This represents the ideal range for a goal, for example, the ideal weight, or the ideal blood pressure.
        /// If there is no information about targetRange the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public GoalRange TargetRange
        {
            get
            {
                return _targetRange;
            }

            set
            {
                _targetRange = value;
            }
        }

        private GoalRange _targetRange;

        /// <summary>
        /// Gets allows specifying additional ranges for the goal.
        /// </summary>
        ///
        /// <remarks>
        /// For example, in a blood pressure goal, it may be useful to include the 'hypertensive' range in addition to the ideal range.
        /// If there is no information about goalAdditionalRanges the collection should be empty.
        /// </remarks>
        ///
        public Collection<GoalRange> GoalAdditionalRanges
        {
            get { return _goalAdditionalRanges; }
        }

        private Collection<GoalRange> _goalAdditionalRanges = new Collection<GoalRange>();

        /// <summary>
        /// Gets or sets this field allows defining recurrence for goals.
        /// </summary>
        ///
        /// <remarks>
        /// A goal can be defined on a weekly interval, meaning the target is intended to be achieved every week. Walking 70000 steps in a week is an example of this.
        /// If there is no information about recurrence the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public GoalRecurrence Recurrence
        {
            get
            {
                return _recurrence;
            }

            set
            {
                _recurrence = value;
            }
        }

        private GoalRecurrence _recurrence;

        /// <summary>
        /// Gets a string representation of the HealthGoal.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the HealthGoal.
        /// </returns>
        ///
        public override string ToString()
        {
            if (TargetRange != null && Recurrence != null)
            {
                return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("HealthGoalWithTargetRangeAndRecurrenceFormat"),
                        Name.Text,
                        TargetRange.ToString(),
                        Recurrence.ToString());
            }

            if (TargetRange != null)
            {
                return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("HealthGoalFormat"),
                        Name.Text,
                        TargetRange.ToString());
            }

            if (Recurrence != null)
            {
                return string.Format(
                        CultureInfo.CurrentUICulture,
                        ResourceRetriever.GetResourceString("HealthGoalFormat"),
                        Name.Text,
                        Recurrence.ToString());
            }

            return Name.Text;
        }
    }
}
