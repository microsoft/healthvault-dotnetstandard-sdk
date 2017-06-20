using System.Collections.Generic;
using System.Xml.XPath;
using Microsoft.HealthVault.Thing;
using Microsoft.HealthVault.Transport;

namespace Microsoft.HealthVault.Clients.Deserializers
{
    /// <summary>
    /// Supports methods to deserialize things
    /// </summary>
    internal interface IThingDeserializer
    {
        /// <summary>
        /// Deserialize HealthServiceResonseData to a collection of things
        /// </summary>
        /// <param name="responseData">Response Data from HealthVault Service</param>
        /// <param name="searcher">HealthRecordSearcher used to retrieve results from HealthVault Service</param>
        /// <returns>Collection of ThingCollection</returns>
        IReadOnlyCollection<ThingCollection> Deserialize(
            HealthServiceResponseData responseData,
            HealthRecordSearcher searcher);

        /// <summary>
        /// Given thing xml, deserializes to a thing
        /// </summary>
        /// <param name="thingXml">xml content in thing element</param>
        /// <returns>ThingBase</returns>
        ThingBase Deserialize(string thingXml);
    }
}