// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Represents an extension to a thing.
    /// </summary>
    ///
    /// <remarks>
    /// Applications can augment the HealthVault defined data for a
    /// thing type with application or vendor specific data by
    /// implementing item extensions.
    /// <br/><br/>
    /// Applications that implement a thing extension should
    /// derive from this class and register their extension by calling
    /// <see cref="ItemTypeManager.RegisterExtensionHandler(string, Type)"/>.
    /// Whenever the HealthVault SDK reads an extension with the specified
    /// extension source an instance of the derived ThingExtension will be
    /// created and it's <see cref="ThingExtension.ParseXml(IXPathNavigable)"/>
    /// method will be called to populate the data of the class from the
    /// web-service XML.
    /// Common portions of the item extension data will be parsed by the base
    /// class and exposed as properties.
    /// </remarks>
    ///
    public class ThingExtension
    {
        #region ctors

        /// <summary>
        /// Constructs a thing extension for the specified source.
        /// </summary>
        ///
        /// <param name="source">
        /// The source identifier for the extension. This is used to uniquely
        /// identify the extension data.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="source"/> is null or empty.
        /// </exception>
        ///
        public ThingExtension(string source)
            : this()
        {
            Validator.ThrowIfStringNullOrEmpty(source, "source");
            this.source = source;
        }

        /// <summary>
        /// Constructor used when creating an instance for XML deserialization.
        /// </summary>
        ///
        public ThingExtension()
        {
            this.extensionData = new XDocument();
        }

        #endregion ctors

        /// <summary>
        /// Parses the common portions of the extension data.
        /// </summary>
        ///
        /// <param name="extensionNav">
        /// The XML to get the extension data from.
        /// </param>
        ///
        internal void ParseXml(XPathNavigator extensionNav)
        {
            this.source = extensionNav.GetAttribute("source", string.Empty);
            this.Version = extensionNav.GetAttribute("ver", string.Empty);

            string logoString =
                extensionNav.GetAttribute("logo", string.Empty);

            if (!string.IsNullOrEmpty(logoString))
            {
                this.Logo = new Uri(logoString);
            }

            string transformString =
                extensionNav.GetAttribute("xsl", string.Empty);

            if (!string.IsNullOrEmpty(transformString))
            {
                this.Transform = new Uri(transformString);
            }

            // Save off the data in its entirety
            this.extensionData = SDKHelper.SafeLoadXml(extensionNav.OuterXml);

            // Call the derived class for parsing.
            this.ParseXml(this.ExtensionData);
        }

        /// <summary>
        /// Populates the extension data from the specified XML.
        /// </summary>
        ///
        /// <param name="extensionData">
        /// The XML to retrieve the extension data from. Note, this may be
        /// an <see cref="XDocument"/>.
        /// </param>
        ///
        /// <remarks>
        /// Derived classes should override this method to parse the extension
        /// XML and populate class members with the data. The common extension
        /// attributes are handled by the base class before this method is
        /// called.
        /// <br/><br/>
        /// The default implementation of this method does nothing.
        /// </remarks>
        ///
        protected virtual void ParseXml(IXPathNavigable extensionData)
        {
        }

        /// <summary>
        /// Writes the extension data to the specified writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The writer to write the extension XML to.
        /// </param>
        ///
        /// <exception cref="ThingSerializationException">
        /// If <see cref="Source"/> is null or empty.
        /// </exception>
        ///
        internal void WriteExtensionXml(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(this.source))
            {
                throw new ThingSerializationException(Resources.ExtensionSerializationSourceMissing);
            }

            // <extension>
            writer.WriteStartElement("extension");
            writer.WriteAttributeString("source", this.source);

            if (!string.IsNullOrEmpty(this.Version))
            {
                writer.WriteAttributeString("ver", this.Version);
            }

            if (this.Logo != null)
            {
                writer.WriteAttributeString("logo", this.Logo.ToString());
            }

            if (this.Transform != null)
            {
                writer.WriteAttributeString("xsl", this.Transform.ToString());
            }

            this.WriteXml(writer);

            // </extension>
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the data portion of the extension to the specified XML
        /// writer.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XML writer to write the data portion of the extension to.
        /// </param>
        ///
        /// <remarks>
        /// Derived classes should override this method to add extension
        /// specific data to the extension XML for the item. Note, the base
        /// class handles writing the extension node and it's attributes to
        /// the writer. The derived class implementation should only write
        /// the inner nodes of the extension.
        /// <br/><br/>
        /// The base implementation writes the contents of the
        /// <see cref="ExtensionData"/> to the writer and should not be called
        /// by the derived class.
        /// </remarks>
        ///
        protected virtual void WriteXml(XmlWriter writer)
        {
            XPathNavigator extensionDocumentNav =
                this.extensionData.CreateNavigator();

            XPathNavigator extensionNodeNav =
                extensionDocumentNav.SelectSingleNode("extension");

            if (extensionNodeNav != null)
            {
                writer.WriteRaw(extensionNodeNav.InnerXml);
            }
        }

        /// <summary>
        /// Gets or sets the extension source identifier.
        /// </summary>
        ///
        /// <remarks>
        /// The source is used to identify the extension. To register a class
        /// as a handler for a specific extension source see
        /// <see cref="ItemTypeManager.RegisterExtensionHandler(string, Type)"/>.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="value"/> is null or empty.
        /// </exception>
        ///
        public string Source
        {
            get { return this.source; }

            set
            {
                Validator.ThrowIfStringNullOrEmpty(value, "Source");
                this.source = value;
            }
        }

        private string source;

        /// <summary>
        /// Gets or sets the version of the extension.
        /// </summary>
        ///
        /// <remarks>
        /// The version is optional and will be ignored if set to null.
        /// </remarks>
        ///
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the URL to a logo for display use with the extension.
        /// </summary>
        ///
        /// <remarks>
        /// The logo is optional and will be ignored if set to null.
        /// <br/><br/>
        /// In some applications a generic view of the thing will be
        /// shown. If a logo is provided on an extension those applications
        /// can embed the logo in the user interface to show the user that
        /// there is additional data. It can also be used along with the
        /// <see cref="Transform"/> to provide a link to the extension data.
        /// </remarks>
        ///
        public Uri Logo { get; set; }

        /// <summary>
        /// Gets or sets the URL to an XSL transform which can transform the
        /// XML of the extension into HTML suitable for display in a small
        /// form or popup.
        /// </summary>
        ///
        /// <remarks>
        /// The transform is optional and will be ignored if set to null.
        /// <br/><br/>
        /// In some applications a generic view of the thing will be
        /// shown. In such applications the extension data can be rendered as
        /// HTML using the specified transform. Applications should consider the
        /// security implications of running external transforms within the application
        /// context.
        /// HealthVault Shell does not render extensions using this transform.
        /// </remarks>
        ///
        public Uri Transform { get; set; }

        /// <summary>
        /// Gets the extension data for the extension.
        /// </summary>
        ///
        /// <remarks>
        /// To modify the extension attributes use the other properties of this
        /// class. To modify the extension data manipulate the returned
        /// XML document unless a derived type is available, in which case
        /// you should use the properties and method of the derived type to
        /// manipulate the data.
        /// <br/><br/>
        /// </remarks>
        ///
        public IXPathNavigable ExtensionData => this.extensionData.CreateNavigator();

        private XDocument extensionData;
    }
}
