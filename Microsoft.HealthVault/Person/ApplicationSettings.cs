// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;

namespace Microsoft.HealthVault.Person
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
        internal void ParseXml(XPathNavigator settingsRootNav)
        {
            XPathNavigator appSettingsNav = settingsRootNav.SelectSingleNode("app-settings");

            if (appSettingsNav != null)
            {
                using (StringReader stringReader = new StringReader(appSettingsNav.OuterXml))
                {
                    XmlSettings = new XPathDocument(XmlReader.Create(stringReader, SDKHelper.XmlReaderSettings));
                }
            }

            XPathNavigator selectedRecordIdNav =
                settingsRootNav.SelectSingleNode("selected-record-id");
            if (selectedRecordIdNav != null)
            {
                SelectedRecordId = new Guid(selectedRecordIdNav.Value);
            }
        }

        /// <summary>
        /// Gets or sets the application specific XML settings that the application has set for the user.
        /// </summary>
        ///
        public IXPathNavigable XmlSettings { get; set; }

        /// <summary>
        /// Gets or sets the selected health record identifier that the user has chosen to use with this
        /// application.
        /// </summary>
        ///
        public Guid SelectedRecordId { get; set; }
    }
}
