// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents an occurrence of an event, such as an activity,
    /// period of awakening during sleep, and so on.
    /// </summary>
    ///
    public class Occurrence : ItemBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Occurrence"/> class with
        /// default values.
        /// </summary>
        ///
        public Occurrence()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Occurrence"/> class with
        /// the specified approximate time and duration.
        /// </summary>
        ///
        /// <param name="when">
        /// The approximate time for the occurrence.
        /// </param>
        ///
        /// <param name="minutes">
        /// The duration of the occurrence in minutes.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="when"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="minutes"/> parameter is negative.
        /// </exception>
        ///
        public Occurrence(ApproximateTime when, int minutes)
        {
            When = when;
            Minutes = minutes;
        }

        /// <summary>
        /// Populates the data from the specified XML.
        /// </summary>
        ///
        /// <param name="navigator">
        /// The XML containing the occurrence information.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="navigator"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public override void ParseXml(XPathNavigator navigator)
        {
            Validator.ThrowIfNavigatorNull(navigator);

            _when = new ApproximateTime();
            _when.ParseXml(navigator.SelectSingleNode("when"));

            _minutes = navigator.SelectSingleNode("minutes").ValueAsInt;
        }

        /// <summary>
        /// Writes the XML representation of the occurrence into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="nodeName">
        /// The name of the outer node for the occurrence.
        /// </param>
        ///
        /// <param name="writer">
        /// The XML writer into which the occurrence should be
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
        /// <exception cref="ThingSerializationException">
        /// The <see cref="When"/> property is <b>null</b>, or the <see cref="Minutes"/> property is less than or equal to zero.
        /// </exception>
        ///
        public override void WriteXml(string nodeName, XmlWriter writer)
        {
            Validator.ThrowIfStringNullOrEmpty(nodeName, "nodeName");
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_when, Resources.OccurrenceWhenNotSet);
            Validator.ThrowSerializationIfNull(_minutes, Resources.OccurrenceMinutesNotSet);

            writer.WriteStartElement(nodeName);

            _when.WriteXml("when", writer);
            writer.WriteElementString("minutes", _minutes.ToString());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the time of the occurrence.
        /// </summary>
        ///
        /// <value>
        /// An <see cref="ApproximateTime"/> value representing the
        /// time of the occurrence.
        /// </value>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b> on set.
        /// </exception>
        ///
        public ApproximateTime When
        {
            get { return _when; }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(When), Resources.OccurrenceWhenMandatory);
                _when = value;
            }
        }

        private ApproximateTime _when;

        /// <summary>
        /// Gets or sets the duration of the occurrence in minutes.
        /// </summary>
        ///
        /// <value>
        /// An integer representing the duration.
        /// </value>
        ///
        /// <exception cref="ArgumentOutOfRangeException">
        /// The <paramref name="value"/> parameter is less than or equal to zero.
        /// </exception>
        ///
        public int Minutes
        {
            get { return (int)_minutes; }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Minutes), Resources.OccurrenceMinutesMustBePositive);
                }

                _minutes = value;
            }
        }

        private int? _minutes;
    }
}
