// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
    /// querying things with the "GetThings" method.
    /// </remarks>
    ///
    [DebuggerDisplay("Sections = {Sections}")]
    public class HealthRecordView
    {
        private readonly HealthVaultConfiguration _configuration;

        /// <summary>
        /// Gets or sets the sections that will be retrieved when the
        /// query is made.
        /// </summary>
        ///
        /// <value>
        /// An instance of <see cref="ThingSections"/>. The default
        /// values are Core and XML.
        /// </value>
        ///
        public ThingSections Sections
        {
            get { return _sections; }

            set
            {
                // Always add in Core so that we get the type-id element
                _sections = value | ThingSections.Core;
            }
        }

        private ThingSections _sections =
            ThingSections.Core |
            ThingSections.Xml;

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

        public HealthRecordView(HealthVaultConfiguration configuration)
        {
            _configuration = configuration;
        }

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
        /// <see cref="ThingBase"/> instance rather than the type specific class instance.
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
                AddViewXml(writer);
                writer.Flush();
            }

            return result.ToString();
        }

        #region internal helpers

        internal static HealthRecordView CreateFromXml(XPathNavigator nav, HealthVaultConfiguration configuration)
        {
            HealthRecordView view = new HealthRecordView(configuration);
            XPathNodeIterator sectionsIterator = nav.Select("section");
            foreach (XPathNavigator sectionNav in sectionsIterator)
            {
                switch (sectionNav.Value)
                {
                    case "core":
                        view.Sections |= ThingSections.Core;
                        break;
                    case "audits":
                        view.Sections |= ThingSections.Audits;
                        break;
                    case "blobpayload":
                        view.Sections |= ThingSections.BlobPayload;
                        break;
                    case "effectivepermissions":
                        view.Sections |= ThingSections.EffectivePermissions;
                        break;
                    case "tags":
                        view.Sections |= ThingSections.Tags;
                        break;
                    case "digitalsignatures":
                        view.Sections |= ThingSections.Signature;
                        break;
                }
            }

            XPathNodeIterator xmlTransformsIterator = nav.Select("xml");

            foreach (XPathNavigator xmlTransformNav in xmlTransformsIterator)
            {
                string transformName = xmlTransformNav.Value;
                if (transformName.Length == 0)
                {
                    view.Sections |= ThingSections.Xml;
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
            if (Sections == ThingSections.None && TransformsToApply.Count == 0)
            {
                throw new ArgumentException(Resources.NoSectionsOrTransforms, nameof(_sections));
            }

            // <format>
            writer.WriteStartElement("format");

            AddSectionsXml(writer);
            AddTransformXml(writer);
            AddTypeFormatXml(writer);
            AddBlobFormatXml(writer);

            // </format>
            writer.WriteEndElement();
        }

        private void AddSectionsXml(XmlWriter writer)
        {
            if ((Sections & ThingSections.Audits) != 0)
            {
                WriteSection(writer, "audits");
            }

            if ((Sections & ThingSections.Core) != 0)
            {
                WriteSection(writer, "core");
            }

            if ((Sections & ThingSections.EffectivePermissions) != 0)
            {
                WriteSection(writer, "effectivepermissions");
            }

            if ((Sections & ThingSections.BlobPayload) != 0)
            {
                WriteSection(writer, "blobpayload");
            }

            if ((Sections & ThingSections.Tags) != 0)
            {
                WriteSection(writer, "tags");
            }

            if ((Sections & ThingSections.Signature) != 0)
            {
                WriteSection(writer, "digitalsignatures");
            }

            if ((Sections & ThingSections.Xml) != 0)
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
            foreach (string transformName in TransformsToApply)
            {
                writer.WriteElementString("xml", transformName);
            }
        }

        private void AddTypeFormatXml(XmlWriter writer)
        {
            if (TypeVersionFormat.Count == 0 && !_configuration.UseLegacyTypeVersionSupport)
            {
                // Add the supported type version formats from configuration.
                foreach (Guid typeFormat in _configuration.SupportedTypeVersions)
                {
                    writer.WriteElementString("type-version-format", typeFormat.ToString());
                }
            }
            else
            {
                foreach (Guid typeFormat in TypeVersionFormat)
                {
                    writer.WriteElementString("type-version-format", typeFormat.ToString());
                }
            }
        }

        private void AddBlobFormatXml(XmlWriter writer)
        {
            if (BlobNames.Count > 0 || BlobFormat != BlobFormat.Unknown)
            {
                writer.WriteStartElement("blob-payload-request");

                if (BlobNames.Count > 0)
                {
                    writer.WriteStartElement("blob-filters");

                    for (int index = 0; index < BlobNames.Count; ++index)
                    {
                        writer.WriteStartElement("blob-filter");
                        writer.WriteElementString("blob-name", BlobNames[index]);
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                BlobFormat format = BlobFormat;
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
