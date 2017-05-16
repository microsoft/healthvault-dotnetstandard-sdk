// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Clients;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;

namespace Microsoft.HealthVault.ItemTypes
{
    /// <summary>
    /// Represents a thing that stores information that can be used by an application to
    /// render content from another application.
    /// </summary>
    ///
    public class ApplicationDataReference : ThingBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationDataReference"/> class with default values.
        /// </summary>
        ///
        /// <remarks>
        /// The item is not added to the health record until the <see cref="IThingClient.CreateNewThingsAsync{ThingBase}(Guid, ICollection{ThingBase})"/> method is called.
        /// </remarks>
        ///
        public ApplicationDataReference()
            : base(TypeId)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationDataReference"/> class
        /// specifying the mandatory values.
        /// </summary>
        ///
        /// <param name="name">
        /// The consumer-friendly name.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="name"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public ApplicationDataReference(string name)
            : base(TypeId)
        {
            Name = name;
        }

        /// <summary>
        /// Retrieves the unique identifier for the item type.
        /// </summary>
        ///
        public static new readonly Guid TypeId =
            new Guid("9ad2a94f-c6a4-4d78-8b50-75b65be0e250");

        /// <summary>
        /// Populates this <see cref="ApplicationDataReference"/> instance from the data in the XML.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The XML to get the application data reference from.
        /// </param>
        ///
        /// <exception cref="InvalidOperationException">
        /// The first node in the <paramref name="typeSpecificXml"/> parameter
        /// is not an application-data-reference node.
        /// </exception>
        ///
        protected override void ParseXml(IXPathNavigable typeSpecificXml)
        {
            XPathNavigator itemNav =
                typeSpecificXml.CreateNavigator().SelectSingleNode(
                    "application-data-reference");

            Validator.ThrowInvalidIfNull(itemNav, Resources.ApplicationDataReferenceUnexpectedNode);

            _name = XPathHelper.GetOptNavValue(itemNav, "name");

            _renderFileName = XPathHelper.GetOptNavValue(itemNav, "render-filename");

            _publicUrl = XPathHelper.GetOptNavValueAsUri(itemNav, "public-url");
            _configurationUrl = XPathHelper.GetOptNavValueAsUri(itemNav, "configuration-url");
            _applicationDataUrl = XPathHelper.GetOptNavValueAsUri(itemNav, "application-data-url");
        }

        /// <summary>
        /// Writes the application data reference data to the specified XmlWriter.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XmlWriter to write the application data reference data to.
        /// </param>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="writer"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Name"/> is <b>null</b>.
        /// </exception>
        ///
        public override void WriteXml(XmlWriter writer)
        {
            Validator.ThrowIfWriterNull(writer);
            Validator.ThrowSerializationIfNull(_name, Resources.ApplicationDataReferenceNameNotSet);

            writer.WriteStartElement("application-data-reference");

            writer.WriteElementString("name", _name);

            XmlWriterHelper.WriteOptString(writer, "render-filename", _renderFileName);

            XmlWriterHelper.WriteOptUrl(writer, "public-url", _publicUrl);

            XmlWriterHelper.WriteOptUrl(writer, "configuration-url", _configurationUrl);

            XmlWriterHelper.WriteOptUrl(writer, "application-data-url", _applicationDataUrl);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the consumer-friendly name.
        /// </summary>
        ///
        /// <value>
        /// The consumer-friendly name.
        /// </value>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null or empty</b>.
        /// </exception>
        ///
        public string Name
        {
            get { return _name; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Name");
                _name = value;
            }
        }

        private string _name;

        /// <summary>
        /// Gets or sets the fully qualified name of the Silverlight application.
        /// </summary>
        ///
        /// <value>
        /// The render file name.
        /// </value>
        ///
        /// <remarks>
        /// In current implementations, the xap files themselves must be hosted by Microsoft.
        /// This name will be used to look-up the source location of the widget for the purpose of display.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="value"/> parameter is <b>null or empty</b>.
        /// </exception>
        ///
        public string RenderFileName
        {
            get { return _renderFileName; }

            set
            {
                if (value != null && value.Length == 0)
                {
                    throw new ArgumentNullException(Resources.RenderFileNameEmptyValue, nameof(RenderFileName));
                }

                _renderFileName = value;
            }
        }

        private string _renderFileName;

        /// <summary>
        /// Gets or sets the URL that renders the application.
        /// </summary>
        ///
        /// <value>
        /// The public URL.
        /// </value>
        ///
        /// <remarks>
        /// The rendering is suitable for IFrame embedding.
        /// </remarks>
        ///
        public Uri PublicUrl
        {
            get { return _publicUrl; }
            set { _publicUrl = value; }
        }

        private Uri _publicUrl;

        /// <summary>
        /// Gets or sets the URL to the application's configuration page.
        /// </summary>
        ///
        /// <value>
        /// The configuration URL.
        /// </value>
        ///
        /// <remarks>
        /// This page is where the user can enable/disable the application.
        /// </remarks>
        ///
        public Uri ConfigurationUrl
        {
            get { return _configurationUrl; }
            set { _configurationUrl = value; }
        }

        private Uri _configurationUrl;

        /// <summary>
        /// Gets or sets the URL used to obtain data from the application.
        /// </summary>
        ///
        /// <value>
        /// The application data URL.
        /// </value>
        ///
        /// <remarks>
        /// If there is application specific data, or data that needs to be served from the application
        /// rather than directly from HealthVault, then this field should contain the fully qualified HTTPS URL.
        /// </remarks>
        ///
        public Uri ApplicationDataUrl
        {
            get { return _applicationDataUrl; }
            set { _applicationDataUrl = value; }
        }

        private Uri _applicationDataUrl;

        /// <summary>
        /// Gets a string representation of the application data reference.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the application data reference.
        /// </returns>
        ///
        public override string ToString()
        {
            if (_name == null)
            {
                return string.Empty;
            }

            return _name;
        }
    }
}
