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
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Clients.Deserializers
{
    internal class ThingDeserializer : IThingDeserializer
    {
        private readonly IHealthVaultConnection connection;

        public ThingDeserializer(IHealthVaultConnection connection)
        {
            this.connection = connection;
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
                    ThingCollection resultGroup = this.CreateResultGroupFromResponse(
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

            return this.GetResultGroupFromResponse(groupName, accessor, matchingQuery, groupNavigator);
        }

        private ThingCollection GetResultGroupFromResponse(
            string groupName,
            HealthRecordAccessor accessor,
            ThingQuery matchingQuery,
            XPathNavigator groupNavigator)
        {
            ThingCollection result =
                new ThingCollection(groupName, accessor, matchingQuery, this.connection);

            int maxResultsPerRequest = 0;

            XPathNodeIterator thingNodeIterator = groupNavigator.Select("thing");
            XPathNodeIterator unprocessedThingKeyInfoNodeIterator = groupNavigator.Select("unprocessed-thing-key-info");

            XPathNavigator filteredNodeNavigator = groupNavigator.SelectSingleNode("filtered");
            XPathNavigator orderByCultureNodeNavigator = groupNavigator.SelectSingleNode("order-by-culture");

            if (thingNodeIterator != null)
            {
                foreach (XPathNavigator thingNode in thingNodeIterator)
                {
                    ThingBase resultThingBase = ItemTypeManager.DeserializeItem(thingNode);

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
    }
}
