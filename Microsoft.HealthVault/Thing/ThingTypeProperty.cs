// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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