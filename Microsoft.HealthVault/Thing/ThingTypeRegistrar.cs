// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// <inheritdoc cref="IThingTypeRegistrar"/>
    /// </summary>
    /// <remarks>This will be a singleton in the application</remarks>
    internal class ThingTypeRegistrar : IThingTypeRegistrar
    {
        private readonly object _appSpecificHandlerLock = new object();
        private readonly object _extensionHandlerLock = new object();

        private readonly Dictionary<string, Dictionary<string, Type>> _registeredAppSpecificHandlers;
        private readonly Dictionary<string, Type> _registeredExtensionHandler;

        public ThingTypeRegistrar()
        {
            // Register all the Thing types that exist in the same namepace as 'weight'
            Assembly itemTypeAssembly = typeof(Weight).GetTypeInfo().Assembly;
            Type derivedType = typeof(ThingBase);
            RegisteredTypeHandlers = itemTypeAssembly
                .DefinedTypes
                .Where(t => t.IsSubclassOf(derivedType) && !t.IsAbstract)
                .ToDictionary(
                    typeInfo => Guid.Parse(typeInfo.GetDeclaredField("TypeId").GetValue(null).ToString()),
                    typeInfo => typeInfo.AsType());

            RegisteredTypeHandlersByClassName = RegisteredTypeHandlers.ToDictionary(x => x.Value.Name, x => x.Key);

            _registeredAppSpecificHandlers = new Dictionary<string, Dictionary<string, Type>>(StringComparer.OrdinalIgnoreCase);
            _registeredExtensionHandler = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<Guid, Type> RegisteredTypeHandlers { get; }

        public Dictionary<string, Guid> RegisteredTypeHandlersByClassName { get; }

        public Dictionary<string, Dictionary<string, Type>> RegisteredAppSpecificHandlers
        {
            get
            {
                lock (_appSpecificHandlerLock)
                {
                    return _registeredAppSpecificHandlers;
                }
            }
        }

        public Dictionary<string, Type> RegisteredExtensionHandlers
        {
            get
            {
                lock (_extensionHandlerLock)
                {
                    return _registeredExtensionHandler;
                }
            }
        }

        public void RegisterExtensionHandler(string extensionSource, Type thingExtensionClass)
        {
            Validator.ThrowIfStringNullOrEmpty(extensionSource, "extensionSource");
            Validator.ThrowIfArgumentNull(thingExtensionClass, nameof(thingExtensionClass), Resources.ItemExtensionClassNull);

            lock (_extensionHandlerLock)
            {
                _registeredExtensionHandler.Add(extensionSource, thingExtensionClass);
            }
        }

        public void RegisterApplicationSpecificHandler(
            string applicationId,
            string subtypeTag,
            Type applicationSpecificHandlerClass)
        {
            Validator.ThrowIfStringNullOrEmpty(applicationId, "applicationId");
            Validator.ThrowIfStringNullOrEmpty(subtypeTag, "subtypeTag");
            Validator.ThrowIfArgumentNull(applicationSpecificHandlerClass, nameof(applicationSpecificHandlerClass), Resources.AppDataHandlerClassMandatory);

            if (applicationSpecificHandlerClass.GetTypeInfo().BaseType.Name != nameof(ApplicationSpecific))
            {
                throw new ArgumentException(Resources.AppDataHandlerNotApplicationSpecific, nameof(applicationSpecificHandlerClass));
            }

            lock (_appSpecificHandlerLock)
            {
                Dictionary<string, Type> handlerDictionary;

                if (_registeredAppSpecificHandlers.ContainsKey(applicationId))
                {
                    handlerDictionary = _registeredAppSpecificHandlers[applicationId];
                }
                else
                {
                    handlerDictionary = new Dictionary<string, Type>();
                    _registeredAppSpecificHandlers.Add(applicationId, handlerDictionary);
                }

                handlerDictionary.Add(subtypeTag, applicationSpecificHandlerClass);
            }
        }
    }
}
