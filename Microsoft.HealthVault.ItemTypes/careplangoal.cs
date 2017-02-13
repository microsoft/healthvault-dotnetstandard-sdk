// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A goal defines a target for a measurement.
    /// </summary>
    ///
    public class CarePlanGoal : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoal"/> class with default values.
        /// </summary>
        ///
        public CarePlanGoal()
        {
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanGoal"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// Name of the goal.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public CarePlanGoal(CodableValue name)
        {
            Name = name;
        }
        
        /// <summary>
        /// Populates this <see cref="CarePlanGoal"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the CarePlanGoal data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _name = XPathHelper.GetOptNavValue<CodableValue>(navigator, "name");
            _description = XPathHelper.GetOptNavValue(navigator, "description");
            _startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "start-date");
            _endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "end-date");
            _targetCompletionDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "target-completion-date");
            _goalAssociatedTypeInfo = XPathHelper.GetOptNavValue<AssociatedTypeInfo>(navigator, "associated-type-info");
            _targetRange = XPathHelper.GetOptNavValue<GoalRange>(navigator, "target-range");

            _goalAdditionalRanges.Clear();
            foreach (XPathNavigator nav in navigator.Select("goal-additional-ranges"))
            {
                GoalRange goalRange = new GoalRange();
                goalRange.ParseXml(nav);
                _goalAdditionalRanges.Add(goalRange);
            }

            _recurrence = XPathHelper.GetOptNavValue<GoalRecurrence>(navigator, "recurrence");
            _referenceId = XPathHelper.GetOptNavValue(navigator, "reference-id");
        }
        
        /// <summary>
        /// Writes the XML representation of the CarePlanGoal into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlanGoal should be
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
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");

            Validator.ThrowSerializationIfNull(_name, "CarePlanGoalNameNull");

            writer.WriteStartElement("goal");
            {
                _name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", _description);
                XmlWriterHelper.WriteOpt(writer, "start-date", _startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", _endDate);
                XmlWriterHelper.WriteOpt(writer, "target-completion-date", _targetCompletionDate);
                XmlWriterHelper.WriteOpt(writer, "associated-type-info", _goalAssociatedTypeInfo);
                XmlWriterHelper.WriteOpt(writer, "target-range", _targetRange);

                if (_goalAdditionalRanges != null && _goalAdditionalRanges.Count != 0)
                {
                    foreach (GoalRange goalRange in _goalAdditionalRanges)
                    {
                        goalRange.WriteXml("goal-additional-ranges", writer);
                    }
                }

                XmlWriterHelper.WriteOpt(writer, "recurrence", _recurrence);
                XmlWriterHelper.WriteOptString(writer, "reference-id", _referenceId);
            }

            writer.WriteEndElement();
        }
        
        /// <summary>
        /// Gets or sets name of the goal.
        /// </summary>
        /// 
        /// <remarks>
        /// Example: average blood-glucose for the last seven days
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
                Validator.ThrowIfArgumentNull(value, "Name", "CarePlanGoalNameNull");
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
        /// The <paramref name="value"/> is empty or contains only whitespace.
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
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Description");
                _description = value;
            }
        }
        
        private string _description;

        private static void ValidateDates(
            ApproximateDateTime startDate,
            ApproximateDateTime endDate)
        {
            if (startDate != null && endDate != null)
            {
                if (startDate.ApproximateDate != null && endDate.ApproximateDate != null)
                {
                    Validator.ThrowArgumentExceptionIf(
                        startDate.CompareTo(endDate) > 0,
                        "StartDate and EndDate",
                        "CarePlanGoalDateInvalid");
                }
            }
        }        

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
                ValidateDates(value, _endDate);

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
                ValidateDates(_startDate, value);

                _endDate = value;
            }
        }
        
        private ApproximateDateTime _endDate;

        /// <summary>
        /// Gets or sets the date user intends to complete the goal.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about targetCompletionDate the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public ApproximateDateTime TargetCompletionDate
        {
            get
            {
                return _targetCompletionDate;
            }

            set
            {
                _targetCompletionDate = value;
            }
        }

        private ApproximateDateTime _targetCompletionDate;

        /// <summary>
        /// Gets or sets HealthVault type information related to this goal.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about GoalAssociatedTypeInfo the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public AssociatedTypeInfo GoalAssociatedTypeInfo
        {
            get
            {
                return _goalAssociatedTypeInfo;
            }

            set
            {
                _goalAssociatedTypeInfo = value;
            }
        }

        private AssociatedTypeInfo _goalAssociatedTypeInfo;

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
        /// Gets or sets additional ranges for the goal.
        /// </summary>
        /// 
        /// <remarks>
        /// For example, in a blood pressure goal, it may be useful to include the 'hypertensive' range in addition to the ideal range.
        /// If there is no information about goalAdditionalRanges the collection should be empty.
        /// </remarks>
        ///
        public Collection<GoalRange> GoalAdditionalRanges => _goalAdditionalRanges;

        private readonly Collection<GoalRange> _goalAdditionalRanges = new Collection<GoalRange>();

        /// <summary>
        /// Gets or sets recurrence for goals.
        /// </summary>
        /// 
        /// <remarks>
        /// A goal can be defined to be achieved every specific interval. Example, walking 70000 steps in a week.
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
        /// Gets or sets an unique id to distinguish one goal from another.
        /// </summary>
        /// 
        /// <remarks>
        /// If there is no information about referenceId the value should be set to <b>null</b>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> contains only whitespace.
        /// </exception>
        /// 
        public string ReferenceId
        {
            get
            {
                return _referenceId;
            }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "ReferenceId");
                _referenceId = value;
            }
        }

        private string _referenceId;

        /// <summary>
        /// Gets a string representation of the CarePlanGoal.
        /// </summary>
        /// 
        /// <returns>
        /// A string representation of the CarePlanGoal.
        /// </returns>
        ///
        public override string ToString()
        {
            string result;

            if (_description == null)
            {
                result = _name.Text;
            }
            else
            {
                result = string.Format(
                    CultureInfo.CurrentUICulture,
                    ResourceRetriever.GetResourceString("CarePlanGoalFormat"),
                    _name.Text,
                    _description);
            }

            return result;
        }
    }
}
