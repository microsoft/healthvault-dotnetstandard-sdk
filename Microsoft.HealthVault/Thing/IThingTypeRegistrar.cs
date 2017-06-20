// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Allows applications to register custom thing types
    /// </summary>
    public interface IThingTypeRegistrar
    {
        /// <summary>
        /// Registers a class as the handler for the application specific health record thing type with the specific subtype tag.
        /// </summary>
        /// <param name="applicationId">The application identifier for the application specific data that the specified type will handle.</param>
        /// <param name="subtypeTag">The subtype tag for the application specific data that the specified type will handle.</param>
        /// <param name="applicationSpecificHandlerClass">
        /// The .NET type that handles parsing of the application specific data for the specified application ID and subtype tag. 
        /// The applicationSpecificHandler class should extend ApplicationSpecific type
        /// </param>
        void RegisterApplicationSpecificHandler(
            string applicationId,
            string subtypeTag,
            Type applicationSpecificHandlerClass);

        /// <summary>
        /// Registers a handler for thing extension data
        /// </summary>
        /// <param name="extensionSource">The unique identifier for the source of the item extension</param>
        /// <param name="thingExtensionClass">The class that implements the thing extension. It must be public, derive from ThingExtension, and have a default constructor.</param>
        /// <remarks>Allows overwriting the existing deserializer</remarks>
        void RegisterExtensionHandler(string extensionSource, Type thingExtensionClass);

        /// <summary>
        /// Get a collection of all the default thing types known in the system
        /// </summary>
        Dictionary<Guid, Type> RegisteredTypeHandlers { get; }

        /// <summary>
        /// Get a collection of all the default thing types with type as the key
        /// </summary>
        Dictionary<string, Guid> RegisteredTypeHandlersByClassName { get; }

        /// <summary>
        /// Get a collection of all the registered application specific handlers
        /// </summary>
        Dictionary<string, Dictionary<string, Type>> RegisteredAppSpecificHandlers { get; }

        /// <summary>
        /// Get a collection of all the registered extension type handlers
        /// </summary>
        Dictionary<string, Type> RegisteredExtensionHandlers { get; }
    }
}
