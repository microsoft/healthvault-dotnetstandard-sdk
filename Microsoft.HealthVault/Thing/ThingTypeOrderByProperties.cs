// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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