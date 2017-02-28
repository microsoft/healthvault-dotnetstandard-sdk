// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Things
{
    /// <summary>
    /// The property that the thing-type can be
    /// ordered by in the result.
    /// </summary>
    public class HealthRecordItemTypeProperty
    {
        /// <summary>
        /// The property that the thing-type can be ordered by in the result.
        /// </summary>
        public HealthRecordItemTypeProperty(
            string name,
            string type,
            string xpath,
            IItemTypePropertyConversion conversion = null)
        {
            this.Name = name;
            this.Type = type;
            this.xpath = xpath;
            this.Conversion = conversion;
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
        public string Xpath => !string.IsNullOrEmpty(this.xpath) ? this.xpath : null;

        private readonly string xpath;

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
        /// HealthRecordItemTypeProperty object.
        /// </summary>
        public static HealthRecordItemTypeProperty CreateFromXml(XPathNavigator propertyNav)
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

            return new HealthRecordItemTypeProperty(name, type, xpath, conversion);
        }
    }
}