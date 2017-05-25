// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
            Name = name;
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

            _name = new CodableValue();
            _name.ParseXml(navigator.SelectSingleNode("name"));
            _description = XPathHelper.GetOptNavValue(navigator, "description");
            _startDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "start-date");
            _endDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "end-date");
            _targetCompletionDate = XPathHelper.GetOptNavValue<ApproximateDateTime>(navigator, "target-completion-date");
            _sequenceNumber = XPathHelper.GetOptNavValueAsInt(navigator, "sequence-number");
            _taskAssociatedTypeInfo = XPathHelper.GetOptNavValue<AssociatedTypeInfo>(navigator, "associated-type-info");
            _recurrence = XPathHelper.GetOptNavValue<CarePlanTaskRecurrence>(navigator, "recurrence");
            _referenceId = XPathHelper.GetOptNavValue(navigator, "reference-id");
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

            Validator.ThrowSerializationIfNull(_name, Resources.CarePlanTaskNameNull);

            writer.WriteStartElement("task");
            {
                _name.WriteXml("name", writer);
                XmlWriterHelper.WriteOptString(writer, "description", _description);
                XmlWriterHelper.WriteOpt(writer, "start-date", _startDate);
                XmlWriterHelper.WriteOpt(writer, "end-date", _endDate);
                XmlWriterHelper.WriteOpt(writer, "target-completion-date", _targetCompletionDate);
                XmlWriterHelper.WriteOptInt(writer, "sequence-number", _sequenceNumber);
                XmlWriterHelper.WriteOpt(writer, "associated-type-info", _taskAssociatedTypeInfo);
                XmlWriterHelper.WriteOpt(writer, "recurrence", _recurrence);
                XmlWriterHelper.WriteOptString(writer, "reference-id", _referenceId);
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
                return _name;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(Name), Resources.CarePlanTaskNameNull);
                _name = value;
            }
        }

        private CodableValue _name;

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
                return _targetCompletionDate;
            }

            set
            {
                _targetCompletionDate = value;
            }
        }

        private ApproximateDateTime _targetCompletionDate;

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
                return _sequenceNumber;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(Resources.CarePlanTaskSequenceNumberInvalid, nameof(SequenceNumber));
                }

                _sequenceNumber = value;
            }
        }

        private int? _sequenceNumber;

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
                return _taskAssociatedTypeInfo;
            }

            set
            {
                _taskAssociatedTypeInfo = value;
            }
        }

        private AssociatedTypeInfo _taskAssociatedTypeInfo;

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
                return _recurrence;
            }

            set
            {
                _recurrence = value;
            }
        }

        private CarePlanTaskRecurrence _recurrence;

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

            if (_description == null)
            {
                result = _name.Text;
            }
            else
            {
                result = string.Format(
                    CultureInfo.CurrentUICulture,
                    Resources.CarePlanTaskFormat,
                    _name.Text,
                    _description);
            }

            return result;
        }
    }
}
