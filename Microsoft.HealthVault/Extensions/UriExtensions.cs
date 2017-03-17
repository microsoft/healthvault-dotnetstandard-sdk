// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

namespace Microsoft.HealthVault.Extensions
{
    /// <summary>
    /// Extensions to help dealing with Uri objects.
    /// </summary>
    internal static class UriExtensions
    {
        public static IDictionary<string, string> ParseQueryWithCaseIgnorantKeys(this Uri uri)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return KeyValueStringIntoDictionary(dictionary, uri.Query.Length > 0 ? uri.Query.Substring(1) : string.Empty);
        }

        public static IDictionary<string, string> ParseQuery(this Uri uri)
        {
            return KeyValueStringToDictionary(uri.Query.Length > 0 ? uri.Query.Substring(1) : string.Empty);
        }

        public static IDictionary<string, string> ParseFragmentAsQuery(this Uri uri)
        {
            return KeyValueStringToDictionary(uri.Fragment.Substring(1));
        }

        private static IDictionary<string, string> KeyValueStringToDictionary(string keyValueString)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            return KeyValueStringIntoDictionary(dictionary, keyValueString);
        }

        private static IDictionary<string, string> KeyValueStringIntoDictionary(IDictionary<string, string> dictionary, string keyValueString)
        {
            int length = keyValueString.Length;

            for (int characterIndex = 0; characterIndex < length; characterIndex++)
            {
                int keyStartCharacterIndex = characterIndex;
                int equalsSeparatorCharacterIndex = -1;

                while (characterIndex < length)
                {
                    char character = keyValueString[characterIndex];

                    if (character == '=')
                    {
                        if (equalsSeparatorCharacterIndex < 0)
                        {
                            equalsSeparatorCharacterIndex = characterIndex;
                        }
                    }
                    else if (character == '&')
                    {
                        break;
                    }

                    characterIndex++;
                }

                string key = null;
                string value;

                if (equalsSeparatorCharacterIndex >= 0)
                {
                    key = keyValueString.Substring(keyStartCharacterIndex, equalsSeparatorCharacterIndex - keyStartCharacterIndex);
                    value = keyValueString.Substring(equalsSeparatorCharacterIndex + 1, (characterIndex - equalsSeparatorCharacterIndex) - 1);
                }
                else
                {
                    value = keyValueString.Substring(keyStartCharacterIndex, characterIndex - keyStartCharacterIndex);
                }

                dictionary.Add(Uri.UnescapeDataString(key), Uri.UnescapeDataString(value));

                if ((characterIndex == (length - 1)) && (keyValueString[characterIndex] == '&'))
                {
                    dictionary.Add(null, Uri.UnescapeDataString(value));
                }
            }

            return dictionary;
        }
    }
}