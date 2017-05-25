// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates a goal to have
    /// a certain number of aerobic sessions per week.
    /// </summary>
    ///
    public class AerobicWeeklyGoal : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AerobicWeeklyGoal"/> class with default
        /// values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
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
        public AerobicWeeklyGoal(AerobicBase session, int recurrence)
            : base(TypeId)
        {
            AerobicSession = session;
            Recurrence = recurrence;
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

            Validator.ThrowInvalidIfNull(aerobicWeeklyNav, Resources.AerobicWeeklyUnexpectedNode);

            _session = new AerobicBase();
            _session.ParseXml(aerobicWeeklyNav.SelectSingleNode("session"));

            _recurrence =
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="AerobicSession"/> property is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);
            Validator.ThrowSerializationIfNull(_session, Resources.AerobicWeeklySessionNotSet);

            // <aerobic-weekly>
            writer.WriteStartElement("aerobic-weekly");

            // <session>
            _session.WriteXml("session", writer);

            // <recurrence>
            writer.WriteElementString(
                "recurrence",
                _recurrence.ToString(CultureInfo.InvariantCulture));

            // </aerobic-weekly>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the definition of the aerobic session needed to
        /// meet the goal.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="AerobicBase"/> value representing the session definition.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public AerobicBase AerobicSession
        {
            get { return _session; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(AerobicSession), Resources.AerobicWeeklySessionNull);
                _session = value;
            }
        }

        private AerobicBase _session = new AerobicBase();

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
            get { return _recurrence; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Recurrence), Resources.AerobicWeeklyRecurrenceNotPositive);
                }

                _recurrence = value;
            }
        }

        private int _recurrence = 3;

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
                Resources.AerobicWeeklyGoalToStringFormat,
                AerobicSession.ToString(),
                Recurrence);
        }
    }
}
