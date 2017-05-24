// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing type that encapsulates application
    /// specific data.
    /// </summary>
    ///
    /// <remarks>
    /// Application specific data can only ever be read or updated by the application
    /// that created.
    ///
    /// This type is not all that interesting by itself but used as a base class
    /// it helps developers implement their own application specific type class
    /// to handle instances by registering the class using the
    /// <see cref="ItemTypeManager.RegisterApplicationSpecificHandler(Guid, string, Type)"/>
    /// method.
    /// </remarks>
    ///
    public class ApplicationSpecific : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationSpecific"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public ApplicationSpecific()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationSpecific"/> class with the
        /// specified application ID, subtype tag, and description.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The unique application identifier.
        /// </param>
        ///
        /// <param name="subtypeTag">
        /// A string identifying a unique schema for the application specific
        /// type. The string format is completely arbitrary and is used by
        /// the application to identify the thing application specific
        /// data as a specific subtype.
        /// </param>
        ///
        /// <param name="description">
        /// A descriptive display text for this item.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> is <see cref="System.Guid.Empty"/>,
        /// or if <paramref name="subtypeTag"/> or <paramref name="description"/> is
        /// <b>null</b> or empty.
        /// </exception>
        ///
        public ApplicationSpecific(
            Guid applicationId,
            string subtypeTag,
            string description)
            : this(applicationId.ToString(), subtypeTag, description)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationSpecific"/> class with the
        /// specified application ID, subtype tag, and description.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The unique application identifier.
        /// </param>
        ///
        /// <param name="subtypeTag">
        /// A string identifying a unique schema for the application specific
        /// type. The string format is completely arbitrary and is used by
        /// the application to identify the thing application specific
        /// data as a specific subtype.
        /// </param>
        ///
        /// <param name="description">
        /// A descriptive display text for this item.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/>, <paramref name="subtypeTag"/>, or
        /// <paramref name="description"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public ApplicationSpecific(
            string applicationId,
            string subtypeTag,
            string description)
            : base(TypeId)
        {
            ApplicationId = applicationId;
            SubtypeTag = subtypeTag;
            Description = description;
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
            new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0");

        /// <summary>
        /// Populates this <see cref="ApplicationSpecific"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the application specific data from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in <paramref name="typeSpecificXml"/> is not
        /// an app-specific node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode("app-specific");

            Validator.ThrowInvalidIfNull(itemNav, Resources.AppSpecificUnexpectedNode);

            XPathNodeIterator itemNodes = itemNav.Select("*");

            foreach (XPathNavigator childNav in itemNodes)
            {
                switch (childNav.Name)
                {
                    case "format-appid":
                        _appId = childNav.Value;
                        break;

                    case "format-tag":
                        _subtypeTag = childNav.Value;
                        break;

                    case "summary":
                        _description = childNav.Value;
                        break;

                    case "when":
                        _when = new HealthServiceDateTime();
                        _when.ParseXml(childNav);
                        break;

                    default:
                        _appSpecificXml.Add(childNav);
                        break;
                }
            }

            ParseApplicationSpecificXml(_appSpecificXml);
        }

        /// <summary>
        /// Parses the parts of the application specific item type that is specific
        /// to each application.
        /// </summary>
        ///
        /// <param name="applicationSpecific">
        /// An <see cref="System.Xml.XPath.XPathNavigator"/> focused on the "app-specific"
        /// element in the item XML.
        /// </param>
        ///
        /// <remarks>
        /// The base elements that all application specific items contain are parsed by
        /// the base class. The derived class overrides this method to parse all
        /// application specific portions of the XML. The base class implementation
        /// places the application specific elements in the <see cref="ApplicationSpecificXml"/>
        /// property and then passes that collection to this method so that derived classes
        /// can further process those elements.  The base class implementation does nothing
        /// to process these elements.
        /// </remarks>
        ///
        protected virtual void ParseApplicationSpecificXml(
            IList<IXPathNavigable> applicationSpecific)
        {
        }

        /// <summary>
        /// Writes the application specific data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the application specific data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="ApplicationId"/>, <see cref="SubtypeTag"/>, or <see cref="Description"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfArgumentNull(writer, nameof(writer), Resources.WriteXmlNullWriter);

            Validator.ThrowSerializationIfNull(ApplicationId, Resources.AppSpecificAppIdMandatory);

            Validator.ThrowSerializationIfNull(SubtypeTag, Resources.AppSpecificTagMandatory);

            Validator.ThrowSerializationIfNull(Description, Resources.AppSpecificDescriptionMandatory);

            // <app-specific>
            writer.WriteStartElement("app-specific");

            // <format-appid>
            writer.WriteElementString("format-appid", ApplicationId);

            // <format-tag>
            writer.WriteElementString("format-tag", SubtypeTag);

            // <when>
            XmlWriterHelper.WriteOpt(
                writer,
                "when",
                _when);

            // <summary>
            writer.WriteElementString("summary", Description);

            // the any node
            WriteApplicationSpecificXml(writer);

            // </app-specific>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the application specific XML to the specified writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer to write the application specific XML to.
        /// </param>
        ///
        /// <remarks>
        /// The XML for the base elements that all application specific items contain is written by
        /// the base class. The derived class overrides this method to write all
        /// application specific portions of the XML. The base class implementation
        /// writes the XML in the <see cref="ApplicationSpecificXml"/> property.
        /// </remarks>
        ///
        protected virtual void WriteApplicationSpecificXml(XmlWriter writer)
        {
            foreach (IXPathNavigable xPathNavigable in _appSpecificXml)
            {
                writer.WriteRaw(xPathNavigable.CreateNavigator().OuterXml);
            }
        }

        /// <summary>
        /// Gets or sets the unique application identifier for which this application
        /// specific information pertains.
        /// </summary>
        ///
        /// <value>
        /// The application identifier should be in the form of a <see cref="System.Guid"/>.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b> or empty.
        /// </exception>
        ///
        public string ApplicationId
        {
            get { return _appId; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "ApplicationId");
                _appId = value;
            }
        }

        private string _appId;

        /// <summary>
        /// Gets or sets a tag that uniquely identifies the schema for the application specific
        /// data.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        ///
        public string SubtypeTag
        {
            get { return _subtypeTag; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "SubtypeTag");
                Validator.ThrowIfStringIsWhitespace(value, "SubtypeTag");
                _subtypeTag = value;
            }
        }

        private string _subtypeTag;

        /// <summary>
        /// Gets or sets the date/time when the application specific item was created.
        /// </summary>
        ///
        /// <value>
        /// A <see cref="HealthServiceDateTime"/> instance representing
        /// the date. The default value is the current year, month, and day.
        /// </value>
        ///
        public HealthServiceDateTime When
        {
            get { return _when; }
            set { _when = value; }
        }

        private HealthServiceDateTime _when;

        /// <summary>
        /// Gets or sets the description of the item for display purposes.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is <b>null</b>, empty, or contains only whitespace.
        /// </exception>
        ///
        public string Description
        {
            get { return _description; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Description");
                Validator.ThrowIfStringIsWhitespace(value, "Description");
                _description = value;
            }
        }

        private string _description;

        /// <summary>
        /// Gets a collection of the application specific XML.
        /// </summary>
        ///
        /// <remarks>
        /// To change the application specific XML for the item, add or remove
        /// items in the collection.
        /// Derived classes can ignore this member.
        /// </remarks>
        ///
        public Collection<IXPathNavigable> ApplicationSpecificXml => _appSpecificXml;

        private readonly Collection<IXPathNavigable> _appSpecificXml =
            new Collection<IXPathNavigable>();

        /// <summary>
        /// Gets a string representation of the application specific item.
        /// </summary>
        ///
        /// <returns>
        /// A string representation of the application specific item.
        /// </returns>
        ///
        public override string ToString()
        {
            return Description;
        }
    }
}
