// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using Microsoft.HealthVault.Person;
using Newtonsoft.Json;
using Microsoft.HealthVault.Web.Extensions;

namespace Microsoft.HealthVault.Web.Cookie
{
    /// <summary>
    /// Provides Custom JsonConverter to serialize and deserialize WebConnectionInfo
    /// </summary>
    /// <remarks>
    /// Cookie in a browser is limited to 4093 bytes. 
    /// Webconnectioninfo will be minimized by this converter to make sure that serialized json
    /// fits in a cookie
    /// </remarks>
    internal class WebConnectionInfoConverter : JsonConverter
    {
        private const int MaxSize = 4093;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            WebConnectionInfo webConnectionInfo = value as WebConnectionInfo;

            if (webConnectionInfo != null)
            {
                string serializeObject = JsonConvert.SerializeObject(webConnectionInfo);

                if (serializeObject.Length > MaxSize)
                {
                    webConnectionInfo.MinimizePersonInfoApplicationSettings();
                    serializeObject = JsonConvert.SerializeObject(webConnectionInfo);
                }

                if (serializeObject.Length > MaxSize)
                {
                    webConnectionInfo.MinimizePersonInfoAuthorizedRecords();
                    serializeObject = JsonConvert.SerializeObject(webConnectionInfo);
                }

                writer.WriteValue(serializeObject);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string serializedPersonInfo = (string)reader.Value;

            return serializedPersonInfo == null 
                ? null 
                : JsonConvert.DeserializeObject(serializedPersonInfo);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(WebConnectionInfo);
        }
    }
}
