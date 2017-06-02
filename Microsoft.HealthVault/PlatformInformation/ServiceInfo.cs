// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Helpers;
using NodaTime;

namespace Microsoft.HealthVault.PlatformInformation
{
    /// <summary>
    /// Provides information about the HealthVault service to which you are
    /// connected.
    /// </summary>
    ///
    public class ServiceInfo
    {
        /// <summary>
        /// Creates a ServiceInfo instance out of the service definition XML.
        /// </summary>
        ///
        /// <param name="serviceInfoXml">
        /// The XML representation of the ServiceInfo object.
        /// </param>
        ///
        /// <returns>
        /// A ServiceInfo instance based on the specified service definition XML.
        /// </returns>
        ///
        public static ServiceInfo Create(XPathNavigator serviceInfoXml)
        {
            if (serviceInfoXml == null)
            {
                throw new ArgumentNullException(nameof(serviceInfoXml));
            }

            return CreateServiceInfo(serviceInfoXml.SelectSingleNode("service-info"));
        }

        internal static ServiceInfo CreateServiceInfo(XPathNavigator nav)
        {
            Uri platformUrl = null;
            string platformVersion = null;
            Dictionary<string, string> configValues;
            XPathNavigator platformNav = nav.SelectSingleNode("platform");

            if (platformNav != null)
            {
                platformUrl = new Uri(platformNav.SelectSingleNode("url").Value);
                platformVersion = platformNav.SelectSingleNode("version").Value;
                configValues = GetConfigurationValues(platformNav.Select("configuration"));
            }
            else
            {
                configValues = new Dictionary<string, string>();
            }

            HealthServiceShellInfo shellInfo = null;
            XPathNavigator shellNav = nav.SelectSingleNode("shell");

            if (shellNav != null)
            {
                shellInfo = HealthServiceShellInfo.CreateShellInfo(
                    nav.SelectSingleNode("shell"));
            }

            Collection<HealthServiceMethodInfo> methods = GetMethods(nav);
            Collection<Uri> includes = GetIncludes(nav);

            string currentInstanceId;
            Dictionary<string, HealthServiceInstance> instances =
                GetServiceInstances(nav, out currentInstanceId);

            Instant lastUpdated = SDKHelper.InstantFromUnmarkedXml(nav.SelectSingleNode("updated-date").Value);

            ServiceInfo serviceInfo =
                new ServiceInfo(
                    platformUrl,
                    platformVersion,
                    shellInfo,
                    methods,
                    includes,
                    configValues,
                    instances,
                    currentInstanceId,
                    lastUpdated);

            return serviceInfo;
        }

        /// <summary>
        /// Gets an XML representation of the ServiceInfo object.
        /// </summary>
        ///
        /// <returns>
        /// An XML string representing the ServiceInfo object.
        /// </returns>
        ///
        public string GetXml()
        {
            StringBuilder result = new StringBuilder();

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(result, settings))
            {
                WriteXml("service-info", writer);
                writer.Flush();
            }

            return result.ToString();
        }

        private void WriteXml(string elementName, XmlWriter writer)
        {
            writer.WriteStartElement(elementName);
            {
                WritePlatformInfo(writer);
                WriteShellInfo(writer);
                WriteMethods(writer);
                WriteIncludes(writer);
                WriteServiceInstances(writer);
                WriteUpdatedDate(writer);
                writer.WriteEndElement();
            }
        }

        private void WritePlatformInfo(XmlWriter writer)
        {
            if (HealthServiceUrl == null || string.IsNullOrEmpty(Version))
            {
                return;
            }

            writer.WriteStartElement("platform");
            {
                writer.WriteElementString("url", HealthServiceUrl.OriginalString);
                writer.WriteElementString("version", Version);

                WriteConfigs(writer, ConfigurationValues);

                writer.WriteEndElement();
            }
        }

        private void WriteIncludes(XmlWriter writer)
        {
            if (IncludedSchemaUrls == null)
            {
                return;
            }

            foreach (Uri include in IncludedSchemaUrls)
            {
                writer.WriteElementString("common-schema", include.OriginalString);
            }
        }

        private void WriteMethods(XmlWriter writer)
        {
            if (Methods == null)
            {
                return;
            }

            foreach (HealthServiceMethodInfo method in Methods)
            {
                writer.WriteStartElement("xml-method");
                {
                    writer.WriteElementString("name", method.Name);

                    foreach (HealthServiceMethodVersionInfo version in method.Versions)
                    {
                        writer.WriteStartElement("version");
                        {
                            writer.WriteAttributeString("number", version.Version.ToString(CultureInfo.InvariantCulture));

                            if (version.RequestSchemaUrl != null)
                            {
                                writer.WriteElementString("request-schema-url", version.RequestSchemaUrl.OriginalString);
                            }

                            if (version.ResponseSchemaUrl != null)
                            {
                                writer.WriteElementString("response-schema-url", version.ResponseSchemaUrl.OriginalString);
                            }

                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }
            }
        }

        private void WriteShellInfo(XmlWriter writer)
        {
            if (HealthServiceShellInfo == null)
            {
                return;
            }

            writer.WriteStartElement("shell");
            {
                writer.WriteElementString("url", HealthServiceShellInfo.BaseUrl.OriginalString);
                writer.WriteElementString("redirect-url", HealthServiceShellInfo.RedirectUrl.OriginalString);

                foreach (HealthServiceShellRedirectToken token in HealthServiceShellInfo.RedirectTokens)
                {
                    writer.WriteStartElement("redirect-token");
                    {
                        writer.WriteElementString("token", token.Token);
                        writer.WriteElementString("description", token.Description);

                        string queryStringParameters =
                            string.Join(
                                ",",
                                token.QueryStringParameters);
                        writer.WriteElementString("querystring-parameters", queryStringParameters);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        private static void WriteConfigs(XmlWriter writer, Dictionary<string, string> configs)
        {
            foreach (KeyValuePair<string, string> config in configs)
            {
                writer.WriteStartElement("configuration");
                {
                    writer.WriteAttributeString("key", config.Key);
                    writer.WriteValue(config.Value);

                    writer.WriteEndElement();
                }
            }
        }

        private void WriteServiceInstances(XmlWriter writer)
        {
            if (ServiceInstances == null || CurrentInstance == null)
            {
                return;
            }

            writer.WriteStartElement("instances");
            {
                writer.WriteAttributeString("current-instance-id", CurrentInstance.Id);

                foreach (HealthServiceInstance instance in ServiceInstances.Values)
                {
                    instance.WriteXml(writer);
                }

                writer.WriteEndElement();
            }
        }

        private void WriteUpdatedDate(XmlWriter writer)
        {
            writer.WriteElementString(
                "updated-date",
                SDKHelper.XmlFromInstant(LastUpdated));
        }

        private static Dictionary<string, string> GetConfigurationValues(
            XPathNodeIterator configIterator)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (XPathNavigator configNav in configIterator)
            {
                result.Add(configNav.GetAttribute("key", string.Empty), configNav.Value);
            }

            return result;
        }

        private static Collection<HealthServiceMethodInfo> GetMethods(
            XPathNavigator nav)
        {
            XPathNodeIterator methodNavs = nav.Select("xml-method");
            Collection<HealthServiceMethodInfo> methods =
                new Collection<HealthServiceMethodInfo>();

            foreach (XPathNavigator methodNav in methodNavs)
            {
                methods.Add(HealthServiceMethodInfo.CreateMethodInfo(methodNav));
            }

            return methods;
        }

        private static Collection<Uri> GetIncludes(
            XPathNavigator nav)
        {
            Collection<Uri> includes = new Collection<Uri>();
            XPathNodeIterator includeNavs = nav.Select("common-schema");

            foreach (XPathNavigator includeNav in includeNavs)
            {
                includes.Add(new Uri(includeNav.Value));
            }

            return includes;
        }

        private static Dictionary<string, HealthServiceInstance> GetServiceInstances(
            XPathNavigator nav,
            out string currentInstanceId)
        {
            Dictionary<string, HealthServiceInstance> instances =
                new Dictionary<string, HealthServiceInstance>(StringComparer.OrdinalIgnoreCase);

            XPathNavigator instancesNav = nav.SelectSingleNode("instances");

            if (instancesNav != null)
            {
                currentInstanceId = instancesNav.GetAttribute("current-instance-id", string.Empty);

                XPathNodeIterator instanceNavs = instancesNav.Select("instance");

                foreach (XPathNavigator instanceNav in instanceNavs)
                {
                    HealthServiceInstance instance = HealthServiceInstance.CreateInstance(instanceNav);
                    instances.Add(instance.Id, instance);
                }

                return instances;
            }

            currentInstanceId = null;
            return null;
        }

        private ServiceInfo(
            Uri healthServiceUrl,
            string healthVaultVersion,
            HealthServiceShellInfo shellInfo,
            IList<HealthServiceMethodInfo> methods,
            IList<Uri> includes,
            Dictionary<string, string> configurationValues,
            Dictionary<string, HealthServiceInstance> instances,
            string currentInstanceId,
            Instant lastUpdated)
        {
            HealthServiceUrl = healthServiceUrl;
            Version = healthVaultVersion;
            HealthServiceShellInfo = shellInfo;

            Methods =
                new ReadOnlyCollection<HealthServiceMethodInfo>(methods);
            IncludedSchemaUrls =
                new ReadOnlyCollection<Uri>(includes);

            if (configurationValues != null)
            {
                ConfigurationValues = configurationValues;
            }

            if (instances != null)
            {
                ServiceInstances = instances;

                CurrentInstance = ServiceInstances[currentInstanceId];
            }

            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Create a new instance of the <see cref="ServiceInfo"/> class for testing purposes.
        /// </summary>
        public ServiceInfo()
        {
        }

        /// <summary>
        /// Gets or sets the HealthVault URL.
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
        public Uri HealthServiceUrl { get; protected set; }

        /// <summary>
        /// Gets or sets the version of the HealthVault service.
        /// </summary>
        ///
        /// <value>
        /// A string indicating the version of the HealthVault Service.
        /// </value>
        ///
        /// <remarks>
        /// This value is generally in the format of a
        /// <see cref="System.Version"/>, but can be changed by the
        /// HealthVault service provider.
        /// </remarks>
        ///
        public string Version { get; protected set; }

        /// <summary>
        /// Gets or sets the latest information about the HealthVault Shell.
        /// </summary>
        ///
        public HealthServiceShellInfo HealthServiceShellInfo { get; protected set; }

        /// <summary>
        /// Gets or sets information about the methods that the HealthVault service
        /// exposes.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection of the HealthVault method definitions.
        /// </value>
        ///
        /// <remarks>
        /// A HealthVault method is a named service point provided by the HealthVault
        /// service that answers HTTP requests that contain XML adhering to
        /// the HealthVault request schema. The elements of this collection
        /// define the method name, and request and response schemas for the
        /// method.
        /// </remarks>
        ///
        public ReadOnlyCollection<HealthServiceMethodInfo> Methods { get; protected set; }

        /// <summary>
        /// Gets or sets the URLs of the common schemas that are included in the
        /// method XSDs.
        /// </summary>
        ///
        /// <value>
        /// A read-only collection containing the URLs of the schemas that
        /// are included in the <see cref="Methods"/> request and response
        /// schemas.
        /// </value>
        ///
        /// <remarks>
        /// Many of the <see cref="Methods"/> contain types that are common
        /// across different method requests and responses. These types are
        /// defined in the included schema URLs so that they can be referenced
        /// by each of the methods as needed.
        /// </remarks>
        ///
        public ReadOnlyCollection<Uri> IncludedSchemaUrls { get; protected set; }

        /// <summary>
        /// Gets or sets the public configuration values for the HealthVault service.
        /// </summary>
        ///
        /// <value>
        /// The dictionary returned uses the configuration value name as the key. All entries are
        /// public configuration values that the HealthVault service exposes as information to
        /// HealthVault applications. Values can be used to throttle thing queries, etc.
        /// </value>
        ///
        public Dictionary<string, string> ConfigurationValues { get; protected set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the set of available HealthVault instances.
        /// </summary>
        ///
        /// <remarks>
        /// In order to work seamlessly across the globe, HealthVault
        /// is deployed in multiple data centers around the world. Each
        /// of these deployments contains a complete instance of all
        /// HealthVault services, including both the HealthVault Shell
        /// and platform, and stores health record data primarily for
        /// users from a particular region or constituent population.
        /// Although an instance can never access personal health data
        /// stored in another instance, it knows that the other instances
        /// exist. When an application or end user needs a user's health
        /// record data, they can make a call to any instance to learn
        /// of the instance in which that user's data is stored.
        /// </remarks>
        ///
        public Dictionary<string, HealthServiceInstance> ServiceInstances { get; } = new Dictionary<string, HealthServiceInstance>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a reference to the information for the HealthVault instance
        /// that was used to get this ServiceInfo instance.
        /// </summary>
        public HealthServiceInstance CurrentInstance { get; private set; }

        /// <summary>
        /// Gets the timestamp of when the service definition was last modified on Platform.
        /// </summary>
        ///
        /// <remarks>
        /// Because a call to Platform may be handled by any of several servers,
        /// all of which refresh at slightly different times, this timestamp will vary
        /// among several values across requests.
        /// </remarks>
        public Instant LastUpdated { get; private set; }
    }
}
