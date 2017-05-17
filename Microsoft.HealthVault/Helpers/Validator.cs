// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Microsoft.HealthVault.Exceptions;

namespace Microsoft.HealthVault.Helpers
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

        #region conditions

        public static void ThrowIfStringIsWhitespace(string value, string parameterName)
        {
            if (!string.IsNullOrEmpty(value) && string.IsNullOrEmpty(value.Trim()))
            {
                throw new ArgumentException(Resources.WhitespaceOnlyValue, parameterName);
            }
        }

        public static void ThrowIfStringIsEmptyOrWhitespace(string value, string parameterName)
        {
            if (value != null && string.IsNullOrEmpty(value.Trim()))
            {
                throw new ArgumentException(Resources.WhitespaceOnlyValue, parameterName);
            }
        }

        public static void ThrowIfStringNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(Resources.StringNullOrEmpty, parameterName);
            }
        }

        public static void ThrowIfArgumentNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ThrowIfArgumentNull(object argument, string argumentName, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName, message);
            }
        }

        public static void ThrowIfGuidEmpty(Guid argument, string argumentName)
        {
            if (argument == Guid.Empty)
            {
                throw new ArgumentException(Resources.GuidParameterEmpty, argumentName);
            }
        }

        public static void ThrowIfCollectionNullOrEmpty<T>(ICollection<T> argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argument.Count == 0)
            {
                throw new ArgumentException(Resources.CollectionEmpty, argumentName);
            }
        }

        public static void ThrowIfWriterNull(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer), Resources.WriteXmlNullWriter);
            }
        }

        public static void ThrowIfNavigatorNull(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                throw new ArgumentNullException(nameof(navigator), Resources.ParseXmlNavNull);
            }
        }

        #endregion

        #region exception and conditions

        public static void ThrowInvalidIfNull(object value, string message)
        {
            if (value == null)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void ThrowSerializationIfNull(object value, string message)
        {
            if (value == null)
            {
                throw new ThingSerializationException(message);
            }
        }

        #endregion
    }
}
