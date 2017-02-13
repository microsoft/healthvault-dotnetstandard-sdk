// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// The set of properties that the thing-type can be 
    /// ordered by in the result.
    /// </summary> 
    public class HealthRecordItemTypeOrderByProperties
    {
        /// <summary>
        /// The set of properties that the thing-type can be ordered by in the result.
        /// </summary>
        public HealthRecordItemTypeOrderByProperties(List<HealthRecordItemTypeProperty> properties)
        {
            _properties = properties;
        }

        /// <summary>
        /// The set of properties that the thing-type can be 
        /// ordered by in the result..
        /// </summary>
        public List<HealthRecordItemTypeProperty> Properties
        {
            get { return _properties ?? new List<HealthRecordItemTypeProperty>(); }
        }

        private readonly List<HealthRecordItemTypeProperty> _properties;

        /// <summary>
        /// This method converts the OrderByProperties xml to the
        /// HealthRecordItemTypeOrderByProperties object. 
        /// </summary>
        public static HealthRecordItemTypeOrderByProperties CreateFromXml(XPathNavigator orderByPropertiesNav)
        {
            XPathNodeIterator propertiesIter = orderByPropertiesNav.Select("property");
            
            List<HealthRecordItemTypeProperty> properties =
                (from XPathNavigator propertiesNav in propertiesIter select GetPropertyFromXml(propertiesNav)).ToList();

            return new HealthRecordItemTypeOrderByProperties(properties);
        }

        private static HealthRecordItemTypeProperty GetPropertyFromXml(XPathNavigator propertyNav)
        {
            return HealthRecordItemTypeProperty.CreateFromXml(propertyNav);
        }
    }
}