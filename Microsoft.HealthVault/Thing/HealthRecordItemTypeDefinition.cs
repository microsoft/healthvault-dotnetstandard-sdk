// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
    }
}
