// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A goal defines a target for a measurement.
    /// </summary>
    ///
    public class CarePlanGoal : ItemBase
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
            this.Name = name;
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

            this.name = XPathHelper.GetOptNavValue<CodableValue>(navigator, "name");
            this.description = XPathHelper.GetOptNavValue(navigator, "description");
            this.startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "start-date");
            this.endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "end-date");
            this.targetCompletionDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "target-completion-date");
            this.goalAssociatedTypeInfo = XPathHelper.GetOptNavValue<AssociatedTypeInfo>(navigator, "associated-type-info");
            this.targetRange = XPathHelper.GetOptNavValue<GoalRange>(navigator, "target-range");

            this.goalAdditionalRanges.Clear();
            foreach (XPathNavigator nav in navigator.Select("goal-additional-ranges"))
            {
                GoalRange goalRange = new GoalRange();
                goalRange.ParseXml(nav);
                this.goalAdditionalRanges.Add(goalRange);
            }

            this.recurrence = XPathHelper.GetOptNavValue<GoalRecurrence>(navigator, "recurrence");
            this.referenceId = XPathHelper.GetOptNavValue(navigator, "reference-id");
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

            Validator.ThrowSerializationIfNull(this.name, "CarePlanGoalNameNull");

            writer.WriteStartElement("goal");
            {
                this.name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", this.description);
                XmlWriterHelper.WriteOpt(writer, "start-date", this.startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", this.endDate);
                XmlWriterHelper.WriteOpt(writer, "target-completion-date", this.targetCompletionDate);
                XmlWriterHelper.WriteOpt(writer, "associated-type-info", this.goalAssociatedTypeInfo);
                XmlWriterHelper.WriteOpt(writer, "target-range", this.targetRange);

                if (this.goalAdditionalRanges != null && this.goalAdditionalRanges.Count != 0)
                {
                    foreach (GoalRange goalRange in this.goalAdditionalRanges)
                    {
                        goalRange.WriteXml("goal-additional-ranges", writer);
                    }
                }

                XmlWriterHelper.WriteOpt(writer, "recurrence", this.recurrence);
                XmlWriterHelper.WriteOptString(writer, "reference-id", this.referenceId);
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
                return this.name;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, "Name", "CarePlanGoalNameNull");
                this.name = value;
            }
        }

        private CodableValue name;

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
                return this.description;
            }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "Description");
                this.description = value;
            }
        }

        private string description;

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
                return this.startDate;
            }

            set
            {
                ValidateDates(value, this.endDate);

                this.startDate = value;
            }
        }

        private ApproximateDateTime startDate;

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
                return this.endDate;
            }

            set
            {
                ValidateDates(this.startDate, value);

                this.endDate = value;
            }
        }

        private ApproximateDateTime endDate;

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
                return this.targetCompletionDate;
            }

            set
            {
                this.targetCompletionDate = value;
            }
        }

        private ApproximateDateTime targetCompletionDate;

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
                return this.goalAssociatedTypeInfo;
            }

            set
            {
                this.goalAssociatedTypeInfo = value;
            }
        }

        private AssociatedTypeInfo goalAssociatedTypeInfo;

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
                return this.targetRange;
            }

            set
            {
                this.targetRange = value;
            }
        }

        private GoalRange targetRange;

        /// <summary>
        /// Gets or sets additional ranges for the goal.
        /// </summary>
        ///
        /// <remarks>
        /// For example, in a blood pressure goal, it may be useful to include the 'hypertensive' range in addition to the ideal range.
        /// If there is no information about goalAdditionalRanges the collection should be empty.
        /// </remarks>
        ///
        public Collection<GoalRange> GoalAdditionalRanges => this.goalAdditionalRanges;

        private readonly Collection<GoalRange> goalAdditionalRanges = new Collection<GoalRange>();

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
                return this.recurrence;
            }

            set
            {
                this.recurrence = value;
            }
        }

        private GoalRecurrence recurrence;

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
                return this.referenceId;
            }

            set
            {
                Validator.ThrowIfStringIsEmptyOrWhitespace(value, "ReferenceId");
                this.referenceId = value;
            }
        }

        private string referenceId;

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

            if (this.description == null)
            {
                result = this.name.Text;
            }
            else
            {
                result = string.Format(
                    CultureInfo.CurrentUICulture,
                    ResourceRetriever.GetResourceString("CarePlanGoalFormat"),
                    this.name.Text,
                    this.description);
            }

            return result;
        }
    }
}
