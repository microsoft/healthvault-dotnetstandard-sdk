// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Diagnostics;
using Microsoft.HealthVault.Exceptions;
using Microsoft.HealthVault.Helpers;
using Microsoft.HealthVault.ItemTypes;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Manages the mapping of a thing type ID to a class
    /// representing the type-specific data for an item and the method used
    /// to deserialize it.
    /// </summary>
    ///
    internal static class ItemTypeManager
    {
        /// <summary>
        /// Registers the default type handlers.
        /// </summary>
        ///
        private static Dictionary<Guid, ThingTypeHandler> RegisterDefaultTypeHandlers(
            out Dictionary<string, ThingTypeHandler> typeHandlersByClassName)
        {
            Dictionary<Guid, ThingTypeHandler> typeHandlers =
                new Dictionary<Guid, ThingTypeHandler>();

            typeHandlersByClassName =
                new Dictionary<string, ThingTypeHandler>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (DefaultTypeHandler typeHandler in s_defaultTypeHandlers)
            {
                ThingTypeHandler handler =
                    new ThingTypeHandler(typeHandler.TypeId, typeHandler.Type);

                typeHandlers.Add(
                    typeHandler.TypeId,
                    handler);

                typeHandlersByClassName.Add(
                    typeHandler.TypeName,
                    handler);
            }

            return typeHandlers;
        }

        private static void RegisterExternalTypeHandlers()
        {
            // Look for the Microsoft.Health.ItemTypes assembly. If loaded, then register
            // those types as well.

            try
            {
                ItemTypeRegistrar.RegisterAssemblyHealthRecordItemTypes();
            }
            catch (Exception)
            {
                // Under no circumstances should this prevent us from proceeding
            }
        }

        private static readonly DefaultTypeHandler[] s_defaultTypeHandlers =
        {
            new DefaultTypeHandler(PasswordProtectedPackage.TypeId, typeof(PasswordProtectedPackage))
        };

        internal class DefaultTypeHandler
        {
            internal DefaultTypeHandler(Guid typeId, Type implementingType)
            {
                TypeId = typeId;
                Type = implementingType;
            }

            public Guid TypeId { get; }

            public Type Type { get; }

            internal string TypeName => Type.Name;
        }

        /// <summary>
        /// Loads the specified HV item-types assembly.  If not found or fails to load, returns null.
        /// </summary>
        private static Assembly GetItemTypesAssembly(string name)
        {
            // otherwise, try loading it by name
            Assembly coreAssemblyName = typeof(ItemTypeManager).GetTypeInfo().Assembly;

            var itemTypesAssemblyName = new AssemblyName
            {
                Name = name,
                Version = coreAssemblyName.GetName().Version,
                CultureName = coreAssemblyName.GetName().CultureName
            };

            itemTypesAssemblyName.SetPublicKeyToken(coreAssemblyName.GetName().GetPublicKeyToken());

            HealthVaultPlatformTrace.Log(
                TraceEventType.Information,
                "Looking for ItemTypes assembly '{0}': {1}",
                name,
                itemTypesAssemblyName.FullName);

            try
            {
                return Assembly.Load(itemTypesAssemblyName);
            }
            catch (FileNotFoundException e)
            {
                // assembly not found
                HealthVaultPlatformTrace.Log(
                    TraceEventType.Information,
                    "ItemTypes assembly '{0}' loading skipped.  It was not found: {1}",
                    name,
                    e.Message);

                return null;
            }
            catch (IOException e)
            {
                // assembly found, but failed to load
                HealthVaultPlatformTrace.Log(
                    TraceEventType.Warning,
                    "ItemTypes assembly '{0}' loading skipped.  It was found, but failed to load: {1}",
                    name,
                    e.Message);

                return null;
            }
            catch (BadImageFormatException e)
            {
                // assembly found, but not valid
                HealthVaultPlatformTrace.Log(
                    TraceEventType.Warning,
                    "ItemTypes assembly '{0}' loading skipped.  It was found, but was not valid: {1}",
                    name,
                    e.Message);

                return null;
            }
        }

        #region RegisterTypeHandler

        /// <summary>
        /// Registers a deserializer for item type-specific data.
        /// </summary>
        ///
        /// <param name="typeId">
        /// The unique identifier for the item type-specific data as defined
        /// by the HealthVault service.
        /// </param>
        ///
        /// <param name="itemTypeClass">
        /// The class that implements the item type-specific data. It must
        /// be public, derive from <see cref="ThingBase"/>, and
        /// have a default constructor.
        /// </param>
        ///
        /// <param name="overwriteExisting">
        /// <b>true</b> to register the new deserializer even if the type
        /// already has a deserializer registered; <b>false</b> to throw an
        /// exception because a deserializer is already registered.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="typeId"/> parameter is <see cref="System.Guid.Empty"/> or
        /// the <paramref name="itemTypeClass"/> parameter does not derive from
        /// <see cref="ThingBase"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="itemTypeClass"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="TypeHandlerAlreadyRegisteredException">
        /// The <paramref name="typeId"/> parameter already has a handler
        /// registered and <paramref name="overwriteExisting"/> is <b>false</b>.
        /// </exception>
        ///
        public static void RegisterTypeHandler(
            Guid typeId,
            Type itemTypeClass,
            bool overwriteExisting = false)
        {
            if (typeId == Guid.Empty)
            {
                throw new ArgumentException(Resources.TypeIdGuidEmpty, nameof(typeId));
            }

            Validator.ThrowIfArgumentNull(itemTypeClass, nameof(itemTypeClass), Resources.ThingTypeClassNull);

            if (!itemTypeClass.GetTypeInfo().IsSubclassOf(typeof(ThingBase)))
            {
                throw new ArgumentException(Resources.TypeClassNotThing, nameof(itemTypeClass));
            }

            if (TypeHandlers.ContainsKey(typeId) && !overwriteExisting)
            {
                throw new TypeHandlerAlreadyRegisteredException(Resources.TypeHandlerAlreadyRegistered);
            }

            TypeHandlers[typeId] = new ThingTypeHandler(typeId, itemTypeClass);
            s_typeHandlersByClassName[itemTypeClass.Name] = TypeHandlers[typeId];
        }

        private static Dictionary<Guid, ThingTypeHandler> TypeHandlers
        {
            get
            {
                if (s_typeHandlers == null)
                {
                    s_typeHandlers = RegisterDefaultTypeHandlers(out s_typeHandlersByClassName);
                    RegisterExternalTypeHandlers();
                }

                return s_typeHandlers;
            }
        }

        private static Dictionary<Guid, ThingTypeHandler> s_typeHandlers;

        internal static Dictionary<string, ThingTypeHandler> TypeHandlersByClassName => s_typeHandlersByClassName;

        private static Dictionary<string, ThingTypeHandler> s_typeHandlersByClassName;

        /// <summary>
        /// Get a collection of all the ThingBase-derived types that are registered.
        /// </summary>
        /// <remarks>
        /// This set of types defines all of the HealthVault item types that this SDK
        /// understands how to process. If new types have been added, it may be a subset of the
        /// types that are available through the HealthVault service.
        ///
        /// To retrieve information about the types from the HealthVault service,
        /// use the <see cref="GetHealthRecordItemTypeDefinitionAsync(IConnectionInternal)"/> method.
        /// </remarks>
        /// <returns>A dictionary of <see cref="Type"/> instances.</returns>
        public static IDictionary<Guid, Type> RegisteredTypes
        {
            get
            {
                Dictionary<Guid, Type> registeredTypes = new Dictionary<Guid, Type>();

                foreach (KeyValuePair<Guid, ThingTypeHandler> item in TypeHandlers)
                {
                    registeredTypes.Add(item.Key, item.Value.ItemTypeClass);
                }

                return registeredTypes;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> instance of the class that is registered to
        /// handle this type id.
        /// </summary>
        /// <remarks>
        /// This method looks up the type id in the list of types that the SDK understands how
        /// to process. If new types were added to the HealthVault service since this SDK was
        /// released, this method will not return them.
        ///
        /// To retrieve information about the types from the HealthVault service,
        /// use the <see cref="GetHealthRecordItemTypeDefinitionAsync(IConnectionInternal)"/> method.
        /// </remarks>
        /// <param name="typeId">The ID of the associated type</param>
        /// <returns>The typeId.</returns>
        public static Type GetRegisteredTypeForTypeId(Guid typeId)
        {
            if (TypeHandlers.ContainsKey(typeId))
            {
                ThingTypeHandler itemTypeHandler = TypeHandlers[typeId];
                return itemTypeHandler.ItemTypeClass;
            }

            return null;
        }

        #endregion RegisterTypeHandler

        #region RegisterExtensionHandler

        /// <summary>
        /// Registers a deserializer for item extension data.
        /// </summary>
        ///
        /// <remarks>
        /// Extension data is available to all applications, and since there is no registration
        /// method for the extensionSource identifiers, collisions between applications are possible.
        ///
        /// Applications should be written to be tolerant of the presence of extension data using the
        /// same extensionSource but a different schema. It is also recommended that extensionSource be
        /// specified the same way .NET namespaces are specified, prefixing the extensionSource with the
        /// company name.
        /// </remarks>
        ///
        /// <param name="extensionSource">
        /// The unique identifier for the source of the item extension.
        /// </param>
        ///
        /// <param name="itemExtensionClass">
        /// The class that implements the item extension. It must
        /// be public, derive from <see cref="ThingExtension"/>,
        /// and have a default constructor.
        /// </param>
        ///
        public static void RegisterExtensionHandler(
            string extensionSource,
            Type itemExtensionClass)
        {
            RegisterExtensionHandler(extensionSource, itemExtensionClass, false);
        }

        /// <summary>
        /// Registers a deserializer for item extension data.
        /// </summary>
        ///
        /// <remarks>
        /// Extension data is available to all applications, and since there is no registration
        /// method for the extensionSource identifiers, collisions between applications are possible.
        ///
        /// Applications should be written to be tolerant of the presence of extension data using the
        /// same extensionSource but a different schema. It is also recommended that extensionSource be
        /// specified the same way .NET namespaces are specified, prefixing the extensionSource with the
        /// company name.
        /// </remarks>
        ///
        /// <param name="extensionSource">
        /// The unique identifier for the source of the item extension.
        /// </param>
        ///
        /// <param name="itemExtensionClass">
        /// The class that implements the item extension. It must
        /// be public, derive from <see cref="ThingExtension"/>,
        /// and have a default constructor.
        /// </param>
        ///
        /// <param name="overwriteExisting">
        /// <b>true</b> to register the new deserializer even if the type
        /// already has a deserializer registered; <b>false</b> to throw an
        /// exception because a deserializer is already registered.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="extensionSource"/> parameter is <b>null</b> or empty or
        /// the <paramref name="itemExtensionClass"/> parameter does not derive from
        /// <see cref="ThingExtension"/>.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="itemExtensionClass"/> parameter is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="TypeHandlerAlreadyRegisteredException">
        /// The <paramref name="extensionSource"/> already has a handler
        /// registered and <paramref name="overwriteExisting"/> is <b>false</b>.
        /// </exception>
        ///
        public static void RegisterExtensionHandler(
            string extensionSource,
            Type itemExtensionClass,
            bool overwriteExisting)
        {
            Validator.ThrowIfStringNullOrEmpty(extensionSource, "extensionSource");
            Validator.ThrowIfArgumentNull(itemExtensionClass, nameof(itemExtensionClass), Resources.ItemExtensionClassNull);

            if (s_extensionHandlers.ContainsKey(extensionSource) &&
                !overwriteExisting)
            {
                throw new TypeHandlerAlreadyRegisteredException(Resources.ExtensionHandlerAlreadyRegistered);
            }

            s_extensionHandlers[extensionSource] =
                new ThingTypeHandler(itemExtensionClass);
        }

        private static Dictionary<string, ThingTypeHandler> s_extensionHandlers =
            new Dictionary<string, ThingTypeHandler>();

        #endregion RegisterExtensionHandler

        #region RegisterApplicationSpecificTypeHandler

        /// <summary>
        /// Registers a class as the handler for the application specific health record
        /// item type with the specific application ID and subtype tag.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application identifier for the application specific data that the
        /// specified type will handle.
        /// </param>
        ///
        /// <param name="subtypeTag">
        /// The subtype tag for the application specific data that the specified type
        /// will handle.
        /// </param>
        ///
        /// <param name="applicationSpecificHandlerClass">
        /// The .NET type that handles parsing of the application specific data for
        /// the specified application ID and subtype tag.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> is <see cref="System.Guid.Empty"/>,
        /// or <paramref name="subtypeTag"/> is
        /// <b>null</b> or empty, or if <paramref name="applicationSpecificHandlerClass"/>
        /// does not derive from ApplicationSpecific.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="applicationSpecificHandlerClass"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="TypeHandlerAlreadyRegisteredException">
        /// If a type is already registered to handle the application specific data
        /// for the specified <paramref name="applicationId"/> and
        /// <paramref name="subtypeTag"/>.
        /// </exception>
        ///
        public static void RegisterApplicationSpecificHandler(
            Guid applicationId,
            string subtypeTag,
            Type applicationSpecificHandlerClass)
        {
            if (applicationId == Guid.Empty)
            {
                throw new ArgumentException(Resources.ApplicationIdGuidEmpty, nameof(applicationId));
            }

            RegisterApplicationSpecificHandler(
                applicationId.ToString(),
                subtypeTag,
                applicationSpecificHandlerClass,
                false);
        }

        /// <summary>
        /// Registers a class as the handler for the application specific health record
        /// item type with the specific application ID and subtype tag.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application identifier for the application specific data that the
        /// specified type will handle.
        /// </param>
        ///
        /// <param name="subtypeTag">
        /// The subtype tag for the application specific data that the specified type
        /// will handle.
        /// </param>
        ///
        /// <param name="applicationSpecificHandlerClass">
        /// The .NET type that handles parsing of the application specific data for
        /// the specified application ID and subtype tag.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> or <paramref name="subtypeTag"/> is
        /// <b>null</b> or empty, or if <paramref name="applicationSpecificHandlerClass"/>
        /// does not derive from ApplicationSpecific.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="applicationSpecificHandlerClass"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="TypeHandlerAlreadyRegisteredException">
        /// If a type is already registered to handle the application specific data
        /// for the specified <paramref name="applicationId"/> and
        /// <paramref name="subtypeTag"/>.
        /// </exception>
        ///
        public static void RegisterApplicationSpecificHandler(
            string applicationId,
            string subtypeTag,
            Type applicationSpecificHandlerClass)
        {
            RegisterApplicationSpecificHandler(
                applicationId,
                subtypeTag,
                applicationSpecificHandlerClass,
                false);
        }

        /// <summary>
        /// Registers a class as the handler for the application specific health record
        /// item type with the specific application ID and subtype tag.
        /// </summary>
        ///
        /// <param name="applicationId">
        /// The application identifier for the application specific data that the
        /// specified type will handle.
        /// </param>
        ///
        /// <param name="subtypeTag">
        /// The subtype tag for the application specific data that the specified type
        /// will handle.
        /// </param>
        ///
        /// <param name="applicationSpecificHandlerClass">
        /// The .NET type that handles parsing of the application specific data for
        /// the specified application ID and subtype tag.
        /// </param>
        ///
        /// <param name="overwriteExisting">
        /// If true and an entry exist for the specified <paramref name="applicationId"/>
        /// and <paramref name="subtypeTag"/> it will be replaced.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="applicationId"/> or <paramref name="subtypeTag"/> is
        /// <b>null</b> or empty, or if <paramref name="applicationSpecificHandlerClass"/>
        /// does not derive from ApplicationSpecific.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="applicationSpecificHandlerClass"/> is <b>null</b>.
        /// </exception>
        ///
        /// <exception cref="TypeHandlerAlreadyRegisteredException">
        /// If a type is already registered to handle the application specific data
        /// for the specified <paramref name="applicationId"/> and
        /// <paramref name="subtypeTag"/> and <paramref name="overwriteExisting"/> is false.
        /// </exception>
        ///
        public static void RegisterApplicationSpecificHandler(
            string applicationId,
            string subtypeTag,
            Type applicationSpecificHandlerClass,
            bool overwriteExisting)
        {
            Validator.ThrowIfStringNullOrEmpty(applicationId, "applicationId");
            Validator.ThrowIfStringNullOrEmpty(subtypeTag, "subtypeTag");
            Validator.ThrowIfArgumentNull(applicationSpecificHandlerClass, nameof(applicationSpecificHandlerClass), Resources.AppDataHandlerClassMandatory);

            if (applicationSpecificHandlerClass.GetTypeInfo().BaseType.Name != nameof(ApplicationSpecific))
            {
                throw new ArgumentException(Resources.AppDataHandlerNotApplicationSpecific, nameof(applicationSpecificHandlerClass));
            }

            Dictionary<string, ThingTypeHandler> handlerDictionary;

            if (s_appSpecificHandlers.ContainsKey(applicationId))
            {
                handlerDictionary = s_appSpecificHandlers[applicationId];

                if (handlerDictionary.ContainsKey(subtypeTag) &&
                    !overwriteExisting)
                {
                    throw new TypeHandlerAlreadyRegisteredException(Resources.AppDataHandlerAlreadyRegistered);
                }
            }
            else
            {
                handlerDictionary = new Dictionary<string, ThingTypeHandler>();
                s_appSpecificHandlers.Add(applicationId, handlerDictionary);
            }

            ThingTypeHandler handler =
                new ThingTypeHandler(applicationSpecificHandlerClass);

            handlerDictionary[subtypeTag] = handler;
        }

        private static Dictionary<string, Dictionary<string, ThingTypeHandler>> s_appSpecificHandlers =
            new Dictionary<string, Dictionary<string, ThingTypeHandler>>();

        #endregion RegisterApplicationSpecificTypeHandler

        #region ThingBase serialization

        /// <summary>
        /// Constructs a <see cref="ThingBase"/> or an appropriate derived type for the
        /// specified item XML.
        /// </summary>
        ///
        /// <param name="itemXml">
        /// The item XML, including the thing tag as the root, to be
        /// deserialized into a <see cref="ThingBase"/>.
        /// </param>
        ///
        /// <returns>
        /// A <see cref="ThingBase"/> or derived type based on the specified item XML.
        /// </returns>
        ///
        public static ThingBase DeserializeItem(string itemXml)
        {
            using (XmlReader thingReader = SDKHelper.GetXmlReaderForXml(itemXml, SDKHelper.XmlReaderSettings))
            {
                thingReader.NameTable.Add("wc");
                thingReader.MoveToContent();

                return DeserializeItem(thingReader);
            }
        }

        private static Guid s_applicationSpecificId = new Guid("a5033c9d-08cf-4204-9bd3-cb412ce39fc0");

        /// <summary>
        /// Deserializes the response XML into a <see cref="ThingBase"/> or derived type
        /// based on the registered thing handler.
        /// </summary>
        ///
        /// <param name="thingReader">
        /// The XML representation of the item.
        /// </param>
        ///
        /// <returns>
        /// The <see cref="ThingBase"/> or derived class instance representing the data
        /// in the XML.
        /// </returns>
        ///
        /// <exception cref="System.Reflection.TargetInvocationException">
        /// The constructor of the type being created throws an
        /// exception. The inner exception is the exception thrown by the
        /// constructor.
        /// </exception>
        ///
        /// <exception cref="MissingMethodException">
        /// The default constructor of the type being created is not public.
        /// If you registered the type handler, be sure that the type you
        /// registered for the item type class has a public default
        /// constructor.
        /// </exception>
        ///
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification =
            "StringReader can be disposed multiple times. Usings block makes the code more readable")]
        internal static ThingBase DeserializeItem(XmlReader thingReader)
        {
            string thingString = thingReader.ReadOuterXml();
            XmlReader reader = XmlReader.Create(new StringReader(thingString), SDKHelper.XmlReaderSettings);

            XPathNavigator thingNav = new XPathDocument(reader).CreateNavigator();

            ThingBase thingBase = DeserializeItem(thingNav);

            return thingBase;
        }

        internal static ThingBase DeserializeItem(XPathNavigator thingNav)
        {
            ThingBase result;
            Guid typeId = GetTypeId(thingNav);

            ThingTypeHandler handler = null;
            if (typeId == s_applicationSpecificId)
            {
                // Handle application specific health item records by checking for handlers
                // for the application ID and subtype tag. If the handler doesn't exist
                // the default handler will be picked up below.

                AppDataKey appDataKey = GetAppDataKey(thingNav);

                if (appDataKey != null)
                {
                    if (s_appSpecificHandlers.ContainsKey(appDataKey.AppId))
                    {
                        if (s_appSpecificHandlers[appDataKey.AppId].ContainsKey(appDataKey.SubtypeTag))
                        {
                            handler =
                                s_appSpecificHandlers[appDataKey.AppId][appDataKey.SubtypeTag];
                        }
                    }
                }
            }

            if (handler == null &&
                TypeHandlers.ContainsKey(typeId))
            {
                handler = TypeHandlers[typeId];
            }

            if (handler != null)
            {
                result = (ThingBase)Activator.CreateInstance(handler.ItemTypeClass);
            }
            else
            {
                result = new ThingBase(typeId);
            }

            result.ParseXml(thingNav, thingNav.OuterXml);

            return result;
        }

        #endregion ThingBase serialization

        #region Extension serialization

        /// <summary>
        /// Deserializes the response XML for extensions into a
        /// <see cref="ThingExtension"/> or derived type based on the registered
        /// extension handler.
        /// </summary>
        ///
        /// <param name="extensionNav">
        /// The XML representation of the extension data.
        /// </param>
        ///
        /// <returns>
        /// The <see cref="ThingExtension"/> or derived class instance
        /// representing the data in the XML.
        /// </returns>
        ///
        /// <exception cref="System.Reflection.TargetInvocationException">
        /// If the constructor of the type being created throws an
        /// exception. The inner exception is the exception thrown by the
        /// constructor.
        /// </exception>
        ///
        /// <exception cref="MissingMethodException">
        /// The default constructor of the type being created is not public.
        /// If you registered the extension handler, be sure that the type you
        /// registered for the extension type class has a public default
        /// constructor.
        /// </exception>
        ///
        internal static ThingExtension DeserializeExtension(
            XPathNavigator extensionNav)
        {
            ThingExtension result;

            string source = extensionNav.GetAttribute("source", string.Empty);
            if (s_extensionHandlers.ContainsKey(source))
            {
                ThingTypeHandler handler = s_extensionHandlers[source];
                result =
                    (ThingExtension)Activator.CreateInstance(
                        handler.ItemTypeClass);
            }
            else
            {
                result = new ThingExtension(source);
            }

            result.ParseXml(extensionNav);
            return result;
        }

        #endregion Extension serialization

        #region ThingBase Type Definitions

        /// <summary>
        /// Gets the definitions for all thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <returns>
        /// The type definitions for all thing type definitions
        /// supported by HealthVault.
        /// </returns>
        ///
        /// <remarks>
        /// This method calls the HealthVault service if the types are not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(
                null,
                ThingTypeSections.All,
                null,
                null,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the definitions for all thing type definitions
        /// supported by HealthVault only if they have been updated since the
        /// specified last client refresh date.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of things whose details are being requested. Null indicates
        /// that all health item records should be returned.
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
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            DateTime? lastClientRefreshDate,
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                ThingTypeSections.All,
                null,
                lastClientRefreshDate,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the requested definitions for the specified thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item record type Ids whose details are being requested. Null
        /// indicates that all health item record types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type of
        /// details to be returned for the specified health item record(s).
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
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                sections,
                null,
                null,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the requested thing type definitions supported by HealthVault
        /// only if they have been updated since the specified last client refresh date.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null
        /// indicates that all health item types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type of
        /// details to be returned for the specified health item record(s).
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
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            DateTime? lastClientRefreshDate,
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                sections,
                null,
                lastClientRefreshDate,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the definitions for all thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type of
        /// details to be returned for the specified health item record(s).
        /// </param>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <returns>
        /// The type definitions for all the thing type definitions
        /// supported by HealthVault.
        /// </returns>
        ///
        /// <remarks>
        /// This method calls the HealthVault service if the types are not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            ThingTypeSections sections,
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(null, sections, null, null, connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the definitions for one or more thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// A collection of health item type IDs whose details are being requested. Null
        /// indicates that all health item types should be returned.
        /// </param>
        ///
        /// <param name="sections">
        /// A collection of ThingTypeSections enumeration values that indicate the type
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
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            ThingTypeSections sections,
            IList<string> imageTypes,
            DateTime? lastClientRefreshDate,
            IConnectionInternal connection)
        {
            return await HealthVaultPlatform.GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                sections,
                imageTypes,
                lastClientRefreshDate,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the definition for a thing type.
        /// </summary>
        ///
        /// <param name="typeId">
        /// The unique identifier for the type to get the definition of.
        /// </param>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <returns>
        /// The type definition for the specified type, or <b>null</b> if the
        /// <paramref name="typeId"/> parameter does not represent a known unique
        /// type identifier.
        /// </returns>
        ///
        /// <remarks>
        /// This method calls the HealthVault service if the type is not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="typeId"/> parameter is Guid.Empty.
        /// </exception>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public static async Task<ThingTypeDefinition> GetHealthRecordItemTypeDefinitionAsync(
            Guid typeId,
            IConnectionInternal connection)
        {
            IDictionary<Guid, ThingTypeDefinition> typeDefs =
                await GetHealthRecordItemTypeDefinitionAsync(new[] { typeId }, connection).ConfigureAwait(false);
            return typeDefs[typeId];
        }

        /// <summary>
        /// Gets the definition of one or more thing type definitions
        /// supported by HealthVault.
        /// </summary>
        ///
        /// <param name="typeIds">
        /// The unique identifiers for the type to get the definition of.
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
        public static async Task<IDictionary<Guid, ThingTypeDefinition>> GetHealthRecordItemTypeDefinitionAsync(
            IList<Guid> typeIds,
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(
                typeIds,
                ThingTypeSections.All,
                connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the thing type definition for the base item type.
        /// </summary>
        ///
        /// <param name="connection">
        /// A connection to the HealthVault service.
        /// </param>
        ///
        /// <remarks>
        /// The base item type is a constructed item type that contains
        /// definitions of the standard item transforms that will work
        /// for any item type. If a specific item type does not define a
        /// standard transformation, the base item type transformation can
        /// be used instead.
        /// <br/><br/>
        /// This method calls the HealthVault service if the type is not
        /// already in the client-side cache.
        /// </remarks>
        ///
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="connection"/> parameter is <b>null</b>.
        /// </exception>
        ///
        public static async Task<ThingTypeDefinition> GetBaseHealthRecordItemTypeDefinitionAsync(
            IConnectionInternal connection)
        {
            return await GetHealthRecordItemTypeDefinitionAsync(s_baseTypeId, connection).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes all item type definitions from the client-side cache.
        /// </summary>
        ///
        public static void ClearItemTypeCache()
        {
            HealthVaultPlatform.ClearItemTypeCache();
        }

        private static readonly Guid s_baseTypeId =
            new Guid("3e730686-781f-4616-aa0d-817bba8eb141");

        #endregion ThingBase Type Definitions

        #region private helpers

        private static Guid GetTypeId(XPathNavigator thingNav)
        {
            string typeIdString = thingNav.SelectSingleNode("type-id").Value;
            return new Guid(typeIdString);
        }

        private static AppDataKey GetAppDataKey(XPathNavigator thingNav)
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

        private class AppDataKey
        {
            public string AppId { get; set; }

            public string SubtypeTag { get; set; }
        }

        #endregion private helpers
    }
}
