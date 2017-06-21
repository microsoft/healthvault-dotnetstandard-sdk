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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Clients.Deserializers
{
    internal class ThingDeserializer : IThingDeserializer
    {
        private readonly Guid _applicationSpecificId = new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0");

        private readonly IHealthVaultConnection _connection;

        private readonly IThingTypeRegistrar _thingTypeRegistrar;

        private readonly Dictionary<string, Dictionary<string, Type>> _appSpecificHandlers;
        private readonly Dictionary<Guid, Type> _typeHandlers;
        private readonly Dictionary<string, Type> _extensionHandlers;

        public ThingDeserializer(IHealthVaultConnection connection, IThingTypeRegistrar thingTypeRegistrar)
        {
            _connection = connection;
            _thingTypeRegistrar = thingTypeRegistrar;

            _appSpecificHandlers = _thingTypeRegistrar.RegisteredAppSpecificHandlers;
            _typeHandlers = _thingTypeRegistrar.RegisteredTypeHandlers;
            _extensionHandlers = _thingTypeRegistrar.RegisteredExtensionHandlers;
        }

        public IReadOnlyCollection<ThingCollection> Deserialize(
            HealthServiceResponseData responseData,
            HealthRecordSearcher searcher)
        {
            XPathNavigator navigator = responseData.InfoNavigator;

            Collection<ThingCollection> result = new Collection<ThingCollection>();

            if (navigator != null)
            {
                XPathNodeIterator groupNodeIterator = navigator.Select("//group");

                foreach (XPathNavigator groupNavigator in groupNodeIterator)
                {
                    ThingCollection resultGroup = CreateResultGroupFromResponse(
                        searcher.Record,
                        groupNavigator,
                        searcher.Filters);

                    if (resultGroup != null)
                    {
                        result.Add(resultGroup);
                    }
                }
            }

            return new ReadOnlyCollection<ThingCollection>(result);
        }

        public ThingBase Deserialize(string thingXml)
        {
            using (XmlReader thingReader = SDKHelper.GetXmlReaderForXml(thingXml, SDKHelper.XmlReaderSettings))
            {
                thingReader.NameTable.Add("wc");
                thingReader.MoveToContent();

                return DeserializeItem(thingReader);
            }
        }

        public ThingExtension DeserializeExtension(XPathNavigator extensionNav)
        {
            ThingExtension result;

            string source = extensionNav.GetAttribute("source", string.Empty);
            if (_extensionHandlers.ContainsKey(source))
            {
                Type handler = _extensionHandlers[source];
                result = (ThingExtension)Activator.CreateInstance(handler);
            }
            else
            {
                result = new ThingExtension(source);
            }

            result.ParseXml(extensionNav);
            return result;
        }

        private ThingCollection CreateResultGroupFromResponse(
            HealthRecordAccessor accessor,
            XPathNavigator groupNavigator,
            Collection<ThingQuery> queryFilters)
        {
            Validator.ThrowIfArgumentNull(accessor, nameof(accessor), Resources.ResponseRecordNull);

            // Name is optional
            ThingQuery matchingQuery = null;
            string groupName = groupNavigator.GetAttribute("name", string.Empty);

            foreach (ThingQuery queryFilter in queryFilters)
            {
                if (string.IsNullOrEmpty(queryFilter.Name) && string.IsNullOrEmpty(groupName))
                {
                    matchingQuery = queryFilter;
                    break;
                }

                if (string.Equals(queryFilter.Name, groupName, StringComparison.Ordinal))
                {
                    matchingQuery = queryFilter;
                    break;
                }
            }

            return GetResultGroupFromResponse(groupName, accessor, matchingQuery, groupNavigator);
        }

        private ThingCollection GetResultGroupFromResponse(
            string groupName,
            HealthRecordAccessor accessor,
            ThingQuery matchingQuery,
            XPathNavigator groupNavigator)
        {
            ThingCollection result =
                new ThingCollection(groupName, accessor, matchingQuery, _connection);

            int maxResultsPerRequest = 0;

            XPathNodeIterator thingNodeIterator = groupNavigator.Select("thing");
            XPathNodeIterator unprocessedThingKeyInfoNodeIterator = groupNavigator.Select("unprocessed-thing-key-info");

            XPathNavigator filteredNodeNavigator = groupNavigator.SelectSingleNode("filtered");
            XPathNavigator orderByCultureNodeNavigator = groupNavigator.SelectSingleNode("order-by-culture");

            if (thingNodeIterator != null)
            {
                foreach (XPathNavigator thingNode in thingNodeIterator)
                {
                    ThingBase resultThingBase = DeserializeItem(thingNode);

                    result.AddResult(resultThingBase);

                    maxResultsPerRequest++;
                }
            }

            if (unprocessedThingKeyInfoNodeIterator != null)
            {
                foreach (XPathNavigator unprocessedThingKeyInfoNode in unprocessedThingKeyInfoNodeIterator)
                {
                    XPathNavigator thingIdNavigator = unprocessedThingKeyInfoNode.SelectSingleNode("thing-id");

                    Guid thingId = Guid.Parse(thingIdNavigator.Value);
                    Guid versionStamp = Guid.Parse(thingIdNavigator.GetAttribute("version-stamp", string.Empty));

                    ThingKey key = new ThingKey(thingId, versionStamp);
                    result.AddResult(key);
                }
            }

            if (filteredNodeNavigator != null)
            {
                result.WasFiltered = filteredNodeNavigator.ValueAsBoolean;
            }

            if (orderByCultureNodeNavigator != null)
            {
                result.OrderByCulture = orderByCultureNodeNavigator.Value;
            }

            if (maxResultsPerRequest > 0)
            {
                result.MaxResultsPerRequest = maxResultsPerRequest;
            }

            return result;
        }

        public ThingBase DeserializeItem(XPathNavigator thingNav)
        {
            ThingBase result;
            Guid typeId = new Guid(thingNav.SelectSingleNode("type-id").Value);

            Type handler = null;
            if (typeId == _applicationSpecificId)
            {
                // Handle application specific health item records by checking for handlers
                // for the application ID and subtype tag. If the handler doesn't exist
                // the default handler will be picked up below.

                AppDataKey appDataKey = GetAppDataKey(thingNav);

                if (appDataKey != null)
                {
                    if (_appSpecificHandlers.ContainsKey(appDataKey.AppId))
                    {
                        if (_appSpecificHandlers[appDataKey.AppId].ContainsKey(appDataKey.SubtypeTag))
                        {
                            handler = _appSpecificHandlers[appDataKey.AppId][appDataKey.SubtypeTag];
                        }
                    }
                }
            }

            if (handler == null && _typeHandlers.ContainsKey(typeId))
            {
                handler = _typeHandlers[typeId];
            }

            if (handler != null)
            {
                result = (ThingBase)Activator.CreateInstance(handler);
            }
            else
            {
                result = new ThingBase(typeId);
            }

            result.ParseXml(thingNav, thingNav.OuterXml);

            return result;
        }

        #region Deserialzer helpers

        private AppDataKey GetAppDataKey(XPathNavigator thingNav)
        {
            AppDataKey result = null;
            XPathNavigator appIdNav =
                thingNav.SelectSingleNode("data-xml/app-specific/format-appid");

            XPathNavigator subtypeNav =
                thingNav.SelectSingleNode("data-xml/app-specific/format-tag");

            if (appIdNav != null && subtypeNav != null)
            {
                result = new AppDataKey
                {
                    AppId = appIdNav.Value,
                    SubtypeTag = subtypeNav.Value
                };
            }

            return result;
        }

        private ThingBase DeserializeItem(XmlReader thingReader)
        {
            string thingString = thingReader.ReadOuterXml();
            XmlReader reader = XmlReader.Create(new StringReader(thingString), SDKHelper.XmlReaderSettings);

            XPathNavigator thingNav = new XPathDocument(reader).CreateNavigator().SelectSingleNode("thing");

            ThingBase thingBase = DeserializeItem(thingNav);

            return thingBase;
        }

        #endregion
    }
}
