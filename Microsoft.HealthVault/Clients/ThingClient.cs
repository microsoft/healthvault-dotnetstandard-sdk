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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.Record;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Clients
{
    /// <summary>
    /// An interface for the HealthVault thing client
    /// </summary>
    internal class ThingClient : IThingClient
    {
        private readonly IHealthVaultConnection connection;

        public ThingClient(IHealthVaultConnection connection)
        {
            this.connection = connection;
        }

        public Guid CorrelationId { get; set; }

        public async Task<T> GetThingAsync<T>(Guid recordId, Guid thingId)
            where T : IThing
        {
            Validator.ThrowIfArgumentNull(recordId, nameof(recordId), Resources.NewItemsNullItem);

            // Create a new searcher to get the item.
            HealthRecordAccessor accessor = new HealthRecordAccessor(this.connection, recordId);
            HealthRecordSearcher searcher = new HealthRecordSearcher(accessor);

            ThingQuery query = new ThingQuery();
            query.ItemIds.Add(thingId);
            query.View.Sections = ThingSections.Default;
            query.CurrentVersionOnly = true;

            searcher.Filters.Add(query);

            HealthServiceResponseData result = await this.connection.ExecuteAsync(HealthVaultMethods.GetThings, 3, GetParametersXml(searcher));

            ReadOnlyCollection<ThingCollection> resultSet = ParseThings(accessor, result, searcher);

            // Check in case HealthVault returned invalid data.
            if (resultSet.Count == 0)
            {
                return default(T);
            }

            if (resultSet.Count > 1 || resultSet[0].Count > 1)
            {
                throw new MoreThanOneThingException(Resources.GetSingleThingTooManyResults);
            }

            if (resultSet.Count == 1)
            {
                ThingCollection resultGroup = resultSet[0];

                if (resultGroup.Count == 1)
                {
                    return (T)resultGroup[0];
                }
            }

            return default(T);
        }

        public async Task<IReadOnlyCollection<ThingCollection>> GetThingsAsync(Guid recordId, ThingQuery query) 
        {
            Validator.ThrowIfArgumentNull(recordId, nameof(recordId), Resources.NewItemsNullItem);
            Validator.ThrowIfArgumentNull(query, nameof(query), Resources.NewItemsNullItem);

            HealthRecordAccessor accessor = new HealthRecordAccessor(this.connection, recordId);
            HealthRecordSearcher searcher = new HealthRecordSearcher(accessor);
            HealthServiceResponseData response = await this.GetRequestWithParameters(recordId, searcher, query);
            ReadOnlyCollection<ThingCollection> resultSet = 
                ParseThings(accessor, response, searcher);

            return resultSet;
        }

        public async Task<IReadOnlyCollection<T>> GetThingsAsync<T>(Guid recordId, ThingQuery query = null)
            where T : IThing
        {
            Validator.ThrowIfArgumentNull(recordId, nameof(recordId), Resources.NewItemsNullItem);

            // Ensure that we have a query that requests the correct type
            T thing = (T)Activator.CreateInstance(typeof(T));
            query = query ?? new ThingQuery();
            query.TypeIds.Clear();
            query.TypeIds.Add(thing.TypeId);

            IReadOnlyCollection<ThingCollection> resultSet = await this.GetThingsAsync(recordId, query);

            IList<T> things = new Collection<T>();
            foreach (ThingCollection results in resultSet)
            {
                foreach (IThing resultThing in results)
                {
                    if (resultThing is T)
                    {
                        things.Add((T)resultThing);
                    }
                }
            }

            return new ReadOnlyCollection<T>(things);
        }

        public async Task CreateNewThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing
        {
            Validator.ThrowIfArgumentNull(recordId, nameof(recordId), Resources.NewItemsNullItem);
            Validator.ThrowIfArgumentNull(things, nameof(things), Resources.NewItemsNullItem);

            StringBuilder infoXml = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (IThing thing in things)
                {
                    Validator.ThrowIfArgumentNull(thing, nameof(thing), Resources.NewItemsNullItem);

                    thing.WriteItemXml(infoXmlWriter);
                }

                infoXmlWriter.Flush();
            }

            HealthServiceResponseData responseData = await this.connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString(), recordId);

            // Now update the Id for the new item
            XPathNodeIterator thingIds =
                responseData.InfoNavigator.Select(
                    HealthVaultPlatformItem.GetThingIdXPathExpression(responseData.InfoNavigator));

            int thingIndex = 0;
            foreach (XPathNavigator thingIdNav in thingIds)
            {
                var thing = things.ElementAt(thingIndex) as ThingBase;
                if (thing != null)
                {
                    thing.Key =
                        new ThingKey(
                            new Guid(thingIdNav.Value),
                            new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", string.Empty)));
                }

                thingIndex++;
            }
        }

        public async Task UpdateThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing
        {
            Validator.ThrowIfArgumentNull(things, nameof(things), Resources.UpdateItemNull);

            StringBuilder infoXml = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            bool somethingRequiresUpdate = false;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (IThing thing in things)
                {
                    Validator.ThrowIfArgumentNull(thing, nameof(things), Resources.UpdateItemsArgumentNull);

                    if (thing.Key == null)
                    {
                        throw new ArgumentException(Resources.UpdateThingWithNoId, nameof(things));
                    }

                    if ((thing as ThingBase)?.WriteItemXml(infoXmlWriter, false) == true)
                    {
                        somethingRequiresUpdate = true;
                    }
                }

                infoXmlWriter.Flush();
            }

            if (somethingRequiresUpdate)
            {
                HealthServiceResponseData response = await this.connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString(), recordId);

                XPathNodeIterator thingIds =
                    response.InfoNavigator.Select(
                        GetThingIdXPathExpression(response.InfoNavigator));

                using (var enumerator = things.GetEnumerator())
                {
                    foreach (XPathNavigator thingIdNav in thingIds)
                    {
                        ThingBase thingBase = enumerator.Current as ThingBase;
                        if (thingBase != null)
                        {
                            thingBase.Key = new ThingKey(
                                new Guid(thingIdNav.Value),
                                new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", string.Empty)));
                            thingBase.ClearDirtyFlags();
                        }

                        enumerator.MoveNext();
                    }
                }
            }
        }

        public async Task RemoveThingsAsync<T>(Guid recordId, ICollection<T> things)
            where T : IThing
        {
            StringBuilder parameters = new StringBuilder();
            foreach (IThing item in things)
            {
                parameters.Append("<thing-id version-stamp=\"");
                parameters.Append(item.Key.VersionStamp);
                parameters.Append("\">");
                parameters.Append(item.Key.Id);
                parameters.Append("</thing-id>");
            }

            await this.connection.ExecuteAsync(HealthVaultMethods.RemoveThings, 1, parameters.ToString(), recordId);
        }

        internal static string GetParametersXml(HealthRecordSearcher searcher)
        {
            if (searcher.Filters.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = Resources.HealthRecordSearcherNoFilters
                };

                HealthServiceException e =
                    HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.InvalidFilter, error);
                throw e;
            }

            StringBuilder parameters = new StringBuilder(128);

            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter writer = XmlWriter.Create(parameters, settings))
            {
                foreach (ThingQuery filter in searcher.Filters)
                {
                    // Add all filters
                    filter.AddFilterXml(writer);
                }

                writer.Flush();
            }

            return parameters.ToString();
        }

        private static ReadOnlyCollection<ThingCollection> ParseThings(HealthRecordAccessor accessor, HealthServiceResponseData responseData, HealthRecordSearcher query)
        {
            XmlReader infoReader = responseData.InfoReader;

            Collection<ThingCollection> result =
                new Collection<ThingCollection>();

            if ((infoReader != null) && infoReader.ReadToDescendant("group"))
            {
                while (infoReader.Name == "group")
                {
                    using (XmlReader groupReader = infoReader.ReadSubtree())
                    {
                        groupReader.MoveToContent();

                        ThingCollection resultGroup =
                            ThingCollection.CreateResultGroupFromResponse(
                                accessor, 
                                groupReader,
                                query.Filters);

                        // infoReader will normally be at the end element of
                        // the group at this point, and needs a read to get to
                        // the next element. If the group was empty, infoReader
                        // will be at the beginning of the group, and a
                        // single read will still move to the next element.
                        infoReader.Read();
                        if (resultGroup != null)
                        {
                            result.Add(resultGroup);
                        }
                    }
                }
            }

            return new ReadOnlyCollection<ThingCollection>(result);
        }

        private async Task<HealthServiceResponseData> GetRequestWithParameters(Guid recordId, HealthRecordSearcher searcher, ThingQuery query)
        {
            searcher.Filters.Add(query);
            return await this.connection.ExecuteAsync(HealthVaultMethods.GetThings, 3, GetParametersXml(searcher), recordId);
        }

        /// <summary>
        /// Returns the XPathExpression for the ThingId from the supplied XPathNavigator
        /// </summary>
        internal static XPathExpression GetThingIdXPathExpression(XPathNavigator infoNav)
        {
            XmlNamespaceManager infoXmlNamespaceManager =
                new XmlNamespaceManager(infoNav.NameTable);

            infoXmlNamespaceManager.AddNamespace(
                    "wc",
                    "urn:com.microsoft.wc.methods.response.PutThings");

            XPathExpression infoThingIdPathClone = XPathExpression.Compile("/wc:info/thing-id");

            infoThingIdPathClone.SetContext(infoXmlNamespaceManager);

            return infoThingIdPathClone;
        }
    }
}