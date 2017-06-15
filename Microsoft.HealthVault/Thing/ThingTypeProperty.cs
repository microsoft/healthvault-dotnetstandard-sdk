// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The property that the thing-type can be
    /// ordered by in the result.
    /// </summary>
    public class ThingTypeProperty
    {
        /// <summary>
        /// The property that the thing-type can be ordered by in the result.
        /// </summary>
        public ThingTypeProperty(
            string name,
            string type,
            string xpath,
            IItemTypePropertyConversion conversion = null)
        {
            Name = name;
            Type = type;
            _xpath = xpath;
            Conversion = conversion;
        }

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The data type for the property.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Naming",
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "We don't want to the change the name specified in the schema definition.")]
        public string Type { get; }

        /// <summary>
        /// The xpath for the property.
        /// </summary>
        public string Xpath => !string.IsNullOrEmpty(_xpath) ? _xpath : null;

        private readonly string _xpath;

        /// <summary>
        /// A units conversion to apply to the value of a property of numeric type.
        /// </summary>
        /// <remarks>
        /// When a thing type has multiple versions that store the same data with different units,
        /// a conversion between units may be required to ensure that values are ordered correctly
        /// across versions.
        /// </remarks>
        public IItemTypePropertyConversion Conversion { get; }

        /// <summary>
        /// This method converts the Property xml to the
        /// ThingTypeProperty object.
        /// </summary>
        public static ThingTypeProperty CreateFromXml(XPathNavigator propertyNav)
        {
            string name = propertyNav.GetAttribute("name", string.Empty);
            string type = propertyNav.GetAttribute("type", string.Empty);
            string xpath = propertyNav.GetAttribute("xpath", string.Empty);
            IItemTypePropertyConversion conversion = null;

            XPathNavigator conversionNav = propertyNav.SelectSingleNode("conversion/linear-conversion");

            if (conversionNav != null)
            {
                conversion = LinearItemTypePropertyConversion.CreateFromXml(conversionNav);
            }

            return new ThingTypeProperty(name, type, xpath, conversion);
        }
    }
}