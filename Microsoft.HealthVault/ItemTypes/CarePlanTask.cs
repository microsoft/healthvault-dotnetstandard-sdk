// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A task defines an action to be performed by the user.
    /// </summary>
    ///
    public class CarePlanTask : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanTask"/> class with default values.
        /// </summary>
        ///
        public CarePlanTask()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CarePlanTask"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// Name of the task.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <b>null</b>.
        /// </exception>
        ///
        public CarePlanTask(CodableValue name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Populates this <see cref="CarePlanTask"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML to get the CarePlanTask data from.
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
            this.startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "start-date");
            this.endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "end-date");
            this.targetCompletionDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "target-completion-date");
            this.sequenceNumber = XPathHelper.GetOptNavValueAsInt(navigator, "sequence-number");
            this.taskAssociatedTypeInfo = XPathHelper.GetOptNavValue<AssociatedTypeInfo>(navigator, "associated-type-info");
            this.recurrence = XPathHelper.GetOptNavValue<CarePlanTaskRecurrence>(navigator, "recurrence");
            this.referenceId = XPathHelper.GetOptNavValue(navigator, "reference-id");
        }

        /// <summary>
        /// Writes the XML representation of the CarePlanTask into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the medical image study series.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the CarePlanTask should be
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
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "WriteXmlEmptyNodeName");
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIfNull(this.name, Resources.CarePlanTaskNameNull);

            writer.WriteStartElement("task");
            {
                this.name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", this.description);
                XmlWriterHelper.WriteOpt(writer, "start-date", this.startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", this.endDate);
                XmlWriterHelper.WriteOpt(writer, "target-completion-date", this.targetCompletionDate);
                XmlWriterHelper.WriteOptInt(writer, "sequence-number", this.sequenceNumber);
                XmlWriterHelper.WriteOpt(writer, "associated-type-info", this.taskAssociatedTypeInfo);
                XmlWriterHelper.WriteOpt(writer, "recurrence", this.recurrence);
                XmlWriterHelper.WriteOptString(writer, "reference-id", this.referenceId);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets name of the task.
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
                Validator.ThrowIfArgumentNull(value, nameof(this.Name), Resources.CarePlanTaskNameNull);
                this.name = value;
            }
        }

        private CodableValue name;

        /// <summary>
        /// Gets or sets description of the task.
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
                    if (startDate.CompareTo(endDate) > 0)
                    {
                        throw new ArgumentException(Resources.CarePlanTaskDateInvalid, nameof(startDate));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the start date for the task.
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
        /// Gets or sets the end date for the task.
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
        /// Gets or sets the date user intends to complete the task.
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
        /// Gets or sets sequence number associated with the task.
        /// </summary>
        ///
        /// <remarks>
        /// Sequence number could be used to decide the order in which the tasks should be performed.
        /// </remarks>
        ///
        public int? SequenceNumber
        {
            get
            {
                return this.sequenceNumber;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Resources.CarePlanTaskSequenceNumberInvalid, nameof(this.SequenceNumber));
                }

                this.sequenceNumber = value;
            }
        }

        private int? sequenceNumber;

        /// <summary>
        /// Gets or sets HealthVault type information related to this task.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about TaskAssociatedTypeInfo the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public AssociatedTypeInfo TaskAssociatedTypeInfo
        {
            get
            {
                return this.taskAssociatedTypeInfo;
            }

            set
            {
                this.taskAssociatedTypeInfo = value;
            }
        }

        private AssociatedTypeInfo taskAssociatedTypeInfo;

        /// <summary>
        /// Gets or sets recurrence of the task.
        /// </summary>
        ///
        /// <remarks>
        /// If there is no information about recurrence the value should be set to <b>null</b>.
        /// </remarks>
        ///
        public CarePlanTaskRecurrence Recurrence
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

        private CarePlanTaskRecurrence recurrence;

        /// <summary>
        /// Gets or sets an unique id to distinguish one task from another.
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
        /// Gets a string representation of the CarePlanTask.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the CarePlanTask.
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
                    Resources.CarePlanTaskFormat,
                    this.name.Text,
                    this.description);
            }

            return result;
        }
    }
}
