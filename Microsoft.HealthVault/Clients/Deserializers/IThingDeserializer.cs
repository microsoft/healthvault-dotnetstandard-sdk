// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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