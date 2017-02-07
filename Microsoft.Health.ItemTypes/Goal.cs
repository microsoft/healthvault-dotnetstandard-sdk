// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health.ItemTypes
{
    /// <summary>
    /// Represents a goal.
    /// </summary>
    /// 
    public class Goal : HealthRecordItemData
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Goal"/> class with default values.
        /// </summary>
        /// 
        public Goal()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Goal"/> class with the specified 
        /// target date.
        /// </summary>
        /// 
        /// <param name="targetDate">
        /// The approximate date which is the target completion date
        /// for the goal.
        /// </param>
        /// 
        public Goal(ApproximateDateTime targetDate)
        {
            this.TargetDate = targetDate;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        /// 
        /// <param name="navigator">
        /// The XML containing the goal information.
        /// </param>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            XPathNavigator targetNav =
                navigator.SelectSingleNode("target-date");

            if (targetNav != null)
            {
                _targetDate = new ApproximateDateTime();
                _targetDate.ParseXml(targetNav);
            }

            XPathNavigator completionNav =
                navigator.SelectSingleNode("completion-date");

            if (completionNav != null)
            {
                _completionDate = new ApproximateDateTime();
                _completionDate.ParseXml(completionNav);
            }

            XPathNavigator statusNav =
                navigator.SelectSingleNode("status");

            if (statusNav != null)
            {
                try
                {
                    _status =
                        (GoalStatus)
                        Enum.Parse(typeof(GoalStatus), statusNav.Value, true);
                }
                catch (ArgumentException)
                {
                    _status = GoalStatus.Unknown;
                    _statusString = statusNav.Value;
                }
            }
        }

        /// <summary>
        /// Writes the XML representation of the goal into
        /// the specified XML writer.
        /// </summary>
        /// 
        /// <param name="nodeName">
        /// The name of the outer node for the goal.
        /// </param>
        /// 
        /// <param name="writer">
        /// The XML writer into which the goal should be 
        /// written.
        /// </param>
        /// 
        /// <exception cref="ArgumentException">
        /// The <paramref name="nodeName"/> parameter is <b>null</b> or empty.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);

            writer.WriteStartElement(nodeName);

            if (_targetDate != null)
            {
                _targetDate.WriteXml("target-date", writer);
            }

            if (_completionDate != null)
            {
                _completionDate.WriteXml("completion-date", writer);
            }

            if (_status != null)
            {
                if (_status != GoalStatus.Unknown)
                {
                    writer.WriteElementString(
                        "status",
                        _status.ToString());
                }
                else
                {
                    if (!String.IsNullOrEmpty(_statusString))
                    {
                        writer.WriteElementString(
                            "status",
                            _statusString);
                    }
                }
            }

            writer.WriteEndElement();
        }


        /// <summary>
        /// Gets or sets the date for achieving the goal.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the date.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if this property should not be stored.
        /// </remarks>
        /// 
        public ApproximateDateTime TargetDate
        {
            get { return _targetDate; }
            set { _targetDate = value; }
        }
        private ApproximateDateTime _targetDate;

        /// <summary>
        /// Gets or sets the date the goal was achieved.
        /// </summary>
        /// 
        /// <value>
        /// An <see cref="ApproximateDateTime"/> representing the date.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if this property should not be stored.
        /// </remarks>
        /// 
        public ApproximateDateTime CompletionDate
        {
            get { return _completionDate; }
            set { _completionDate = value; }
        }
        private ApproximateDateTime _completionDate;

        /// <summary>
        /// Gets or sets the status of the goal.
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="GoalStatus"/> value.
        /// </value>
        /// 
        /// <remarks>
        /// Set the value to <b>null</b> if this property should not be stored.
        /// </remarks>
        /// 
        public GoalStatus? Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private GoalStatus? _status;
        private string _statusString;
    }
}
