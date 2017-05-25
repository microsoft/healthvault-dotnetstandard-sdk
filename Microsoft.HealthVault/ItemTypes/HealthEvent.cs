// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// A health event that occurred for the owner of the record.
    /// </summary>
    ///
    /// <remarks>
    /// A health event is a health-related occurrence for the owner of the record.  For
    /// children, it might be used to record the date that the child first crawls.
    /// For adults, it might be used to record the date of an accident or progress through a rehabilitation.
    /// </remarks>
    ///
    public class HealthEvent : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HealthEvent"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public HealthEvent()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthEvent"/> class
        /// specifying mandatory values.
        /// </summary>
        ///
        /// <remarks>
        /// This item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        /// <param name="when">
        /// The date and time the event occurred.
        /// </param>
        /// <param name="eventValue">
        /// The name of the health event.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="when"/> is <b>null</b>.
        /// If <paramref name="eventValue"/> is <b>null</b>.
        /// </exception>
        ///
        public HealthEvent(
            ApproximateDateTime when,
            CodableValue eventValue)
        : base(TypeId)
        {
            When = when;
            Event = eventValue;
        }

        /// <summary>
        /// Retrieves the unique identifier for the HealthEvent type.
        /// </summary>
        ///
        /// <value>
        /// A GUID.
        /// </value>
        ///
        public static new readonly Guid TypeId =
            new Guid("1572af76-1653-4c39-9683-9f9ca6584ba3");

        /// <summary>
        /// Populates this <see cref="HealthEvent"/> instance from the data in the specified XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the HealthEvent data from.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="typeSpecificXml"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the first node in <paramref name="typeSpecificXml"/> is not
        /// a HealthEvent node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            Validator.ThrowIfArgumentNull(typeSpecificXml, nameof(typeSpecificXml), Resources.ParseXmlNavNull);

            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("health-event");

            Validator.ThrowInvalidIfNull(itemNav, Resources.HealthEventUnexpectedNode);

            _when = new ApproximateDateTime();
            _when.ParseXml(itemNav.SelectSingleNode("when"));
            _event = new CodableValue();
            _event.ParseXml(itemNav.SelectSingleNode("event"));
            _category = XPathHelper.GetOptNavValue<CodableValue>(itemNav, "category");
        }

        /// <summary>
        /// Writes the XML representation of the HealthEvent into
        /// the specified XML writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer into which the HealthEvent should be
        /// written.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="When"/> is <b>null</b>.
        /// If <see cref="Event"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);

            Validator.ThrowSerializationIfNull(_when, Resources.WhenNullValue);
            Validator.ThrowSerializationIfNull(_event, "EventNullValue");

            writer.WriteStartElement("health-event");

            _when.WriteXml("when", writer);
            _event.WriteXml("event", writer);
            XmlWriterHelper.WriteOpt(writer, "category", _category);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the date and time the event occurred.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApproximateDateTime When
        {
            get
            {
                return _when;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(value), Resources.WhenNullValue);

                _when = value;
            }
        }

        private ApproximateDateTime _when;

        /// <summary>
        /// Gets or sets the name of the health event.
        /// </summary>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="value"/> parameter is <b>null</b>.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Event
        {
            get
            {
                return _event;
            }

            set
            {
                Validator.ThrowIfArgumentNull(value, nameof(value), Resources.EventNullValue);

                _event = value;
            }
        }

        private CodableValue _event;

        /// <summary>
        /// Gets or sets the category of the health event.
        /// </summary>
        ///
        /// <remarks>
        /// The category can be used to group related events together. For example, 'pediatric'.
        /// If there is no information about category the value should be set to <b>null</b>.
        /// </remarks>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "FXCop thinks that CodableValue is a collection, so it throws this error.")]
        public CodableValue Category
        {
            get
            {
                return _category;
            }

            set
            {
                _category = value;
            }
        }

        private CodableValue _category;

        /// <summary>
        /// Gets a string representation of the HealthEvent.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the HealthEvent.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append(_event.Text);

            if (_category != null)
            {
                result.Append(" ");
                result.Append(Resources.OpenParen);
                result.Append(_category.Text);
                result.Append(Resources.CloseParen);
            }

            return result.ToString();
        }
    }
}
