// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about a single deployment of HealthVault services and health
    /// record storage.
    /// </summary>
    public class HealthServiceInstance
    {
        internal static HealthServiceInstance CreateInstance(
            XPathNavigator nav)
        {
            HealthServiceInstance instance =
                new HealthServiceInstance();

            instance.ParseXml(nav);
            return instance;
        }

        /// <summary>
        /// Initialize a <see cref="HealthServiceInstance"/> from GetServiceDefinitionAsync response XML.
        /// </summary>
        public void ParseXml(XPathNavigator navigator)
        {
            this.Id = navigator.SelectSingleNode("id").Value;
            this.Name = navigator.SelectSingleNode("name").Value;
            this.Description = navigator.SelectSingleNode("description").Value;
            this.HealthServiceUrl = new Uri(navigator.SelectSingleNode("platform-url").Value);
            this.ShellUrl = new Uri(navigator.SelectSingleNode("shell-url").Value);
        }

        /// <summary>
        /// Write the <see cref="HealthServiceInstance"/> object to an XML writer.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("instance");
            writer.WriteElementString("id", this.Id);
            writer.WriteElementString("name", this.Name);
            writer.WriteElementString("description", this.Description);
            writer.WriteElementString("platform-url", this.HealthServiceUrl.OriginalString);
            writer.WriteElementString("shell-url", this.ShellUrl.OriginalString);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceInstance"/> class with default values.
        /// </summary>
        public HealthServiceInstance()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HealthServiceInstance"/> class with the specified
        /// ID, name, description, HealthVault web-service URL, and Shell URL.
        /// </summary>
        /// <param name="id">Instance ID</param>
        /// <param name="name">Instance name</param>
        /// <param name="description">Description of the instance</param>
        /// <param name="healthServiceUrl">HealthVault web-service URL for the instance</param>
        /// <param name="shellUrl">HealthVault Shell URL for the instance</param>
        public HealthServiceInstance(
            string id,
            string name,
            string description,
            Uri healthServiceUrl,
            Uri shellUrl)
        {
            this.Id = id;
            this.Name = name;
            this.Description = description;
            this.HealthServiceUrl = healthServiceUrl;
            this.ShellUrl = shellUrl;
        }

        /// <summary>
        /// Gets the instance ID.
        /// </summary>
        ///
        /// <value>
        /// A string uniquely identifying the instance.
        /// </value>
        ///
        public string Id { get; private set; }

        /// <summary>
        /// Gets the instance name.
        /// </summary>
        ///
        /// <value>
        /// A friendly name for the instance.
        /// </value>
        ///
        public string Name { get; private set; }

        /// <summary>
        /// Gets a description of the instance.
        /// </summary>
        ///
        /// <value>
        /// A friendly description of the instance.
        /// </value>
        ///
        public string Description { get; private set; }

        /// <summary>
        /// Gets the HealthVault URL.
        /// </summary>
        ///
        /// <value>
        /// A Uri representing a URL to the HealthVault service.
        /// </value>
        ///
        /// <remarks>
        /// This is the URL to the wildcat.ashx which is used to call the
        /// HealthVault XML methods.
        /// </remarks>
        ///
        public Uri HealthServiceUrl { get; private set; }

        /// <summary>
        /// Gets the Shell URL.
        /// </summary>
        ///
        /// <value>
        /// A Uri representing the URL to access the HealthVault Shell.
        /// </value>
        public Uri ShellUrl { get; private set; }
    }
}
