// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Health
{
    /// <summary>
    /// Provides information about the HealthVault service to which you are 
    /// connected.
    /// </summary>
    /// 
    public class ServiceInfo
    {
        private static IServiceInfoProvider _defaultServiceInfoProvider = new CachedServiceInfoProvider();

        /// <summary>
        /// Gets the HealthVault service information.
        /// </summary>
        /// 
        /// <remarks>
        /// <p>
        /// By default, retrieval of <see cref="ServiceInfo"/> through this singleton property is thread-safe. It is obtained from the
        /// configured default HealthVault web-service instance (<see cref="HealthApplicationConfiguration.HealthVaultUrl"/>) on the
        /// first get, and cached for a configured period of time (<see cref="HealthApplicationConfiguration.ServiceInfoDefaultCacheTtl"/>).
        /// The next get after this cache has expired will result in calling the HealthVault web-service to check for updates to the service
        /// information, and retrieving the updated service information when there is an update.
        /// </p>
        /// 
        /// <p>
        /// If you want to control the retrieval behavior of <see cref="ServiceInfo"/> objects through this singleton,
        /// use the <see cref="SetSingletonProvider(IServiceInfoProvider)"/> method to set your own implementation of the provider.
        /// </p>
        /// </remarks>
        /// 
        public static ServiceInfo Current
        {
            get
            {
                return _defaultServiceInfoProvider.GetServiceInfo();
            }
        }

        /// <summary>
        /// Sets the provider to be used for the <see cref="P:CurrentInfo"/> singleton.
        /// </summary>
        /// 
        /// <param name="defaultProvider">
        /// The service info provider to be used for the <see cref="P:CurrentInfo"/> singleton.
        /// </param>
        /// 
        public static void SetSingletonProvider(IServiceInfoProvider defaultProvider)
        {
            _defaultServiceInfoProvider = defaultProvider;
        }

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
                throw new ArgumentNullException("serviceInfoXml");
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

            HealthServiceMeaningfulUseInfo meaningfulUseInfo = null;
            XPathNavigator meaningfulUseNav = nav.SelectSingleNode("meaningful-use");

            if (meaningfulUseNav != null)
            {
                meaningfulUseInfo = HealthServiceMeaningfulUseInfo.CreateMeaningfulUseInfo(
                    meaningfulUseNav);
            }

            // updated-date is in UTC, but we have to say so explicitly or it's treated as local.
            DateTime lastUpdated = nav.SelectSingleNode("updated-date").ValueAsDateTime;
            lastUpdated = new DateTime(lastUpdated.Ticks, DateTimeKind.Utc);

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
                    meaningfulUseInfo,
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
                WriteMeaningfulUseInfo(writer);

                writer.WriteEndElement();
            }
        }

        private void WritePlatformInfo(XmlWriter writer)
        {
            if (HealthServiceUrl == null || String.IsNullOrEmpty(Version))
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
                            String.Join(
                                ",",
                                token.QueryStringParameters);
                        writer.WriteElementString("querystring-parameters", queryStringParameters);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        private static void WriteConfigs(XmlWriter writer, Dictionary<string,string> configs)
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

        private void WriteMeaningfulUseInfo(XmlWriter writer)
        {
            if (MeaningfulUseInfo == null)
            {
                return;
            }

            writer.WriteStartElement("meaningful-use");
            {
                writer.WriteStartElement("enabled");
                {
                    writer.WriteValue(MeaningfulUseInfo.Enabled);
                    writer.WriteEndElement();
                }

                WriteConfigs(writer, MeaningfulUseInfo.ConfigurationValues);

                writer.WriteEndElement();
            }
        }

        private void WriteUpdatedDate(XmlWriter writer)
        {
            writer.WriteElementString(
                "updated-date",
                SDKHelper.XmlFromDateTime(LastUpdated.ToUniversalTime()));
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
                currentInstanceId = instancesNav.GetAttribute("current-instance-id", String.Empty);

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
            HealthServiceMeaningfulUseInfo meaningfulUseInfo,
            DateTime lastUpdated)
        {
            _healthServiceUrl = healthServiceUrl;
            _healthVaultVersion = healthVaultVersion;
            _shellInfo = shellInfo;
            _meaningfulUseInfo = meaningfulUseInfo;

            _methods =
                new ReadOnlyCollection<HealthServiceMethodInfo>(methods);
            _includes =
                new ReadOnlyCollection<Uri>(includes);

            if (configurationValues != null)
            {
                _configurationValues = configurationValues;
            }

            if (instances != null)
            {
                _instances = instances;

                CurrentInstance = _instances[currentInstanceId];
            }

            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Create a new instance of the <see cref="ServiceInfo"/> class for testing purposes.
        /// </summary>
        protected ServiceInfo()
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
        public Uri HealthServiceUrl
        {
            get { return _healthServiceUrl; }
            protected set { _healthServiceUrl = value; }
        }
        private Uri _healthServiceUrl;

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
        public string Version
        {
            get { return _healthVaultVersion; }
            protected set { _healthVaultVersion = value; }
        }
        private string _healthVaultVersion;

        /// <summary>
        /// Gets or sets the latest information about the HealthVault Shell.
        /// </summary>
        /// 
        public HealthServiceShellInfo HealthServiceShellInfo
        {
            get { return _shellInfo; }
            protected set { _shellInfo = value; }
        }
        private HealthServiceShellInfo _shellInfo;

        /// <summary>
        /// Gets the latest information about the assemblies that represent 
        /// the HealthVault SDK.
        /// </summary>
        /// 
        /// <value>
        /// A read-only collection of information about the .NET assemblies
        /// that can be used as helpers for accessing the HealthVault service.
        /// </value>
        /// 
        /// <remarks>
        /// This property is no longer supported and will always return an empty 
        /// collection.
        /// </remarks>
        /// 
        [Obsolete("No longer supported - remove references to this property.")]
        public ReadOnlyCollection<HealthServiceAssemblyInfo> Assemblies
        {
            get
            {
                return
                    new ReadOnlyCollection<HealthServiceAssemblyInfo>(
                        new HealthServiceAssemblyInfo[] { });
            }
        }

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
        public ReadOnlyCollection<HealthServiceMethodInfo> Methods
        {
            get { return _methods; }
            protected set { _methods = value; }
        }
        private ReadOnlyCollection<HealthServiceMethodInfo> _methods;

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
        public ReadOnlyCollection<Uri> IncludedSchemaUrls
        {
            get { return _includes; }
            protected set { _includes = value; }
        }
        private ReadOnlyCollection<Uri> _includes;

        /// <summary>
        /// Gets or sets the public configuration values for the HealthVault service.
        /// </summary>
        /// 
        /// <value>
        /// The dictionary returned uses the configuration value name as the key. All entries are
        /// public configuration values that the HealthVault service exposes as information to 
        /// HealthVault applications. Values can be used to throttle health record item queries, etc.
        /// </value>
        /// 
        public Dictionary<string, string> ConfigurationValues
        {
            get { return _configurationValues; }
            protected set { _configurationValues = value; }
        }
        private Dictionary<string, string> _configurationValues = new Dictionary<string, string>();

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
        public Dictionary<string, HealthServiceInstance> ServiceInstances
        {
            get { return _instances; }
        }
        private Dictionary<string, HealthServiceInstance> _instances =
            new Dictionary<string, HealthServiceInstance>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a reference to the information for the HealthVault instance
        /// that was used to get this ServiceInfo instance.
        /// </summary>
        public HealthServiceInstance CurrentInstance { get; private set; }

        /// <summary>
        /// Gets configuration information for Meaningful Use features.
        /// </summary>
        public HealthServiceMeaningfulUseInfo MeaningfulUseInfo
        {
            get { return _meaningfulUseInfo; }
            protected set { _meaningfulUseInfo = value; }
        }
        private HealthServiceMeaningfulUseInfo _meaningfulUseInfo;

        /// <summary>
        /// Gets the timestamp of when the service definition was last modified on Platform.
        /// </summary>
        /// 
        /// <remarks>
        /// Because a call to Platform may be handled by any of several servers,
        /// all of which refresh at slightly different times, this timestamp will vary
        /// among several values across requests.
        /// </remarks>
        public DateTime LastUpdated { get; private set; }
    }
}
