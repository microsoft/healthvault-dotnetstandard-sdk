// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault.PlatformPrimitives
{

    /// <summary>
    /// Provides low-level access to the HealthVault message operations.
    /// </summary>
    /// <remarks>
    /// <see cref="HealthVaultPlatform"/> uses this class to perform operations. Set 
    /// HealthVaultPlatformInformation.Current to a derived class to intercept all message calls.
    /// </remarks>

    public class HealthVaultPlatformInformation
    {
        /// <summary>
        /// Enables mocking of calls to this class.
        /// </summary>
        /// 
        /// <remarks>
        /// The calling class should pass in a class that derives from this
        /// class and overrides the calls to be mocked. 
        /// </remarks>
        /// 
        /// <param name="mock">The mocking class.</param>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is already a mock registered for this class.
        /// </exception>
        /// 
        public static void EnableMock(HealthVaultPlatformInformation mock)
        {
            Validator.ThrowInvalidIf(_saved != null, "ClassAlreadyMocked");

            _saved = _current;
            _current = mock;
        }

        /// <summary>
        /// Removes mocking of calls to this class.
        /// </summary>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is no mock registered for this class.
        /// </exception>
        /// 
        public static void DisableMock()
        {
            Validator.ThrowInvalidIfNull(_saved, "ClassIsntMocked");

            _current = _saved;
            _saved = null;
        }
        internal static HealthVaultPlatformInformation Current
        {
            get { return _current; }
        }
        private static HealthVaultPlatformInformation _current = new HealthVaultPlatformInformation();
        private static HealthVaultPlatformInformation _saved;

        #region GetServiceDefinition

        /// <summary>
        /// Gets information about the HealthVault service.
        /// </summary>
        /// 
        /// <param name="connection">The connection to use to perform the operation. </param>
        /// 
        /// <remarks>
        /// Gets the latest information about the HealthVault service. This 
        /// includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and 
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        /// 
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception> 
        /// 
        public virtual ServiceInfo GetServiceDefinition(HealthServiceConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");
            return GetServiceDefinition(connection, null);
        }

        /// <summary>
        /// Gets information about the HealthVault service only if it has been updated since
        /// the specified update time.
        /// </summary>
        /// 
        /// <param name="connection">The connection to use to perform the operation.</param>
        /// 
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// This includes:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and 
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// </remarks>
        /// 
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains the service version, SDK
        /// assemblies versions and URLs, method information, and so on.  Otherwise, if there were no updates,
        /// returns <b>null</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception> 
        /// 
        public virtual ServiceInfo GetServiceDefinition(HealthServiceConnection connection, DateTime lastUpdatedTime)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");

            StringBuilder requestBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(requestBuilder, SDKHelper.XmlUnicodeWriterSettings))
            {
                writer.WriteElementString(
                    "updated-date",
                    SDKHelper.XmlFromUtcDateTime(lastUpdatedTime.ToUniversalTime()));

                writer.Flush();
            }

            string requestParams = requestBuilder.ToString();
            return GetServiceDefinition(connection, requestParams);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories.
        /// </summary>
        /// 
        /// <param name="connection">The connection to use to perform the operation.</param>
        /// 
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Gets the latest information about the HealthVault service. Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and 
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// 
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        /// 
        /// <returns>
        /// A <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception> 
        /// 
        public virtual ServiceInfo GetServiceDefinition(
            HealthServiceConnection connection,
            ServiceInfoSections responseSections)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");
            string requestParams = CreateServiceDefinitionRequestParameters(responseSections, null);
            return GetServiceDefinition(connection, requestParams);
        }

        /// <summary>
        /// Gets information about the HealthVault service corresponding to the specified
        /// categories if the requested information has been updated since the specified
        /// update time.
        /// </summary>
        /// 
        /// <param name="connection">The connection to use to perform the operation.</param>
        /// 
        /// <param name="responseSections">
        /// A bitmask of one or more <see cref="ServiceInfoSections"/> which specify the
        /// categories of information to be populated in the <see cref="ServiceInfo"/>.
        /// </param>
        /// 
        /// <param name="lastUpdatedTime">
        /// The time of the last update to an existing cached copy of <see cref="ServiceInfo"/>.
        /// </param>
        /// 
        /// <remarks>
        /// Gets the latest information about the HealthVault service, if there were updates
        /// since the specified <paramref name="lastUpdatedTime"/>.  If there were no updates
        /// the method returns <b>null</b>.
        /// Depending on the specified
        /// <paramref name="responseSections"/>, this will include some or all of:<br/>
        /// - The version of the service.<br/>
        /// - The SDK assembly URLs.<br/>
        /// - The SDK assembly versions.<br/>
        /// - The SDK documentation URL.<br/>
        /// - The URL to the HealthVault Shell.<br/>
        /// - The schema definition for the HealthVault method's request and 
        ///   response.<br/>
        /// - The common schema definitions for types that the HealthVault methods
        ///   use.<br/>
        /// - Information about all available HealthVault instances.<br/>
        /// 
        /// Retrieving only the sections you need will give a faster response time than
        /// downloading the full response.
        /// </remarks>
        /// 
        /// <returns>
        /// If there were updates to the service information since the specified <paramref name="lastUpdatedTime"/>,
        /// a <see cref="ServiceInfo"/> instance that contains some or all of the service version,
        /// SDK  assemblies versions and URLs, method information, and so on, depending on which
        /// information categories were specified.  Otherwise, if there were no updates, returns
        /// <b>null</b>.
        /// </returns>
        /// 
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connection"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception>
        /// 
        /// <exception cref="UriFormatException">
        /// One or more URL strings returned by HealthVault is invalid.
        /// </exception> 
        /// 
        public virtual ServiceInfo GetServiceDefinition(
            HealthServiceConnection connection,
            ServiceInfoSections responseSections,
            DateTime lastUpdatedTime)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "ConnectionNull");
            string requestParams = CreateServiceDefinitionRequestParameters(responseSections, lastUpdatedTime);
            return GetServiceDefinition(connection, requestParams);
        }

        private static string CreateServiceDefinitionRequestParameters(
            ServiceInfoSections responseSections,
            DateTime? lastUpdatedTime)
        {
            StringBuilder requestBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(requestBuilder, SDKHelper.XmlUnicodeWriterSettings))
            {
                if (lastUpdatedTime != null)
                {
                    writer.WriteElementString(
                        "updated-date",
                        SDKHelper.XmlFromUtcDateTime(lastUpdatedTime.Value.ToUniversalTime()));
                }

                writer.WriteStartElement("response-sections");

                if ((responseSections & ServiceInfoSections.Platform) == ServiceInfoSections.Platform)
                {
                    writer.WriteElementString("section", "platform");
                }

                if ((responseSections & ServiceInfoSections.Shell) == ServiceInfoSections.Shell)
                {
                    writer.WriteElementString("section", "shell");
                }

                if ((responseSections & ServiceInfoSections.Topology) == ServiceInfoSections.Topology)
                {
                    writer.WriteElementString("section", "topology");
                }

                if ((responseSections & ServiceInfoSections.XmlOverHttpMethods) == ServiceInfoSections.XmlOverHttpMethods)
                {
                    writer.WriteElementString("section", "xml-over-http-methods");
                }

                if ((responseSections & ServiceInfoSections.MeaningfulUse) == ServiceInfoSections.MeaningfulUse)
                {
                    writer.WriteElementString("section", "meaningful-use");
                }

                writer.WriteEndElement();

                writer.Flush();
            }

            return requestBuilder.ToString();
        }

        private static ServiceInfo GetServiceDefinition(HealthServiceConnection connection, string parameters)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetServiceDefinition", 2);
            request.Parameters = parameters;
            request.Execute();

            if (request.Response.InfoNavigator.HasChildren)
            {
                XPathExpression infoPath = SDKHelper.GetInfoXPathExpressionForMethod(
                    request.Response.InfoNavigator,
                    "GetServiceDefinition2");

                XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);

                return ServiceInfo.CreateServiceInfo(infoNav);
            }
            else
            {
                return null;
            }
        }

        #endregion GetServiceDefinition

        #region GetHealthRecordItemType

        /// <summary> 
        /// Removes all item type definitions from the client-side cache.
        /// </summary>
        /// 
        public virtual void ClearItemTypeCache()
        {
            lock (_sectionCache)
            {
                _sectionCache.Clear();
            }
        }

        private Dictionary<String, Dictionary<HealthRecordItemTypeSections, Dictionary<Guid, HealthRecordItemTypeDefinition>>>
            _sectionCache = new Dictionary<String, Dictionary<HealthRecordItemTypeSections, Dictionary<Guid, HealthRecordItemTypeDefinition>>>();

        /// <summary>
        /// Gets the definitions for one or more health record item type definitions
        /// supported by HealthVault.
        /// </summary>
        /// 
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null 
        /// indicates that all health item types should be returned.
        /// </param>
        /// 
        /// <param name="sections">
        /// A collection of HealthRecordItemTypeSections enumeration values that indicate the type 
        /// of details to be returned for the specified health item records(s).
        /// </param>
        /// 
        /// <param name="imageTypes">
        /// A collection of strings that identify which health item record images should be 
        /// retrieved.
        /// 
        /// This requests an image of the specified mime type should be returned. For example, 
        /// to request a GIF image, "image/gif" should be specified. For icons, "image/vnd.microsoft.icon" 
        /// should be specified. Note, not all health item records will have all image types and 
        /// some may not have any images at all.
        ///                
        /// If '*' is specified, all image types will be returned.
        /// </param>
        /// 
        /// <param name="lastClientRefreshDate">
        /// A <see cref="DateTime"/> instance that specifies the time of the last refresh
        /// made by the client.
        /// </param>
        /// 
        /// <param name="connection"> 
        /// A connection to the HealthVault service.
        /// </param>
        /// 
        /// <returns>
        /// The type definitions for the specified types, or empty if the
        /// <paramref name="typeIds"/> parameter does not represent a known unique
        /// type identifier.
        /// </returns>
        /// 
        /// <remarks>
        /// This method calls the HealthVault service if the types are not
        /// already in the client-side cache.
        /// </remarks>
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="typeIds"/> is <b>null</b> and empty, or 
        /// <paramref name="typeIds"/> is <b>null</b> and member in <paramref name="typeIds"/> is 
        /// <see cref="System.Guid.Empty"/>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        /// 
        public virtual IDictionary<Guid, HealthRecordItemTypeDefinition> GetHealthRecordItemTypeDefinition(
            IList<Guid> typeIds,
            HealthRecordItemTypeSections sections,
            IList<String> imageTypes,
            DateTime? lastClientRefreshDate,
            HealthServiceConnection connection)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "TypeManagerConnectionNull");

            Validator.ThrowArgumentExceptionIf(
                (typeIds != null && typeIds.Contains(Guid.Empty)) ||
                (typeIds != null && typeIds.Count == 0),
                "typeIds",
                "TypeIdEmpty");

            if (lastClientRefreshDate != null)
            {
                return GetHealthRecordItemTypeDefinitionByDate(typeIds,
                                                               sections,
                                                               imageTypes,
                                                               lastClientRefreshDate.Value,
                                                               connection);
            }
            else
            {
                return GetHealthRecordItemTypeDefinitionNoDate(typeIds,
                                                               sections,
                                                               imageTypes,
                                                               connection);
            }
        }

        private IDictionary<Guid, HealthRecordItemTypeDefinition>
            GetHealthRecordItemTypeDefinitionNoDate(
                IList<Guid> typeIds,
                HealthRecordItemTypeSections sections,
                IList<String> imageTypes,
                HealthServiceConnection connection)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetThingType", 1);

            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            Dictionary<Guid, HealthRecordItemTypeDefinition> cachedThingTypes = null;

            string cultureName = connection.Culture.Name;
            Boolean sendRequest = false;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                if ((typeIds != null) && (typeIds.Count > 0))
                {
                    foreach (Guid id in typeIds)
                    {
                        lock (_sectionCache)
                        {
                            if (!_sectionCache.ContainsKey(cultureName))
                            {
                                _sectionCache.Add(
                                    cultureName,
                                    new Dictionary<HealthRecordItemTypeSections, Dictionary<Guid, HealthRecordItemTypeDefinition>>());
                            }
                            if (!_sectionCache[cultureName].ContainsKey(sections))
                            {
                                _sectionCache[cultureName].Add(sections, new Dictionary<Guid, HealthRecordItemTypeDefinition>());
                            }

                            if (_sectionCache[cultureName][sections].ContainsKey(id))
                            {
                                if (cachedThingTypes == null)
                                {
                                    cachedThingTypes =
                                        new Dictionary<Guid, HealthRecordItemTypeDefinition>();
                                }

                                cachedThingTypes[id] = _sectionCache[cultureName][sections][id];
                            }
                            else
                            {
                                sendRequest = true;

                                writer.WriteStartElement("id");

                                writer.WriteString(id.ToString("D"));

                                writer.WriteEndElement();
                            }
                        }
                    }
                }
                else
                {
                    lock (_sectionCache)
                    {
                        if (!_sectionCache.ContainsKey(cultureName))
                        {
                            _sectionCache.Add(
                                cultureName,
                                new Dictionary<HealthRecordItemTypeSections, Dictionary<Guid, HealthRecordItemTypeDefinition>>());
                        }
                        if (!_sectionCache[cultureName].ContainsKey(sections))
                        {
                            _sectionCache[cultureName].Add(sections, new Dictionary<Guid, HealthRecordItemTypeDefinition>());

                        }
                        else
                        {
                            cachedThingTypes = _sectionCache[cultureName][sections];
                        }
                    }

                    sendRequest = true;
                }

                if (!sendRequest)
                {
                    return cachedThingTypes;
                }
                else
                {
                    WriteSectionSpecs(writer, sections);

                    if ((imageTypes != null) && (imageTypes.Count > 0))
                    {
                        foreach (string imageType in imageTypes)
                        {
                            writer.WriteStartElement("image-type");

                            writer.WriteString(imageType);

                            writer.WriteEndElement();
                        }
                    }

                    writer.Flush();
                }
                request.Parameters = requestParameters.ToString();

                request.Execute();

                Dictionary<Guid, HealthRecordItemTypeDefinition> result =
                    CreateThingTypesFromResponse(
                        cultureName,
                        request.Response,
                        sections,
                        cachedThingTypes);

                lock (_sectionCache)
                {
                    foreach (Guid id in result.Keys)
                    {
                        _sectionCache[cultureName][sections][id] = result[id];
                    }
                }

                return result;
            }
        }

        private IDictionary<Guid, HealthRecordItemTypeDefinition>
            GetHealthRecordItemTypeDefinitionByDate(
                IList<Guid> typeIds,
                HealthRecordItemTypeSections sections,
                IList<String> imageTypes,
                DateTime lastClientRefreshDate,
                HealthServiceConnection connection)
        {
            HealthServiceRequest request =
                new HealthServiceRequest(connection, "GetThingType", 1);

            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                if ((typeIds != null) && (typeIds.Count > 0))
                {
                    foreach (Guid id in typeIds)
                    {
                        writer.WriteStartElement("id");

                        writer.WriteString(id.ToString("D"));

                        writer.WriteEndElement();
                    }
                }

                WriteSectionSpecs(writer, sections);

                if ((imageTypes != null) && (imageTypes.Count > 0))
                {
                    foreach (string imageType in imageTypes)
                    {
                        writer.WriteStartElement("image-type");

                        writer.WriteString(imageType);

                        writer.WriteEndElement();
                    }
                }

                writer.WriteElementString("last-client-refresh",
                                          SDKHelper.XmlFromLocalDateTime(lastClientRefreshDate));

                writer.Flush();

                request.Parameters = requestParameters.ToString();

                request.Execute();

                Dictionary<Guid, HealthRecordItemTypeDefinition> result =
                    CreateThingTypesFromResponse(
                        connection.Culture.Name,
                        request.Response,
                        sections,
                        null);

                lock (_sectionCache)
                {
                    _sectionCache[connection.Culture.Name][sections] = result;
                }

                return result;
            }
        }

        private static void WriteSectionSpecs(XmlWriter writer,
                                              HealthRecordItemTypeSections sectionSpecs)
        {
            if ((sectionSpecs & HealthRecordItemTypeSections.Core) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.Core.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.Xsd) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.Xsd.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.Columns) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.Columns.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.Transforms) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.Transforms.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.TransformSource) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.TransformSource.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.Versions) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.Versions.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }

            if ((sectionSpecs & HealthRecordItemTypeSections.EffectiveDateXPath) != HealthRecordItemTypeSections.None)
            {
                writer.WriteStartElement("section");
                writer.WriteString(HealthRecordItemTypeSections.EffectiveDateXPath.ToString().ToLowerInvariant());
                writer.WriteEndElement();
            }
        }

        private Dictionary<Guid, HealthRecordItemTypeDefinition> CreateThingTypesFromResponse(
            string cultureName,
            HealthServiceResponseData response,
            HealthRecordItemTypeSections sections,
            Dictionary<Guid, HealthRecordItemTypeDefinition> cachedThingTypes)
        {
            Dictionary<Guid, HealthRecordItemTypeDefinition> thingTypes = null;

            if (cachedThingTypes != null && cachedThingTypes.Count > 0)
            {
                thingTypes = new Dictionary<Guid, HealthRecordItemTypeDefinition>(cachedThingTypes);
            }
            else
            {
                thingTypes = new Dictionary<Guid, HealthRecordItemTypeDefinition>();
            }

            XPathNodeIterator thingTypesIterator =
                response.InfoNavigator.Select("thing-type");

            lock (_sectionCache)
            {
                if (!_sectionCache.ContainsKey(cultureName))
                {
                    _sectionCache.Add(cultureName, new Dictionary<HealthRecordItemTypeSections, Dictionary<Guid, HealthRecordItemTypeDefinition>>());
                }

                if (!_sectionCache[cultureName].ContainsKey(sections))
                {
                    _sectionCache[cultureName].Add(sections, new Dictionary<Guid, HealthRecordItemTypeDefinition>());
                }

                foreach (XPathNavigator navigator in thingTypesIterator)
                {
                    HealthRecordItemTypeDefinition thingType =
                        HealthRecordItemTypeDefinition.CreateFromXml(navigator);

                    _sectionCache[cultureName][sections][thingType.TypeId] = thingType;
                    thingTypes[thingType.TypeId] = thingType;
                }
            }

            return thingTypes;
        }
        #endregion GetHealthRecordItemType

        #region SelectInstance

        /// <summary>
        /// Gets the instance where a HealthVault account should be created
        /// for the specified account location.
        /// </summary>
        /// 
        /// <param name="preferredLocation">
        /// A user's preferred geographical location, used to select the best instance
        /// in which to create a new HealthVault account. If there is a location associated
        /// with the credential that will be used to log into the account, that location
        /// should be used.
        /// </param>
        /// 
        /// <param name="connection">
        /// The connection to use to perform the operation.
        /// </param>
        /// 
        /// <remarks>
        /// If no suitable instance can be found, a null value is returned. This can happen,
        /// for example, if the account location is not supported by HealthVault.
        /// 
        /// Currently the returned instance IDs all parse to integers, but that is not
        /// guaranteed and should not be relied upon.
        /// </remarks>
        /// 
        /// <returns>
        /// A <see cref="HealthServiceInstance"/> object represents the selected instance,
        /// or null if no suitable instance exists.
        /// </returns>
        /// 
        /// <exception cref="HealthServiceException">
        /// The HealthVault service returned an error.
        /// </exception> 
        /// 
        /// <exception cref="ArgumentException">
        /// If <paramref name="preferredLocation"/> is <b>null</b>.
        /// </exception>
        /// 
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        public virtual HealthServiceInstance SelectInstance(
            HealthServiceConnection connection,
            Location preferredLocation)
        {
            Validator.ThrowIfArgumentNull(connection, "connection", "TypeManagerConnectionNull");
            Validator.ThrowIfArgumentNull(preferredLocation, "location", "SelectInstanceLocationRequired");

            StringBuilder requestParameters = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(requestParameters, settings))
            {
                preferredLocation.WriteXml(writer, "preferred-location");
                writer.Flush();
            }

            HealthServiceRequest request =
                new HealthServiceRequest(connection, "SelectInstance", 1);

            request.Parameters = requestParameters.ToString();
            request.Execute();

            XPathExpression infoPath = SDKHelper.GetInfoXPathExpressionForMethod(
                request.Response.InfoNavigator,
                "SelectInstance");
            XPathNavigator infoNav = request.Response.InfoNavigator.SelectSingleNode(infoPath);

            XPathNavigator instanceNav = infoNav.SelectSingleNode("selected-instance");

            if (instanceNav != null)
            {
                return HealthServiceInstance.CreateInstance(instanceNav);
            }

            return null;
        }
        #endregion
    }
}

