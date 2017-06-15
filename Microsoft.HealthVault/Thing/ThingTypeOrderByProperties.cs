// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// The set of properties that the thing-type can be
    /// ordered by in the result.
    /// </summary>
    public class ThingTypeOrderByProperties
    {
        /// <summary>
        /// The set of properties that the thing-type can be ordered by in the result.
        /// </summary>
        public ThingTypeOrderByProperties(List<ThingTypeProperty> properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// The set of properties that the thing-type can be
        /// ordered by in the result..
        /// </summary>
        public List<ThingTypeProperty> Properties => _properties ?? new List<ThingTypeProperty>();

        private readonly List<ThingTypeProperty> _properties;

        /// <summary>
        /// This method converts the OrderByProperties xml to the
        /// ThingTypeOrderByProperties object.
        /// </summary>
        public static ThingTypeOrderByProperties CreateFromXml(XPathNavigator orderByPropertiesNav)
        {
            XPathNodeIterator propertiesIter = orderByPropertiesNav.Select("property");

            List<ThingTypeProperty> properties =
                (from XPathNavigator propertiesNav in propertiesIter select GetPropertyFromXml(propertiesNav)).ToList();

            return new ThingTypeOrderByProperties(properties);
        }

        private static ThingTypeProperty GetPropertyFromXml(XPathNavigator propertyNav)
        {
            return ThingTypeProperty.CreateFromXml(propertyNav);
        }
    }
}