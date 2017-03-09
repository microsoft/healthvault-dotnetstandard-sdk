// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Configuration;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Defines a result view for use with
    /// <see cref="HealthRecordSearcher"/> searches.
    /// </summary>
    ///
    /// <remarks>
    /// This class wraps up the logic for constructing the format tag for
    /// querying health record items with the "GetThings" method.
    /// </remarks>
    ///
    [DebuggerDisplay("Sections = {Sections}")]
    public class HealthRecordView
    {
        private IConfiguration configuration = Ioc.Get<IConfiguration>();

        /// <summary>
        /// Gets or sets the sections that will be retrieved when the
        /// query is made.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="HealthRecordItemSections"/>. The default
        /// values are Core and XML.
        /// </value>
        ///
        public HealthRecordItemSections Sections
        {
            get { return this.sections; }

            set
            {
                // Always add in Core so that we get the type-id element
                this.sections = value | HealthRecordItemSections.Core;
            }
        }

        private HealthRecordItemSections sections =
            HealthRecordItemSections.Core |
            HealthRecordItemSections.Xml;

        /// <summary>
        /// Gets the names of the transforms to apply to the resulting
        /// XML data section.
        /// </summary>
        ///
        /// <remarks>
        /// The name of the transform refers to the format of the data to
        /// which the transform converts the XML data. For example, "rss"
        /// converts the XML data into RSS format. Note, all formats to
        /// which the data can be converted are legal XML documents
        /// (RSS, XHTML, and so on).  A list of the valid transform names is
        /// available in the HealthVault Developer's Guide.
        /// </remarks>
        ///
        public Collection<string> TransformsToApply { get; } = new Collection<string>();

        /// <summary>
        /// Gets a collection of the version IDs for the types in which the results should be
        /// formatted.
        /// </summary>
        ///
        /// <remarks>
        /// When a type gets versioned, HealthVault will retrieve instances of
        /// multiple versions even when only one type is specified in the request
        /// filter.  By default HealthVault will map the XML of the instance to
        /// the version supported by the application based on the base
        /// authorization XML specified in the configuration of the application in
        /// HealthVault. However, if multiple versions are supported by the
        /// application, it can use this property to state the version format to
        /// use when writing out the instance XML.
        /// For example, when querying for medications, HealthVault will retrieve
        /// medications of both version one and two schemas. If an application
        /// only supports version one of the medication schema, then HealthVault
        /// will automatically map version two instances down to the version one
        /// schema. However, if the application supports both version one and two
        /// by declaring both type IDs in the applications configuration, then
        /// version one instances will be returned using the version one schema, and
        /// version two instances will be returned using the version two schema.
        /// If this application wants to retrieve all instances using a common
        /// schema (say version two), then it would specify the version two type
        /// ID in this parameter and all instances will be mapped to version two
        /// of the medication schema before being returned.
        /// Be careful when adding to this collection. If you are not specifying any
        /// <see cref="TransformsToApply"/> and the Microsoft.Health.ItemTypes namespace doesn't
        /// contain a class for parsing the type version you specify, you will get a
        /// <see cref="HealthRecordItem"/> instance rather than the type specific class instance.
        /// </remarks>
        ///
        public Collection<Guid> TypeVersionFormat { get; } = new Collection<Guid>();

        /// <summary>
        /// Gets a collection representing the names of the BLOBs to return for the item(s).
        /// </summary>
        ///
        /// <remarks>
        /// If no BLOB names are specified, all BLOBs will be returned.
        /// </remarks>
        ///
        public Collection<string> BlobNames { get; } = new Collection<string>();

        /// <summary>
        /// Gets the format of the BLOB that should be returned.
        /// </summary>
        ///
        /// <remarks>
        /// Defaults to <see cref="BlobFormat"/>.Unknown.
        /// </remarks>
        ///
        public BlobFormat BlobFormat { get; set; } = BlobFormat.Unknown;

        /// <summary>
        /// Gets a string representation of the instance.
        /// </summary>
        ///
        /// <returns>
        /// A string representing the XML used as the group portion of the
        /// XML request for a "GetThings" method call.
        /// </returns>
        ///
        public override string ToString()
        {
            StringBuilder result = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                this.AddViewXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }

        #region internal helpers

        internal static HealthRecordView CreateFromXml(XPathNavigator nav)
        {
            HealthRecordView view = new HealthRecordView();
            XPathNodeIterator sectionsIterator = nav.Select("section");
            foreach (XPathNavigator sectionNav in sectionsIterator)
            {
                switch (sectionNav.Value)
                {
                    case "core":
                        view.Sections |= HealthRecordItemSections.Core;
                        break;
                    case "audits":
                        view.Sections |= HealthRecordItemSections.Audits;
                        break;
                    case "blobpayload":
                        view.Sections |= HealthRecordItemSections.BlobPayload;
                        break;
                    case "effectivepermissions":
                        view.Sections |= HealthRecordItemSections.EffectivePermissions;
                        break;
                    case "tags":
                        view.Sections |= HealthRecordItemSections.Tags;
                        break;
                    case "digitalsignatures":
                        view.Sections |= HealthRecordItemSections.Signature;
                        break;
                }
            }

            XPathNodeIterator xmlTransformsIterator = nav.Select("xml");

            foreach (XPathNavigator xmlTransformNav in xmlTransformsIterator)
            {
                string transformName = xmlTransformNav.Value;
                if (transformName.Length == 0)
                {
                    view.Sections |= HealthRecordItemSections.Xml;
                }
                else
                {
                    view.TransformsToApply.Add(transformName);
                }
            }

            XPathNodeIterator typeFormatIterator = nav.Select("type-version-format");

            foreach (XPathNavigator typeFormatNav in typeFormatIterator)
            {
                Guid typeFormat = new Guid(typeFormatNav.Value);
                view.TypeVersionFormat.Add(typeFormat);
            }

            return view;
        }

        /// <summary>
        /// Constructs the XML for the format tag used in the
        /// "GetThings" request.
        /// </summary>
        ///
        /// <param name="writer">
        /// The XMLWriter that receives the format tag.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// No sections or transforms were specified for the search.
        /// </exception>
        ///
        internal void AddViewXml(XmlWriter writer)
        {
            // First check to be sure we either have sections and/or
            // transforms defined.
            Validator.ThrowArgumentExceptionIf(
                this.Sections == HealthRecordItemSections.None &&
                this.TransformsToApply.Count == 0,
                "sections",
                "NoSectionsOrTransforms");

            // <format>
            writer.WriteStartElement("format");

            this.AddSectionsXml(writer);
            this.AddTransformXml(writer);
            this.AddTypeFormatXml(writer);
            this.AddBlobFormatXml(writer);

            // </format>
            writer.WriteEndElement();
        }

        private void AddSectionsXml(XmlWriter writer)
        {
            if ((this.Sections & HealthRecordItemSections.Audits) != 0)
            {
                WriteSection(writer, "audits");
            }

            if ((this.Sections & HealthRecordItemSections.Core) != 0)
            {
                WriteSection(writer, "core");
            }

            if ((this.Sections & HealthRecordItemSections.EffectivePermissions) != 0)
            {
                WriteSection(writer, "effectivepermissions");
            }

            if ((this.Sections & HealthRecordItemSections.BlobPayload) != 0)
            {
                WriteSection(writer, "blobpayload");
            }

            if ((this.Sections & HealthRecordItemSections.Tags) != 0)
            {
                WriteSection(writer, "tags");
            }

            if ((this.Sections & HealthRecordItemSections.Signature) != 0)
            {
                WriteSection(writer, "digitalsignatures");
            }

            if ((this.Sections & HealthRecordItemSections.Xml) != 0)
            {
                writer.WriteStartElement("xml");
                writer.WriteEndElement();
            }
        }

        private static void WriteSection(XmlWriter writer, string sectionName)
        {
            writer.WriteStartElement("section");
            writer.WriteValue(sectionName);
            writer.WriteEndElement();
        }

        private void AddTransformXml(XmlWriter writer)
        {
            foreach (string transformName in this.TransformsToApply)
            {
                writer.WriteElementString("xml", transformName);
            }
        }

        private void AddTypeFormatXml(XmlWriter writer)
        {
            if (this.TypeVersionFormat.Count == 0 && !this.configuration.UseLegacyTypeVersionSupport)
            {
                // Add the supported type version formats from configuration.
                foreach (Guid typeFormat in this.configuration.SupportedTypeVersions)
                {
                    writer.WriteElementString("type-version-format", typeFormat.ToString());
                }
            }
            else
            {
                foreach (Guid typeFormat in this.TypeVersionFormat)
                {
                    writer.WriteElementString("type-version-format", typeFormat.ToString());
                }
            }
        }

        private void AddBlobFormatXml(XmlWriter writer)
        {
            if (this.BlobNames.Count > 0 || this.BlobFormat != BlobFormat.Unknown)
            {
                writer.WriteStartElement("blob-payload-request");

                if (this.BlobNames.Count > 0)
                {
                    writer.WriteStartElement("blob-filters");

                    for (int index = 0; index < this.BlobNames.Count; ++index)
                    {
                        writer.WriteStartElement("blob-filter");
                        writer.WriteElementString("blob-name", this.BlobNames[index]);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                BlobFormat format = this.BlobFormat;
                if (format == BlobFormat.Unknown)
                {
                    format = BlobFormat.Default;
                }

                writer.WriteStartElement("blob-format");
                writer.WriteElementString("blob-format-spec", format.ToString().ToLowerInvariant());
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion internal helpers
    }
}
