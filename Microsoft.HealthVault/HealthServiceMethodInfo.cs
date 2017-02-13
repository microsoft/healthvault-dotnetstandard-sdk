// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System.Collections.ObjectModel;
using System.Xml.XPath;

namespace Microsoft.HealthVault
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
            _name = nav.SelectSingleNode("name").Value;

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
            _versions =
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
            _name = name;
            _versions = versions;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        /// 
        /// <value>
        /// A string representing the method name.
        /// </value>
        /// 
        public string Name
        {
            get { return _name; }
        }
        private string _name;

        /// <summary>
        /// Gets the information about the supported versions of the method.
        /// </summary>
        /// 
        /// <value>
        /// A read-only collection containing version information.
        /// </value>
        /// 
        public ReadOnlyCollection<HealthServiceMethodVersionInfo> Versions
        {
            get { return _versions; }
        }
        private ReadOnlyCollection<HealthServiceMethodVersionInfo> _versions;

    }

}
