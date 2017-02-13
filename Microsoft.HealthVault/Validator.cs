// Copyright(c) Microsoft Corporation.
// This content is subject to the Microsoft Reference Source License,
// see http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.HealthVault
{
    /// <summary>
    /// Helps perform validation of parameters and throw appropriate exceptions. 
    /// </summary>
    /// 
    internal static class Validator
    {
        // Naming rules
        // 1) methods that return the exception should just be named for the exception.
        // 2) Methods that are checking for a condition are of the form:
        //      ThrowIf<condition>
        //      ThrowIfStringIsWhitespace(...), for example
        // 3) Methods that include the exception are of the format:
        //      Throw<exception>If<condition>
        //      ThrowSerializationIfNull(), for example
        //      It is acceptable to abbreviate the exception name as long as it is clear.

        #region Private Exception Helpers

        private static ArgumentException ArgumentException(Assembly assembly, string parameterName, string resourceId)
        {
            return new ArgumentException(
                    GetResourceString(assembly, resourceId),
                    parameterName);
        }

        private static HealthRecordItemSerializationException HealthRecordItemSerializationException(
            Assembly assembly,
            string resourceId)
        {
            return new HealthRecordItemSerializationException(
                    GetResourceString(assembly, resourceId));
        }

        private static InvalidOperationException InvalidOperationException(
            Assembly assembly,
            string resourceId)
        {
            return new InvalidOperationException(
                    GetResourceString(assembly, resourceId));
        }

        private static HealthServiceException HealthServiceException(
            Assembly assembly,
            string resourceId)
        {
            return new HealthServiceException(
                    GetResourceString(assembly, resourceId));
        }

        private static ArgumentNullException ArgumentNullException(
            Assembly assembly,
            string argumentName,
            string resourceId)
        {
            return new ArgumentNullException(
                    argumentName,
                    GetResourceString(assembly, resourceId));
        }

        #endregion

        #region Exceptions

        public static ArgumentException ArgumentException(string parameterName, string resourceId)
        {
            return ArgumentException(Assembly.GetCallingAssembly(), parameterName, resourceId);
        }

        public static HealthRecordItemSerializationException HealthRecordItemSerializationException(string resourceId)
        {
            return HealthRecordItemSerializationException(Assembly.GetCallingAssembly(), resourceId);
        }

        public static InvalidOperationException InvalidOperationException(string resourceId)
        {
            return InvalidOperationException(Assembly.GetCallingAssembly(), resourceId);
        }

        public static HealthServiceException HealthServiceException(string resourceId)
        {
            return HealthServiceException(Assembly.GetCallingAssembly(), resourceId);
        }

        public static InvalidConfigurationException InvalidConfigurationException(string resourceId)
        {
            return new InvalidConfigurationException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId));
        }

        public static SecurityException SecurityException(string resourceId)
        {
            return new SecurityException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId));
        }

        public static SecurityException SecurityException(string resourceId, Exception innerException)
        {
            return new SecurityException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId),
                        innerException);
        }

        public static NotSupportedException NotSupportedException(string resourceId)
        {
            return new NotSupportedException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId));
        }

        public static IOException IOException(string resourceId)
        {
            return new IOException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId));
        }

        public static WebException WebException(string resourceId, WebExceptionStatus webExceptionStatus)
        {
            return new WebException(
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId),
                        webExceptionStatus);
        }

        public static HealthRecordItemDeserializationException HealthRecordItemDeserializationException(
            string resourceId,
            Exception innerException)
        {
            return new HealthRecordItemDeserializationException(
                    GetResourceString(Assembly.GetCallingAssembly(), resourceId),
                    innerException);
        }

        #endregion

        #region conditions
        public static void ThrowIfStringIsWhitespace(string value, string parameterName)
        {
            if (!String.IsNullOrEmpty(value) && String.IsNullOrEmpty(value.Trim()))
            {
                throw ArgumentException(Assembly.GetCallingAssembly(), parameterName, "WhitespaceOnlyValue");
            }
        }

        public static void ThrowIfStringIsEmptyOrWhitespace(string value, string parameterName)
        {
            if (value != null && String.IsNullOrEmpty(value.Trim()))
            {
                throw ArgumentException(Assembly.GetCallingAssembly(), parameterName, "WhitespaceOnlyValue");
            }
        }

        public static void ThrowIfStringNullOrEmpty(string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw ArgumentException(Assembly.GetCallingAssembly(), parameterName, "StringNullOrEmpty");
            }
        }

        public static void ThrowIfArgumentNull(object argument, string argumentName, string resourceId)
        {
            if (argument == null)
            {
                throw ArgumentNullException(Assembly.GetCallingAssembly(), argumentName, resourceId);
            }
        }

        public static void ThrowIfWriterNull(XmlWriter writer)
        {
            if (writer == null)
            {
                throw ArgumentNullException(Assembly.GetCallingAssembly(), "writer", "WriteXmlNullWriter");
            }
        }

        public static void ThrowIfNavigatorNull(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                throw ArgumentNullException(Assembly.GetCallingAssembly(), "navigator", "ParseXmlNavNull");
            }
        }

        #endregion

        #region exception and conditions

        public static void ThrowArgumentExceptionIf(
            bool condition,
            string argumentName,
            string resourceId)
        {
            if (condition)
            {
                throw ArgumentException(Assembly.GetCallingAssembly(), argumentName, resourceId);
            }
        }

        public static void ThrowArgumentOutOfRangeIf(
            bool condition,
            string argumentName,
            string resourceId)
        {
            if (condition)
            {
                throw new ArgumentOutOfRangeException(
                    argumentName,
                    GetResourceString(
                        Assembly.GetCallingAssembly(),
                        resourceId));
            }
        }

        public static void ThrowInvalidIfNull(object value, string resourceId)
        {
            if (value == null)
            {
                throw InvalidOperationException(Assembly.GetCallingAssembly(), resourceId);
            }
        }

        public static void ThrowInvalidIf(bool condition, string resourceId)
        {
            if (condition)
            {
                throw InvalidOperationException(Assembly.GetCallingAssembly(), resourceId);
            }
        }

        public static void ThrowSerializationIfNull(object value, string resourceId)
        {
            if (value == null)
            {
                throw HealthRecordItemSerializationException(Assembly.GetCallingAssembly(), resourceId);
            }
        }

        public static void ThrowSerializationIf(bool condition, string resourceId)
        {
            if (condition)
            {
                throw HealthRecordItemSerializationException(Assembly.GetCallingAssembly(), resourceId);
            }
        }

        private static string GetResourceString(Assembly assembly, string resourceId)
        {
            return ResourceRetriever.GetResourceString(
                        assembly,
                        "resources",
                        resourceId);
        }

        #endregion

    }
}
