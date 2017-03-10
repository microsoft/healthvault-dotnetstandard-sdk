using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
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
    /// An interface for the HealthVault thing client. Used to access things associated with a particular record.
    /// </summary>
    public class ThingClient : IThingClient
    {
        public IConnectionInternal Connection { get; set; }

        public Guid CorrelationId { get; set; }

        public Guid LastResponseId { get; }

        public HealthRecordInfo Record { get; set; }

        public async Task<T> GetThingAsync<T>(Guid thingId)
            where T : IThing
        {
            // Create a new searcher to get the item.
            HealthRecordSearcher searcher = new HealthRecordSearcher(this.Record);

            ThingQuery query = new ThingQuery();
            query.ItemIds.Add(thingId);
            query.View.Sections = HealthRecordItemSections.Default;
            query.CurrentVersionOnly = true;

            searcher.Filters.Add(query);

            HealthServiceResponseData result = await this.Connection.ExecuteAsync(HealthVaultMethods.GetThings, 3, GetParametersXml(searcher));

            ReadOnlyCollection<HealthRecordItemCollection> resultSet = this.ParseThings(result, searcher);

            // Check in case HealthVault returned invalid data.
            if (resultSet.Count == 0)
            {
                return default(T);
            }

            if (resultSet.Count > 1 || resultSet[0].Count > 1)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = ResourceRetriever.GetResourceString("GetSingleThingTooManyResults")
                };

                HealthServiceException e = HealthServiceExceptionHelper.GetHealthServiceException(
                        HealthServiceStatusCode.MoreThanOneThingReturned,
                        error);
                throw e;
            }

            if (resultSet.Count == 1)
            {
                HealthRecordItemCollection resultGroup = resultSet[0];

                if (resultGroup.Count == 1)
                {
                    return (T)resultGroup[0];
                }
            }

            return default(T);
        }

        public async Task<IReadOnlyCollection<HealthRecordItemCollection>> GetThingsAsync(ThingQuery query)
        {
            HealthServiceResponseData response = await this.GetRequestWithParameters(query);
            HealthRecordSearcher searcher = new HealthRecordSearcher(this.Record);
            ReadOnlyCollection<HealthRecordItemCollection> resultSet =
                this.ParseThings(response, searcher);

            return resultSet;
        }

        public async Task<IReadOnlyCollection<T>> GetThingsAsync<T>(ThingQuery query = null)
            where T : IThing
        {
            // Ensure that we have a query that requests the correct type
            T thing = (T)Activator.CreateInstance(typeof(T));
            query = query ?? new ThingQuery();
            query.TypeIds.Clear();
            query.TypeIds.Add(thing.TypeId);

            IReadOnlyCollection<HealthRecordItemCollection> resultSet = await this.GetThingsAsync(query);

            IList<T> things = new Collection<T>();
            foreach (HealthRecordItemCollection results in resultSet)
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

        public async Task CreateNewThingsAsync(ICollection<IThing> things)
        {
            Validator.ThrowIfArgumentNull(things, "things", "CreateNewThingsAsync");

            StringBuilder infoXml = new StringBuilder();
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (IThing thing in things)
                {
                    Validator.ThrowIfArgumentNull(thing, "thing", "CreateNewThingsAsync");

                    thing.WriteXml(infoXmlWriter);
                }

                infoXmlWriter.Flush();
            }

            HealthServiceResponseData responseData = await this.Connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString());

            // Now update the Id for the new item
            XPathNodeIterator thingIds =
                responseData.InfoNavigator.Select(
                    HealthVaultPlatformItem.GetThingIdXPathExpression(responseData.InfoNavigator));

            int thingIndex = 0;
            foreach (XPathNavigator thingIdNav in thingIds)
            {
                var thing = things.ElementAt(thingIndex) as HealthRecordItem;
                if (thing != null)
                {
                    thing.Key =
                        new HealthRecordItemKey(
                            new Guid(thingIdNav.Value),
                            new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", string.Empty)));
                }

                thingIndex++;
            }
        }

        public async Task UpdateThingsAsync(ICollection<IThing> things)
        {
            Validator.ThrowIfArgumentNull(things, "things", "PutThingsNull");

            StringBuilder infoXml = new StringBuilder(128);
            XmlWriterSettings settings = SDKHelper.XmlUnicodeWriterSettings;

            bool somethingRequiresUpdate = false;

            using (XmlWriter infoXmlWriter =
                XmlWriter.Create(infoXml, settings))
            {
                foreach (IThing thing in things)
                {
                    Validator.ThrowIfArgumentNull(thing, "things", "UpdateItemsArgumentNull");

                    Validator.ThrowArgumentExceptionIf(
                        thing.Key == null,
                        "thingsToUpdate",
                        "UpdateThingWithNoId");

                    if ((thing as HealthRecordItem)?.WriteItemXml(infoXmlWriter, false) == true)
                    {
                        somethingRequiresUpdate = true;
                    }
                }

                infoXmlWriter.Flush();
            }

            if (somethingRequiresUpdate)
            {
                HealthServiceResponseData response = await this.Connection.ExecuteAsync(HealthVaultMethods.PutThings, 2, infoXml.ToString());

                XPathNodeIterator thingIds =
                    response.InfoNavigator.Select(
                        GetThingIdXPathExpression(response.InfoNavigator));

                using (var enumerator = things.GetEnumerator())
                {
                    foreach (XPathNavigator thingIdNav in thingIds)
                    {
                        HealthRecordItem healthRecordItem = enumerator.Current as HealthRecordItem;
                        if (healthRecordItem != null)
                        {
                            healthRecordItem.Key = new HealthRecordItemKey(
                                new Guid(thingIdNav.Value),
                                new Guid(thingIdNav.GetAttribute(
                                    "version-stamp", string.Empty)));
                            healthRecordItem.ClearDirtyFlags();
                        }

                        enumerator.MoveNext();
                    }
                }
            }
        }

        public async Task RemoveThings(ICollection<IThing> things)
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

            await this.Connection.ExecuteAsync(HealthVaultMethods.RemoveThings, 1, parameters.ToString());
        }

        internal static string GetParametersXml(HealthRecordSearcher searcher)
        {
            if (searcher.Filters.Count == 0)
            {
                HealthServiceResponseError error = new HealthServiceResponseError
                {
                    Message = ResourceRetriever.GetResourceString(
                        "HealthRecordSearcherNoFilters")
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

        private ReadOnlyCollection<HealthRecordItemCollection> ParseThings(HealthServiceResponseData responseData, HealthRecordSearcher query)
        {
            XmlReader infoReader = responseData.InfoReader;

            Collection<HealthRecordItemCollection> result =
                new Collection<HealthRecordItemCollection>();

            if ((infoReader != null) && infoReader.ReadToDescendant("group"))
            {
                while (infoReader.Name == "group")
                {
                    using (XmlReader groupReader = infoReader.ReadSubtree())
                    {
                        groupReader.MoveToContent();

                        HealthRecordItemCollection resultGroup =
                            HealthRecordItemCollection.CreateResultGroupFromResponse(
                                this.Record,
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

            return new ReadOnlyCollection<HealthRecordItemCollection>(result);
        }

        private async Task<HealthServiceResponseData> GetRequestWithParameters(ThingQuery query)
        {
            HealthRecordSearcher searcher = new HealthRecordSearcher(this.Record);
            searcher.Filters.Add(query);
            return await this.Connection.ExecuteAsync(HealthVaultMethods.GetThings, 3, GetParametersXml(searcher));
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