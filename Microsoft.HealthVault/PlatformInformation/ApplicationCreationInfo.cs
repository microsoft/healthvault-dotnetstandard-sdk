// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Represents information to provision a newly created application instance for the SODA flow.
    /// </summary>
    public class ApplicationCreationInfo
    {
        internal static ApplicationCreationInfo Create(XPathNavigator nav)
        {
            ApplicationCreationInfo creationInfo = new ApplicationCreationInfo();

            creationInfo.ParseXml(nav);
            return creationInfo;
        }

        internal void ParseXml(XPathNavigator nav)
        {
            AppInstanceId = Guid.Parse(nav.SelectSingleNode("app-id").Value);
            SharedSecret = nav.SelectSingleNode("shared-secret").Value;
            AppCreationToken = nav.SelectSingleNode("app-token").Value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationCreationInfo"/> class with default values.
        /// </summary>
        public ApplicationCreationInfo()
        {
        }

        // [XmlElement("app-id", Order = 1)]
        public Guid AppInstanceId
        {
            get; set;
        }

        // [XmlElement("shared-secret", Order = 2)]
        public string SharedSecret
        {
            get; set;
        }

        // [XmlElement("app-token", Order = 3)]
        public string AppCreationToken
        {
            get; set;
        }
    }
}
