// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
{

    /// <summary>
    /// Application specific settings for the user.
    /// </summary>
    /// 
    /// <remarks>
    /// HealthVault can maintain settings for a specific user and application on behalf of the 
    /// application. You may think of this like the HKEY_CURRENT_USER part of the Windows Registry
    /// for HealthVault.
    /// </remarks>
    /// 
    public class ApplicationSettings
    {
        /// <summary>
        /// Constructs an ApplicationSettings instance with default values.
        /// </summary>
        /// 
        public ApplicationSettings()
        {
        }

        internal void ParseXml(XPathNavigator settingsRootNav)
        {
            XPathNavigator appSettingsNav = settingsRootNav.SelectSingleNode("app-settings");

            if (appSettingsNav != null)
            {
                using (StringReader stringReader = new StringReader(appSettingsNav.OuterXml))
                {
                    _xmlSettings = new XPathDocument(XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings));
                }
            }

            XPathNavigator selectedRecordIdNav =
                settingsRootNav.SelectSingleNode("selected-record-id");
            if (selectedRecordIdNav != null)
            {
                _selectedRecordId = new Guid(selectedRecordIdNav.Value);
            }
        }

        /// <summary>
        /// Gets or sets the application specific XML settings that the application has set for the user.
        /// </summary>
        /// 
        public IXPathNavigable XmlSettings
        {
            get { return _xmlSettings; }
            set { _xmlSettings = value; }
        }
        private IXPathNavigable _xmlSettings;

        /// <summary>
        /// Gets or sets the selected health record identifier that the user has chosen to use with this
        /// application.
        /// </summary>
        /// 
        public Guid SelectedRecordId
        {
            get { return _selectedRecordId; }
            set { _selectedRecordId = value; }
        }
        private Guid _selectedRecordId;
    }

}
