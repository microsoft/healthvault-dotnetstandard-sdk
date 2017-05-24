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
using System.Globalization;
using System.Linq;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Describes the schema and structure of a thing type.
    /// </summary>
    ///
    public class ThingTypeDefinition
    {
        /// <summary>
        /// Constructs an instance of <see cref="ThingTypeDefinition"/> from the specified
        /// XML.
        /// </summary>
        ///
        /// <param name="typeNavigator">
        /// XML navigator containing the information needed to construct the instance. This XML
        /// must adhere to the schema for a ThingType as defined by response-getthingtype.xsd.
        /// </param>
        ///
        /// <returns>
        /// An instance of <see cref="ThingTypeDefinition"/> constructed from the
        /// specified XML.
        /// </returns>
        ///
        public static ThingTypeDefinition CreateFromXml(
            XPathNavigator typeNavigator)
        {
            ThingTypeDefinition typeDefinition =
                new ThingTypeDefinition();

            typeDefinition.TypeNavigator = typeNavigator;

            typeDefinition.ParseXml(typeNavigator);
            return typeDefinition;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ThingTypeDefinition"/> class for use in testing.
        /// </summary>
        protected ThingTypeDefinition()
        {
        }

        protected virtual void ParseXml(XPathNavigator typeNavigator)
        {
            string typeIdString = typeNavigator.SelectSingleNode("id").Value;
            TypeId = new Guid(typeIdString);
            Name = typeNavigator.SelectSingleNode("name").Value;

            XPathNavigator isCreatableNavigator = typeNavigator.SelectSingleNode("uncreatable");
            if (isCreatableNavigator != null)
            {
                IsCreatable = !isCreatableNavigator.ValueAsBoolean;
            }

            XPathNavigator isImmutableNavigator = typeNavigator.SelectSingleNode("immutable");
            if (isImmutableNavigator != null)
            {
                IsImmutable = isImmutableNavigator.ValueAsBoolean;
            }

            XPathNavigator isSingletonNavigator = typeNavigator.SelectSingleNode("singleton");
            if (isSingletonNavigator != null)
            {
                IsSingletonType = isSingletonNavigator.ValueAsBoolean;
            }

            XPathNavigator allowReadOnlyNavigator = typeNavigator.SelectSingleNode("allow-readonly");
            if (allowReadOnlyNavigator != null)
            {
                AllowReadOnly = allowReadOnlyNavigator.ValueAsBoolean;
            }

            XPathNavigator xsdNavigator = typeNavigator.SelectSingleNode("xsd");

            if (xsdNavigator != null)
            {
                XmlSchemaDefinition = xsdNavigator.Value;
            }
            else
            {
                XmlSchemaDefinition = string.Empty;
            }

            Versions = GetThingTypeVersions(typeNavigator);

            XPathNavigator effectiveDateXPath = typeNavigator.SelectSingleNode("effective-date-xpath");

            if (effectiveDateXPath != null)
            {
                EffectiveDateXPath = effectiveDateXPath.Value;
            }

            XPathNavigator updatedEndDateNavigator = typeNavigator.SelectSingleNode("updated-end-date-xpath");

            if (updatedEndDateNavigator != null)
            {
                UpdatedEndDateXPath = updatedEndDateNavigator.Value;
            }
        }

        private static ReadOnlyCollection<ThingTypeVersionInfo> GetThingTypeVersions(XPathNavigator typeNavigator)
        {
            XPathNodeIterator versionInfoIterator = typeNavigator.Select("versions/version-info");

            List<ThingTypeVersionInfo> versions =
                (from XPathNavigator versionInfoNav in versionInfoIterator select GetVersionInfoFromXml(versionInfoNav)).ToList();

            return new ReadOnlyCollection<ThingTypeVersionInfo>(versions);
        }

        private static ThingTypeVersionInfo GetVersionInfoFromXml(XPathNavigator versionInfoNav)
        {
            var versionTypeId = new Guid(versionInfoNav.GetAttribute("version-type-id", string.Empty));
            string versionName = versionInfoNav.GetAttribute("version-name", string.Empty);
            int versionSequence = int.Parse(versionInfoNav.GetAttribute("version-sequence", string.Empty), CultureInfo.InvariantCulture);

            XPathNavigator orderByPropertiesNav = versionInfoNav.SelectSingleNode("order-by-properties");

            if (orderByPropertiesNav == null)
            {
                return new ThingTypeVersionInfo(
                    versionTypeId,
                    versionName,
                    versionSequence,
                    new ThingTypeOrderByProperties(new List<ThingTypeProperty>(0)));
            }

            ThingTypeOrderByProperties orderByProperties = ThingTypeOrderByProperties.CreateFromXml(orderByPropertiesNav);

            return new ThingTypeVersionInfo(
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
        /// Gets or sets a value indicating whether the ThingBase Type allows read-only
        /// instances.
        /// </summary>
        /// <value>
        /// <b>true</b> if the ThingBase Type allows read-only instances; otherwise <b>false</b>
        /// </value>
        public bool AllowReadOnly
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a collection of the version information for the type.
        /// </summary>
        public ReadOnlyCollection<ThingTypeVersionInfo> Versions { get; protected set; }

        /// <summary>
        /// Gets or sets the XPath to the effective date element in the <see cref="ThingBase.TypeSpecificData"/>.
        /// </summary>
        ///
        /// <value>
        /// The String representation of the XPath.
        /// </value>
        ///
        public string EffectiveDateXPath { get; protected set; }

        /// <summary>
        /// Gets or sets the XPath to the updated end date element in the <see cref="ThingBase.TypeSpecificData"/>.
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
