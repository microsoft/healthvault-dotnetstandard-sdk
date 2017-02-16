// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using Microsoft.HealthVault.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Describes the schema and structure of a health record item type.
    /// </summary>
    ///
    public class HealthRecordItemTypeDefinition
    {
        /// <summary>
        /// Constructs an instance of <see cref="HealthRecordItemTypeDefinition"/> from the specified
        /// XML.
        /// </summary>
        ///
        /// <param name="typeNavigator">
        /// XML navigator containing the information needed to construct the instance. This XML
        /// must adhere to the schema for a ThingType as defined by response-getthingtype.xsd.
        /// </param>
        ///
        /// <returns>
        /// An instance of <see cref="HealthRecordItemTypeDefinition"/> constructed from the
        /// specified XML.
        /// </returns>
        ///
        public static HealthRecordItemTypeDefinition CreateFromXml(
            XPathNavigator typeNavigator)
        {
            HealthRecordItemTypeDefinition typeDefinition =
                new HealthRecordItemTypeDefinition();

            typeDefinition.ParseXml(typeNavigator);
            return typeDefinition;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemTypeDefinition"/> class for use in testing.
        /// </summary>
        protected HealthRecordItemTypeDefinition()
        {
        }

        private void ParseXml(XPathNavigator typeNavigator)
        {
            string typeIdString = typeNavigator.SelectSingleNode("id").Value;
            _id = new Guid(typeIdString);
            _name = typeNavigator.SelectSingleNode("name").Value;

            XPathNavigator isCreatableNavigator = typeNavigator.SelectSingleNode("uncreatable");
            if (isCreatableNavigator != null)
            {
                _isCreatable = !isCreatableNavigator.ValueAsBoolean;
            }

            XPathNavigator isImmutableNavigator = typeNavigator.SelectSingleNode("immutable");
            if (isImmutableNavigator != null)
            {
                _isImmutable = isImmutableNavigator.ValueAsBoolean;
            }

            XPathNavigator isSingletonNavigator = typeNavigator.SelectSingleNode("singleton");
            if (isSingletonNavigator != null)
            {
                _isSingletonType = isSingletonNavigator.ValueAsBoolean;
            }

            XPathNavigator allowReadOnlyNavigator = typeNavigator.SelectSingleNode("allow-readonly");
            if (allowReadOnlyNavigator != null)
            {
                AllowReadOnly = allowReadOnlyNavigator.ValueAsBoolean;
            }

            XPathNavigator xsdNavigator = typeNavigator.SelectSingleNode("xsd");

            if (xsdNavigator != null)
            {
                _xsd = xsdNavigator.Value;
            }
            else
            {
                _xsd = String.Empty;
            }

            _columns = GetThingTypeColumns(typeNavigator);

            var transforms = new List<string>();
            XPathNodeIterator transformsIterator = typeNavigator.Select("transforms/tag");

            foreach (XPathNavigator transformsNav in transformsIterator)
            {
                transforms.Add(transformsNav.Value);
            }

            _supportedTransformNames = new ReadOnlyCollection<string>(transforms);

            XPathNodeIterator transformSourceIterator = typeNavigator.Select("transform-source");

            foreach (XPathNavigator transformSourceNav in transformSourceIterator)
            {
                string transformTag = transformSourceNav.GetAttribute("tag", String.Empty);
                _transformSource.Add(transformTag, transformSourceNav.Value);
            }

            _versions = GetThingTypeVersions(typeNavigator);

            XPathNavigator effectiveDateXPath = typeNavigator.SelectSingleNode("effective-date-xpath");

            if (effectiveDateXPath != null)
            {
                _effectiveDateXPath = effectiveDateXPath.Value;
            }

            XPathNavigator updatedEndDateNavigator = typeNavigator.SelectSingleNode("updated-end-date-xpath");

            if (updatedEndDateNavigator != null)
            {
                UpdatedEndDateXPath = updatedEndDateNavigator.Value;
            }
        }

        private static ReadOnlyCollection<ItemTypeDataColumn> GetThingTypeColumns(XPathNavigator typeNavigator)
        {
            XPathNodeIterator columnIterator = typeNavigator.Select("columns/column");

            var columns = (from XPathNavigator columnNav in columnIterator select ItemTypeDataColumn.CreateFromXml(columnNav)).ToList();

            return new ReadOnlyCollection<ItemTypeDataColumn>(columns);
        }

        private static ReadOnlyCollection<HealthRecordItemTypeVersionInfo> GetThingTypeVersions(XPathNavigator typeNavigator)
        {
            XPathNodeIterator versionInfoIterator = typeNavigator.Select("versions/version-info");

            List<HealthRecordItemTypeVersionInfo> versions =
                (from XPathNavigator versionInfoNav in versionInfoIterator select GetVersionInfoFromXml(versionInfoNav)).ToList();

            return new ReadOnlyCollection<HealthRecordItemTypeVersionInfo>(versions);
        }

        private static HealthRecordItemTypeVersionInfo GetVersionInfoFromXml(XPathNavigator versionInfoNav)
        {
            var versionTypeId = new Guid(versionInfoNav.GetAttribute("version-type-id", ""));
            string versionName = versionInfoNav.GetAttribute("version-name", "");
            int versionSequence = int.Parse(versionInfoNav.GetAttribute("version-sequence", ""), CultureInfo.InvariantCulture);

            XPathNavigator orderByPropertiesNav = versionInfoNav.SelectSingleNode("order-by-properties");

            if (orderByPropertiesNav == null)
            {
                return new HealthRecordItemTypeVersionInfo(
                    versionTypeId,
                    versionName,
                    versionSequence,
                    new HealthRecordItemTypeOrderByProperties(new List<HealthRecordItemTypeProperty>(0)));
            }

            HealthRecordItemTypeOrderByProperties orderByProperties = HealthRecordItemTypeOrderByProperties.CreateFromXml(orderByPropertiesNav);

            return new HealthRecordItemTypeVersionInfo(
                versionTypeId,
                versionName,
                versionSequence,
                orderByProperties);
        }

        /// <summary>
        /// Gets or sets the type name.
        /// </summary>
        ///
        /// <value>
        /// A string representing the type name.
        /// </value>
        ///
        public string Name
        {
            get { return _name; }
            protected set { _name = value; }
        }
        private string _name;

        /// <summary>
        /// Gets or sets the type unique identifier.
        /// </summary>
        ///
        /// <value>
        /// A GUID representing the type identifier.
        /// </value>
        ///
        public Guid TypeId
        {
            get { return _id; }
            protected set { _id = value; }
        }
        private Guid _id;

        /// <summary>
        /// Gets or sets the XML schema definition.
        /// </summary>
        ///
        /// <value>
        /// A string representing the definition.
        /// </value>
        ///
        public string XmlSchemaDefinition
        {
            get { return _xsd; }
            protected set { _xsd = value; }
        }
        private string _xsd;

        /// <summary>
        /// Gets the schema definition of the type.
        /// </summary>
        public XmlSchema TypeSchema
        {
            get
            {
                XmlSchema result = null;

                if (!String.IsNullOrEmpty(XmlSchemaDefinition))
                {
                    using (XmlReader xmlReader = SDKHelper.GetXmlReaderForXml(XmlSchemaDefinition))
                    {
                        result = XmlSchema.Read(xmlReader, null);
                        result.SourceUri =
                            HealthApplicationConfiguration.Current.GetHealthVaultTypeSchemaUrl().OriginalString;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the XmlSchemaSet that can be used to validate an XML fragment of
        /// this health record item type.
        /// </summary>
        public XmlSchemaSet ValidationSchemaSet
        {
            get
            {
                XmlSchemaSet result = null;

                if (!String.IsNullOrEmpty(XmlSchemaDefinition))
                {
                    result = new XmlSchemaSet();
                    result.XmlResolver =
                        new HealthVaultXmlResolver(
                            HealthApplicationConfiguration.Current.GetHealthVaultTypeSchemaUrl());

                    result.Add(TypeSchema);

                    result.Compile();
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether instances of the type are creatable.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the instances are creatable; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool IsCreatable
        {
            get { return _isCreatable; }
            protected set { _isCreatable = value; }
        }
        private bool _isCreatable;

        /// <summary>
        /// Gets or sets a value indicating whether instances of the type are immutable.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the instances are immutable; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool IsImmutable
        {
            get { return _isImmutable; }
            protected set { _isImmutable = value; }
        }
        private bool _isImmutable;

        /// <summary>
        /// Gets or sets a value indicating whether only a single instance of the type
        /// can exist for each health record.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if only a single instance of the type can exist for each
        /// health record; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool IsSingletonType
        {
            get { return _isSingletonType; }
            protected set { _isSingletonType = value; }
        }
        private bool _isSingletonType;

        /// <summary>
        /// Gets or sets a value indicating whether the Thing Type allows read-only
        /// instances.
        /// </summary>
        /// <value>
        /// <b>true</b> if the Thing Type allows read-only instances; otherwise <b>false</b>
        /// </value>
        public bool AllowReadOnly
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the column definitions when dealing with the type as a
        /// single type table.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection containing the defintions.
        /// </value>
        ///
        public ReadOnlyCollection<ItemTypeDataColumn> ColumnDefinitions
        {
            get { return _columns; }
            protected set { _columns = value; }
        }
        private ReadOnlyCollection<ItemTypeDataColumn> _columns =
            new ReadOnlyCollection<ItemTypeDataColumn>(new ItemTypeDataColumn[0]);

        /// <summary>
        /// Gets or sets the HealthVault transform names supported by the type.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection containing the transforms.
        /// </value>
        ///
        public ReadOnlyCollection<string> SupportedTransformNames
        {
            get { return _supportedTransformNames; }
            protected set { _supportedTransformNames = value; }
        }
        private ReadOnlyCollection<string> _supportedTransformNames =
            new ReadOnlyCollection<string>(new string[0]);

        /// <summary>
        /// Gets or sets the HealthVault transforms supported by the type.
        /// </summary>
        ///
        /// <value>
        /// A dictionary containing each of the transforms supported by the type. The key is the
        /// transform name and the value is the source of the transform.
        /// </value>
        ///
        /// <remarks>
        /// The transform can be run by calling one of the <see cref="TransformItem"/> overloads.
        /// </remarks>
        ///
        public Dictionary<string, string> TransformSource
        {
            get { return _transformSource; }
            protected set { _transformSource = value; }
        }
        private Dictionary<string, string> _transformSource =
            new Dictionary<string, string>();

        private Dictionary<string, XslCompiledTransform> _compiledTransform =
            new Dictionary<string, XslCompiledTransform>();

        /// <summary>
        /// Gets or sets a collection of the version information for the type.
        /// </summary>
        public ReadOnlyCollection<HealthRecordItemTypeVersionInfo> Versions
        {
            get { return _versions; }
            protected set { _versions = value; }
        }
        private ReadOnlyCollection<HealthRecordItemTypeVersionInfo> _versions;

        /// <summary>
        /// Gets or sets the XPath to the effective date element in the <see cref="HealthRecordItem.TypeSpecificData"/>.
        /// </summary>
        ///
        /// <value>
        /// The String representation of the XPath.
        /// </value>
        ///
        public string EffectiveDateXPath
        {
            get { return _effectiveDateXPath; }
            protected set { _effectiveDateXPath = value; }
        }
        private string _effectiveDateXPath;

        /// <summary>
        /// Gets or sets the XPath to the updated end date element in the <see cref="HealthRecordItem.TypeSpecificData"/>.
        /// </summary>
        ///
        /// <value>
        /// The String representation of the XPath.
        /// </value>
        ///
        public string UpdatedEndDateXPath
        {
            get;
            protected set;
        }

        #region Schema validation

        /// <summary>
        /// Validates the type specific XML is valid based on item type definition schema.
        /// </summary>
        ///
        /// <param name="typeSpecificXml">
        /// The type specific portion of the XML for a health record item. It does not include
        /// the header information like type-id, thing-id, etc.
        /// </param>
        ///
        /// <returns>
        /// True if <paramref name="typeSpecificXml"/> is schema valid according to
        /// this type definition.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="typeSpecificXml"/> is <b>null</b> or empty.
        /// </exception>
        ///
        /// <exception cref="XmlSchemaException">
        /// If <paramref name="typeSpecificXml"/> is not valid according to
        /// this type definition.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        public bool IsItemXmlSchemaValid(string typeSpecificXml)
        {
            Validator.ThrowIfStringNullOrEmpty(typeSpecificXml, "typeSpecificXml");

            XmlReaderSettings settings = SDKHelper.XmlReaderSettings;
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = ValidationSchemaSet;
            settings.NameTable = ValidationSchemaSet.NameTable;
            settings.ValidationFlags =
                XmlSchemaValidationFlags.ReportValidationWarnings
                    | XmlSchemaValidationFlags.AllowXmlAttributes;
            settings.XmlResolver = null;

            string itemXml =
                GetQualifiedThingXml(
                    TypeSchema.TargetNamespace,
                    typeSpecificXml);

            using (StringReader stringReader = new StringReader(itemXml))
            {
                using (XmlReader reader = XmlReader.Create(stringReader, settings))
                {
                    while (reader.Read())
                    {
                    }
                }
            }

            return true;
        }

        private static string GetQualifiedThingXml(
            string typeNamespace,
            string thingXml)
        {
            XmlDocument unqualifiedthing = new XmlDocument();
            unqualifiedthing.XmlResolver = null;
            unqualifiedthing.SafeLoadXml(thingXml);

            if (String.IsNullOrEmpty(unqualifiedthing.DocumentElement.NamespaceURI))
            {
                thingXml = String.Format(
                    CultureInfo.InvariantCulture,
                    "<tt:{0} xmlns:tt=\"{1}\">{2}</tt:{0}>",
                    unqualifiedthing.DocumentElement.Name,
                    typeNamespace,
                    unqualifiedthing.DocumentElement.InnerXml);
            }

            return thingXml;
        }

        #endregion Schema validation

        #region TransformItem

        /// <summary>
        /// Transforms the XML of the specified health record item using the specified transform.
        /// </summary>
        ///
        /// <param name="transformName">
        /// The name of the transform to use. Supported transforms for the type can be found in the
        /// <see cref="SupportedTransformNames"/> collection.
        /// </param>
        ///
        /// <param name="item">
        /// The health record item to be transformed.
        /// </param>
        ///
        /// <returns>
        /// A string containing the results of the transform.
        /// </returns>
        ///
        /// <remarks>
        /// If the transform has been used before a cached instance of the compiled transform will
        /// be used. Compiled transforms are not thread safe. It is up to the caller to ensure
        /// that multiple threads do not attempt to use the same transform at the same time.
        /// </remarks>
        ///
        /// <exception cref="KeyNotFoundException">
        /// If <paramref name="transformName"/> could not be found in the <see cref="TransformSource"/>
        /// collection.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        public string TransformItem(string transformName, HealthRecordItem item)
        {
            XslCompiledTransform transform = GetTransform(transformName);

            string thingXml = item.GetItemXml();

            StringBuilder result = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            XsltArgumentList args = new XsltArgumentList();

            args.AddParam("culture", String.Empty, System.Globalization.CultureInfo.CurrentUICulture.Name);
            args.AddParam("typename", String.Empty, this.Name);
            args.AddParam("typeid", String.Empty, this.TypeId.ToString());

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                using (StringReader stringReader = new StringReader(thingXml))
                {
                    using (XmlReader reader = XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings))
                    {
                        XPathDocument thingXmlDoc = new XPathDocument(reader);
                        transform.Transform(thingXmlDoc.CreateNavigator(), args, writer, null);
                    }
                }

                writer.Flush();
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets a compiled version of the specified transform.
        /// </summary>
        ///
        /// <param name="transformName">
        /// The name of the transform to get.
        /// </param>
        ///
        /// <returns>
        /// A compiled version of the specified transform.
        /// </returns>
        ///
        /// <exception cref="KeyNotFoundException">
        /// If <paramref name="transformName"/> is not found as a transform for the item type.
        /// </exception>
        ///
        /// <exception cref="XmlException">
        /// There is a load or parse error in the specified transform.
        /// </exception>
        ///
        /// <exception cref="XsltException">
        /// The specified style sheet contains an error.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        public XslCompiledTransform GetTransform(string transformName)
        {
            if (!_transformSource.ContainsKey(transformName))
            {
                throw new KeyNotFoundException(
                    ResourceRetriever.FormatResourceString("TransformNameNotFound", transformName));
            }

            XslCompiledTransform transform = null;

            lock (_compiledTransform)
            {
                if (_compiledTransform.ContainsKey(transformName))
                {
                    transform = _compiledTransform[transformName];
                }
                else
                {
                    using (StringReader stringReader = new StringReader(_transformSource[transformName]))
                    {
                        using (XmlReader reader = XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings))
                        {
                            transform = new XslCompiledTransform();
                            XsltSettings settings = new XsltSettings(false, true);
                            XPathDocument transformDoc = new XPathDocument(reader);
                            transform.Load(transformDoc.CreateNavigator(), settings, null);
                            _compiledTransform.Add(transformName, transform);
                        }
                    }
                }
            }

            return transform;
        }

        #endregion TransformItem
    }
}
