// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about the HealthVault methods.
    /// </summary>
    ///
    public class HealthServiceMethodInfo
    {
        internal static HealthServiceMethodInfo CreateMethodInfo(
            XPathNavigator nav)
        {
            HealthServiceMethodInfo methodInfo =
                new HealthServiceMethodInfo();

            methodInfo.ParseXml(nav);
            return methodInfo;
        }

        internal void ParseXml(XPathNavigator nav)
        {
            Name = nav.SelectSingleNode("name").Value;

            XPathNodeIterator versionIterator =
                nav.Select("version");

            Collection<HealthServiceMethodVersionInfo> versions =
                new Collection<HealthServiceMethodVersionInfo>();
            foreach (XPathNavigator versionNav in versionIterator)
            {
                HealthServiceMethodVersionInfo methodVersion =
                    new HealthServiceMethodVersionInfo(this);
                methodVersion.ParseXml(versionNav);
                versions.Add(methodVersion);
            }

            Versions =
                new ReadOnlyCollection<HealthServiceMethodVersionInfo>(
                    versions);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceMethodInfo"/> class with default values.
        /// </summary>
        public HealthServiceMethodInfo()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceMethodInfo"/> class and sets
        /// the name and version information.
        /// </summary>
        /// <param name="name">The name of the method.</param>
        /// <param name="versions">A collection of version information.</param>
        public HealthServiceMethodInfo(
            string name,
            ReadOnlyCollection<HealthServiceMethodVersionInfo> versions)
        {
            Name = name;
            Versions = versions;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        ///
        /// <value>
        /// A string representing the method name.
        /// </value>
        ///
        public string Name { get; private set; }

        /// <summary>
        /// Gets the information about the supported versions of the method.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection containing version information.
        /// </value>
        ///
        public ReadOnlyCollection<HealthServiceMethodVersionInfo> Versions { get; private set; }
    }
}
