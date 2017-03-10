// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a health record item type that encapsulates a goal to have
    /// a certain number of aerobic sessions per week.
    /// </summary>
    ///
    public class AerobicWeeklyGoal : HealthRecordItem
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AerobicWeeklyGoal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the
        /// <see cref="HealthRecordAccessor.NewItemAsync(HealthRecordItem)"/>
        /// method is called.
        /// </remarks>
        ///
        public AerobicWeeklyGoal()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="AerobicWeeklyGoal"/> class
        /// with the specified session data.
        /// </summary>
        ///
        /// <param name="session">
        /// An aerobic session that defines the goal.
        /// </param>
        ///
        /// <param name="recurrence">
        /// The number of occurrences of the session required in a week to
        /// meet the goal.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="session"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="recurrence"/> parameter is negative or zero.
        /// </exception>
        ///
        public AerobicWeeklyGoal(AerobicData session, int recurrence)
            : base(TypeId)
        {
            this.AerobicSession = session;
            this.Recurrence = recurrence;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("e4501363-fb95-4a11-bb60-da64e98048b5");

        /// <summary>
        /// Populates this <see cref="AerobicWeeklyGoal"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the aerobic goal data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an aerobic-session node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator aerobicWeeklyNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "aerobic-weekly");

            Validator.ThrowInvalidIfNull(aerobicWeeklyNav, "AerobicWeeklyUnexpectedNode");

            this.session = new AerobicData();
            this.session.ParseXml(aerobicWeeklyNav.SelectSingleNode("session"));

            this.recurrence =
                aerobicWeeklyNav.SelectSingleNode("recurrence").ValueAsInt;
        }

        /// <summary>
        /// Writes the aerobic weekly goal data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the aerobic weekly goal data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="HealthRecordItemSerializationException">
        /// The <see cref="AerobicSession"/> property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, "writer", "WriteXmlNullWriter");
            Validator.ThrowSerializationIfNull(this.session, "AerobicWeeklySessionNotSet");

            // <aerobic-weekly>
            writer.WriteStartElement("aerobic-weekly");

            // <session>
            this.session.WriteXml("session", writer);

            // <recurrence>
            writer.WriteElementString(
                "recurrence",
                this.recurrence.ToString(CultureInfo.InvariantCulture));

            // </aerobic-weekly>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the definition of the aerobic session needed to
        /// meet the goal.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="AerobicData"/> value representing the session definition.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public AerobicData AerobicSession
        {
            get { return this.session; }

            set
            {
                Validator.ThrowIfArgumentNull(value, "AerobicSession", "AerobicWeeklySessionNull");
                this.session = value;
            }
        }

        private AerobicData session = new AerobicData();

        /// <summary>
        /// Gets or sets the number of sessions per week required to meet
        /// the goal.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the number of sessions.
        /// </value>
        ///
        /// <remarks>
        /// This number must be greater than zero. The property defaults
        /// to three.
        /// </remarks>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int Recurrence
        {
            get { return this.recurrence; }

            set
            {
                Validator.ThrowArgumentOutOfRangeIf(
                    value <= 0,
                    "Recurrence",
                    "AerobicWeeklyRecurrenceNotPositive");
                this.recurrence = value;
            }
        }

        private int recurrence = 3;

        /// <summary>
        /// Gets a string representation of the aerobic weekly goal item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the aerobic weekly goal item.
        /// </returns>
        ///
        public override string ToString()
        {
            return string.Format(
                ResourceRetriever.GetResourceString(
                    "AerobicWeeklyGoalToStringFormat"),
                this.AerobicSession.ToString(),
                this.Recurrence);
        }
    }
}
