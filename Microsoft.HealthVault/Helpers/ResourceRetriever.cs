// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.HealthVault.Helpers
{
    /// <summary>
    /// Helps manage ResourceManager instances.
    /// </summary>
    ///
    /// <remarks>
    /// Due to the heavy-weight nature of ResourceManager it is a good idea
    /// to cache instances whenever possible. This class uses a static member
    /// to cache all ResourceManager instances that have been retrieved using
    /// the assembly and base name.
    /// </remarks>
    ///
    internal static class ResourceRetriever
    {
        /// <summary>
        /// Maintains a cache of ResourceManager objects. This is a dictionary
        /// that is keyed based on the full name of the default resource assembly.
        /// The value is another dictionary that is keyed based on the base
        /// name for the resource that is being retrieved. The value for this
        /// dictionary is the ResourceManager.
        /// </summary>
        private static readonly Dictionary<string, Dictionary<string, ResourceManager>> ResourceRetrieverValue =
            new Dictionary<string, Dictionary<string, ResourceManager>>(
                StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Maintains a cache of string resources. This dictionary is keyed based
        /// on the <see cref="ResourceManager"/> instance from which the string
        /// resource was retrieved.
        /// </summary>
        ///
        /// <remarks>
        /// This cache is only used if GetResourceString or FormatResourceString is used to
        /// retrieve the resource. If the caller calls GetResourceManager the resources
        /// returned from the ResourceManager instance are not cached here though they
        /// may be cached by the ResourceManager under some circumstances. According to
        /// the BCL team they made a change to ResourceManager to cache resources less
        /// frequently.
        /// </remarks>
        ///
        private static readonly Dictionary<ResourceManager, Dictionary<string, Dictionary<string, string>>> StringResources =
            new Dictionary<ResourceManager, Dictionary<string, Dictionary<string, string>>>();

        /// <summary>
        /// Used to synchronize access to the ResourceRetriever
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// Gets the ResourceManager from the cache or gets an instance of
        /// the ResourceManager and returns it if it isn't already present
        /// in the cache.
        /// </summary>
        ///
        /// <param name="assembly">
        /// The assembly to be used as the base for resource lookup.
        /// </param>
        ///
        /// <param name="baseName">
        /// The base name of the resources to get the ResourceManager for.
        /// </param>
        ///
        /// <returns>
        /// A ResourceManager instance for the assembly and base name that
        /// were specified.
        /// </returns>
        ///
        internal static ResourceManager GetResourceManager(
            Assembly assembly,
            string baseName)
        {
            baseName = baseName ?? "resources";

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            // Check to see if the manager is already in the cache

            ResourceManager manager = null;
            Dictionary<string, ResourceManager> baseNameCache = null;

            lock (SyncRoot)
            {
                // First do the lookup based on the assembly location

                if (ResourceRetrieverValue.ContainsKey(assembly.FullName))
                {
                    baseNameCache =
                        ResourceRetrieverValue[assembly.FullName];

                    if (baseNameCache != null)
                    {
                        if (baseNameCache.ContainsKey(baseName))
                        {
                            // Now do the lookup based on the resource
                            // base name
                            manager = baseNameCache[baseName];
                        }
                    }
                }
            }

            // If it's not in the cache, create it an add it.
            if (manager == null)
            {
                manager = new ResourceManager(baseName, assembly);

                // Add the new resource manager to the hash

                if (baseNameCache != null)
                {
                    lock (SyncRoot)
                    {
                        // Since the assembly is already cached, we just have
                        // to cache the base name entry

                        baseNameCache[baseName] = manager;
                    }
                }
                else
                {
                    // Since the assembly wasn't cached, we have to create
                    // base name cache entry and then add it into the cache
                    // keyed by the assembly location

                    Dictionary<string, ResourceManager> baseNameCacheEntry =
                        new Dictionary<string, ResourceManager> { [baseName] = manager };

                    lock (SyncRoot)
                    {
                        ResourceRetrieverValue[assembly.FullName] =
                            baseNameCacheEntry;
                    }
                }
            }

            Debug.Assert(
                manager != null,
                "If the manager was not already created, it should have been" +
                "dynamically created or an exception should have been thrown");

            return manager;
        } // GetResourceManager

        /// <summary>
        /// Gets the string from the resource manager based on the base name,
        /// and resource ID specified
        /// </summary>
        ///
        /// <param name="resourceId">
        /// Resource ID for which the localized string needs to be retrieved
        /// </param>
        ///
        /// <returns>
        /// Localized string, or null if the string does not exist
        /// </returns>
        ///
        /// <remarks>
        /// The current thread's UI culture is used.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// ArgumentException if
        /// <paramref name="resourceId"/> is null or empty.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the value of the specified resource is not a string.
        /// </exception>
        ///
        /// <exception cref="MissingManifestResourceException">
        /// If no usable set of resources have been found, and there are no
        /// neutral culture resources.
        /// </exception>
        ///
        internal static string GetResourceString(
            string resourceId)
        {
            return
                GetResourceString(
                    typeof(ResourceRetriever).GetTypeInfo().Assembly,
                    "resources",
                    resourceId);
        }

        /// <summary>
        /// Gets the string from the resource manager based on the base name,
        /// and resource ID specified
        /// </summary>
        ///
        /// <param name="baseName">
        /// The base name of the resource to retrieve the string from.
        /// </param>
        ///
        /// <param name="resourceId">
        /// Resource ID for which the localized string needs to be retrieved
        /// </param>
        ///
        /// <returns>
        /// Localized string, or null if the string does not exist
        /// </returns>
        ///
        /// <remarks>
        /// The current thread's UI culture is used.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// ArgumentException if <paramref name="baseName"/> or
        /// <paramref name="resourceId"/> are null or empty.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the value of the specified resource is not a string.
        /// </exception>
        ///
        /// <exception cref="MissingManifestResourceException">
        /// If no usable set of resources have been found, and there are no
        /// neutral culture resources.
        /// </exception>
        ///
        internal static string GetResourceString(
            string baseName,
            string resourceId)
        {
            return
                GetResourceString(
                    typeof(ResourceRetriever).GetTypeInfo().Assembly,
                    baseName,
                    resourceId);
        }

        /// <summary>
        /// Gets the string from the resource manager based on the assembly,
        /// base name, resource ID, and culture specified
        /// </summary>
        ///
        /// <param name="assembly">
        /// The base assembly from which to get the resources from.
        /// </param>
        ///
        /// <param name="baseName">
        /// The base name of the resource to retrieve the string from.
        /// </param>
        ///
        /// <param name="resourceId">
        /// Resource ID for which the localized string needs to be retrieved
        /// </param>
        ///
        /// <returns>
        /// Localized String, or null if the string does not exist
        /// </returns>
        ///
        /// <remarks>
        /// The current thread's UI culture is used.
        /// </remarks>
        ///
        /// <exception cref="ArgumentException">
        /// ArgumentException if <paramref name="baseName"/> or
        /// <paramref name="resourceId"/> are null or empty.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the value of the specified resource is not a string.
        /// </exception>
        ///
        /// <exception cref="MissingManifestResourceException">
        /// If no usable set of resources have been found, and there are no
        /// neutral culture resources.
        /// </exception>
        ///
        internal static string GetResourceString(
            Assembly assembly,
            string baseName,
            string resourceId)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (string.IsNullOrEmpty(baseName))
            {
                throw new ArgumentException("baseName cannot be null", nameof(baseName));
            }

            if (string.IsNullOrEmpty(resourceId))
            {
                throw new ArgumentException("resourceId cannot be null", nameof(resourceId));
            }

            ResourceManager resourceManager =
                GetResourceManager(
                    assembly,
                    baseName);

            string text = null;
            string currentCulture = CultureInfo.CurrentUICulture.Name;

            if (StringResources.ContainsKey(resourceManager) &&
                StringResources[resourceManager].ContainsKey(currentCulture) &&
                StringResources[resourceManager][currentCulture].ContainsKey(resourceId))
            {
                text = StringResources[resourceManager][currentCulture][resourceId];
            }
            else
            {
                text = resourceManager.GetString(resourceId);

                if (!StringResources.ContainsKey(resourceManager))
                {
                    StringResources.Add(resourceManager, new Dictionary<string, Dictionary<string, string>>());
                }

                if (!StringResources[resourceManager].ContainsKey(currentCulture))
                {
                    StringResources[resourceManager].Add(currentCulture, new Dictionary<string, string>());
                }

                StringResources[resourceManager][currentCulture].Add(resourceId, text);
            }

            if (string.IsNullOrEmpty(text))
            {
                Debug.Assert(
                    false,
                    "Lookup failure: baseName " +
                    baseName +
                    " resourceId " +
                    resourceId);
            }

            return text;
        }

        /// <summary>
        /// Gets a template string from the resource manager using the
        /// calling assembly, and the current thread's UI culture, then
        /// inserts parameters using String.Format.
        /// </summary>
        ///
        /// <param name="resourceId">
        /// Resource ID for which the localized string needs to be retrieved
        /// </param>
        ///
        /// <param name="args">
        /// String.Format insertion parameters
        /// </param>
        ///
        /// <returns>
        /// Localized string, or null if the string does not exist
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// ArgumentException if <paramref name="resourceId"/>
        /// is null or empty.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the value of the specified resource is not a string.
        /// </exception>
        ///
        /// <exception cref="MissingManifestResourceException">
        /// If no usable set of resources have been found, and there are no
        /// neutral culture resources.
        /// </exception>
        ///
        /// <exception cref="FormatException">
        /// If <paramref name="args"/> could not be formatted into the
        /// resource string.
        /// </exception>
        ///
        internal static string FormatResourceString(
            string resourceId,
            params object[] args)
        {
            string baseName = "resources";

            if (string.IsNullOrEmpty(resourceId))
            {
                throw new ArgumentException("resourceId cannot be null", nameof(resourceId));
            }

            string template =
                GetResourceString(
                    typeof(ResourceRetriever).GetTypeInfo().Assembly,
                    baseName,
                    resourceId);

            string result = null;
            if (template != null)
            {
                result =
                    string.Format(
                        CultureInfo.CurrentCulture,
                        template,
                        args);
            }

            return result;
        }

        /// <summary>
        /// Gets the current representation of the space character.
        /// </summary>
        ///
        /// <param name="baseName">
        /// The base name of the resource to retrieve the string from.
        /// </param>
        ///
        /// <returns>
        /// Localized space, or null if the XSpace string cannot be found.
        /// </returns>
        ///
        /// <exception cref="ArgumentException">
        /// ArgumentException if <paramref name="baseName"/>
        /// is null or empty.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// If the value of the specified resource is not a string.
        /// </exception>
        ///
        /// <exception cref="MissingManifestResourceException">
        /// If no usable set of resources have been found, and there are no
        /// neutral culture resources.
        /// </exception>
        ///
        internal static string GetSpace(string baseName)
        {
            string xSpace = GetResourceString(
                                    typeof(ResourceRetriever).GetTypeInfo().Assembly,
                                    baseName,
                                    "XSpace");

            xSpace = xSpace?.Substring(1);

            return xSpace;
        }
    } // class ResourceRetriever
} // namespace Microsoft.Health.Utility
