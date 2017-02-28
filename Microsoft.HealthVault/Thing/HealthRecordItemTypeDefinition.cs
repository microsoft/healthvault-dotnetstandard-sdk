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

namespace Microsoft.HealthVault.Things
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

            typeDefinition.TypeNavigator = typeNavigator;

            typeDefinition.ParseXml(typeNavigator);
            return typeDefinition;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthRecordItemTypeDefinition"/> class for use in testing.
        /// </summary>
        protected HealthRecordItemTypeDefinition()
        {
        }

        protected virtual void ParseXml(XPathNavigator typeNavigator)
        {
            string typeIdString = typeNavigator.SelectSingleNode("id").Value;
            this.TypeId = new Guid(typeIdString);
            this.Name = typeNavigator.SelectSingleNode("name").Value;

            XPathNavigator isCreatableNavigator = typeNavigator.SelectSingleNode("uncreatable");
            if (isCreatableNavigator != null)
            {
                this.IsCreatable = !isCreatableNavigator.ValueAsBoolean;
            }

            XPathNavigator isImmutableNavigator = typeNavigator.SelectSingleNode("immutable");
            if (isImmutableNavigator != null)
            {
                this.IsImmutable = isImmutableNavigator.ValueAsBoolean;
            }

            XPathNavigator isSingletonNavigator = typeNavigator.SelectSingleNode("singleton");
            if (isSingletonNavigator != null)
            {
                this.IsSingletonType = isSingletonNavigator.ValueAsBoolean;
            }

            XPathNavigator allowReadOnlyNavigator = typeNavigator.SelectSingleNode("allow-readonly");
            if (allowReadOnlyNavigator != null)
            {
                this.AllowReadOnly = allowReadOnlyNavigator.ValueAsBoolean;
            }

            XPathNavigator xsdNavigator = typeNavigator.SelectSingleNode("xsd");

            if (xsdNavigator != null)
            {
                this.XmlSchemaDefinition = xsdNavigator.Value;
            }
            else
            {
                this.XmlSchemaDefinition = string.Empty;
            }

            this.Versions = GetThingTypeVersions(typeNavigator);

            XPathNavigator effectiveDateXPath = typeNavigator.SelectSingleNode("effective-date-xpath");

            if (effectiveDateXPath != null)
            {
                this.EffectiveDateXPath = effectiveDateXPath.Value;
            }

            XPathNavigator updatedEndDateNavigator = typeNavigator.SelectSingleNode("updated-end-date-xpath");

            if (updatedEndDateNavigator != null)
            {
                this.UpdatedEndDateXPath = updatedEndDateNavigator.Value;
            }
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
            var versionTypeId = new Guid(versionInfoNav.GetAttribute("version-type-id", string.Empty));
            string versionName = versionInfoNav.GetAttribute("version-name", string.Empty);
            int versionSequence = int.Parse(versionInfoNav.GetAttribute("version-sequence", string.Empty), CultureInfo.InvariantCulture);

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
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the type unique identifier.
        /// </summary>
        ///
        /// <value>
        /// A GUID representing the type identifier.
        /// </value>
        ///
        public Guid TypeId { get; protected set; }

        /// <summary>
        /// Gets or sets the XML schema definition.
        /// </summary>
        ///
        /// <value>
        /// A string representing the definition.
        /// </value>
        ///
        public string XmlSchemaDefinition { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether instances of the type are creatable.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the instances are creatable; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool IsCreatable { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether instances of the type are immutable.
        /// </summary>
        ///
        /// <value>
        /// <b>true</b> if the instances are immutable; otherwise, <b>false</b>.
        /// </value>
        ///
        public bool IsImmutable { get; protected set; }

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
        public bool IsSingletonType { get; protected set; }

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
        /// Gets or sets a collection of the version information for the type.
        /// </summary>
        public ReadOnlyCollection<HealthRecordItemTypeVersionInfo> Versions { get; protected set; }

        /// <summary>
        /// Gets or sets the XPath to the effective date element in the <see cref="HealthRecordItem.TypeSpecificData"/>.
        /// </summary>
        ///
        /// <value>
        /// The String representation of the XPath.
        /// </value>
        ///
        public string EffectiveDateXPath { get; protected set; }

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

        public XPathNavigator TypeNavigator { get; protected set; }
    }
}
