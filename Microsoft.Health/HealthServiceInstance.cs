// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
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
        /// Initialize a <see cref="HealthServiceInstance"/> from GetServiceDefinition response XML.
        /// </summary>
        public void ParseXml(XPathNavigator navigator)
        {
            _id = navigator.SelectSingleNode("id").Value;
            _name = navigator.SelectSingleNode("name").Value;
            _description = navigator.SelectSingleNode("description").Value;
            _healthServiceUrl = new Uri(navigator.SelectSingleNode("platform-url").Value);
            _shellUrl = new Uri(navigator.SelectSingleNode("shell-url").Value);
        }

        /// <summary>
        /// Write the <see cref="HealthServiceInstance"/> object to an XML writer.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("instance");
            writer.WriteElementString("id", Id);
            writer.WriteElementString("name", Name);
            writer.WriteElementString("description", Description);
            writer.WriteElementString("platform-url", HealthServiceUrl.OriginalString);
            writer.WriteElementString("shell-url", ShellUrl.OriginalString);
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
            _id = id;
            _name = name;
            _description = description;
            _healthServiceUrl = healthServiceUrl;
            _shellUrl = shellUrl;
        }

        /// <summary>
        /// Gets the instance ID.
        /// </summary>
        /// 
        /// <value>
        /// A string uniquely identifying the instance.
        /// </value>
        /// 
        public string Id
        {
            get { return _id; }
        }

        private string _id;

        /// <summary>
        /// Gets the instance name.
        /// </summary>
        /// 
        /// <value>
        /// A friendly name for the instance.
        /// </value>
        /// 
        public string Name
        {
            get { return _name; }
        }

        private string _name;

        /// <summary>
        /// Gets a description of the instance.
        /// </summary>
        /// 
        /// <value>
        /// A friendly description of the instance.
        /// </value>
        /// 
        public string Description
        {
            get { return _description; }
        }

        private string _description;

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
        public Uri HealthServiceUrl
        {
            get { return _healthServiceUrl; }
        }

        private Uri _healthServiceUrl;

        /// <summary>
        /// Gets the Shell URL.
        /// </summary>
        /// 
        /// <value>
        /// A Uri representing the URL to access the HealthVault Shell.
        /// </value>
        public Uri ShellUrl
        {
            get { return _shellUrl; }
        }

        private Uri _shellUrl;
    }
}
