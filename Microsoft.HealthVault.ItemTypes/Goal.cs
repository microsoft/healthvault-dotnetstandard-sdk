// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Health.ItemTypes;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a goal.
    /// </summary>
    ///
    public class Goal : ItemBase
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
                this.targetDate = new ApproximateDateTime();
                this.targetDate.ParseXml(targetNav);
            }

            XPathNavigator completionNav =
                navigator.SelectSingleNode("completion-date");

            if (completionNav != null)
            {
                this.completionDate = new ApproximateDateTime();
                this.completionDate.ParseXml(completionNav);
            }

            XPathNavigator statusNav =
                navigator.SelectSingleNode("status");

            if (statusNav != null)
            {
                try
                {
                    this.status =
                        (GoalStatus)Enum.Parse(typeof(GoalStatus), statusNav.Value, true);
                }
                catch (ArgumentException)
                {
                    this.status = GoalStatus.Unknown;
                    this.statusString = statusNav.Value;
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

            if (this.targetDate != null)
            {
                this.targetDate.WriteXml("target-date", writer);
            }

            if (this.completionDate != null)
            {
                this.completionDate.WriteXml("completion-date", writer);
            }

            if (this.status != null)
            {
                if (this.status != GoalStatus.Unknown)
                {
                    writer.WriteElementString(
                        "status",
                        this.status.ToString());
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.statusString))
                    {
                        writer.WriteElementString(
                            "status",
                            this.statusString);
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
            get { return this.targetDate; }
            set { this.targetDate = value; }
        }

        private ApproximateDateTime targetDate;

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
            get { return this.completionDate; }
            set { this.completionDate = value; }
        }

        private ApproximateDateTime completionDate;

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
            get { return this.status; }
            set { this.status = value; }
        }

        private GoalStatus? status;
        private string statusString;
    }
}
