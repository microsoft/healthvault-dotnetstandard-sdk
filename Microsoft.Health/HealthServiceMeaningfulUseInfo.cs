// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.XPath;

namespace Microsoft.Health
{
    /// <summary>
    /// Provides configuration information for Meaningful Use features.
    /// </summary>
    public class HealthServiceMeaningfulUseInfo
    {
        /// <summary>
        /// Constructs a <see cref="HealthServiceMeaningfulUseInfo"/> instance.
        /// </summary>
        internal HealthServiceMeaningfulUseInfo()
        {
        }

        /// <summary>
        /// Indicates whether Meaningful Use features are enabled on this HealthVault instance.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// Gets the public configuration values for the HealthVault service's
        /// Meaningful Use features.
        /// </summary>
        /// 
        /// <value>
        /// The dictionary returned uses the configuration value name as the key. All entries are
        /// public configuration values that the HealthVault service exposes as information to
        /// HealthVault applications using Meaningful Use features.
        /// For example, the MaxMeaningfulUseReportItemsPerRetrieval configuration value specifies the maximum number
        /// of report entries returned per page for Meaningful Use reports.
        /// </value>
        /// 
        [SuppressMessage(
            "Microsoft.Usage",
            "CA2227:CollectionPropertiesShouldBeReadOnly",
            Justification = "Keep code consistent with ServiceInfo.")]
        public Dictionary<string, string> ConfigurationValues
        {
            get { return _configurationValues; }
            private set { _configurationValues = value; }
        }

        private Dictionary<string, string> _configurationValues = new Dictionary<string, string>();

        /// <summary>
        /// Constructs a <see cref="HealthServiceMeaningfulUseInfo"/> object from the  
        /// supplied XML.
        /// </summary>
        /// 
        /// <param name="nav">
        /// An <see cref="XPathNavigator"/> to access the XML from which the 
        /// <see cref="HealthServiceMeaningfulUseInfo"/> object will be constructed.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="HealthServiceMeaningfulUseInfo"/> object.
        /// </returns>
        public static HealthServiceMeaningfulUseInfo CreateMeaningfulUseInfo(
            XPathNavigator nav)
        {
            HealthServiceMeaningfulUseInfo info = new HealthServiceMeaningfulUseInfo();
            info.Enabled = nav.SelectSingleNode("enabled").ValueAsBoolean;
            info.ConfigurationValues = GetConfigurationValues(nav.Select("configuration"));
            return info;
        }

        private static Dictionary<string, string> GetConfigurationValues(
            XPathNodeIterator configIterator)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (XPathNavigator configNav in configIterator)
            {
                result.Add(configNav.GetAttribute("key", String.Empty), configNav.Value);
            }

            return result;
        }
    }
}