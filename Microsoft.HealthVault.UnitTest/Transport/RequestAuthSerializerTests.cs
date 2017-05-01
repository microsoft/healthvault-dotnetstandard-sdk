// Copyright (c) Microsoft Corporation.  All rights reserved. 
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Xml.XPath;
using Microsoft.HealthVault.Connection;
using Microsoft.HealthVault.Transport;
using Microsoft.HealthVault.Transport.Serializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest.Transport
{
    /// <summary>
    /// Contains Tests for auth section of the request
    /// </summary>
    [TestClass]
    public class RequestAuthSerializerTests
    {
        [TestMethod]
        public void WhenRequestAuthIsSerialized()
        {
            RequestAuth requestAuth = new RequestAuth { HmacData = new CryptoData() { Algorithm = "someAlg", Value = "someValue" } };

            RequestAuthSerializer requestAuthSerializer = new RequestAuthSerializer();
            string authXml = requestAuthSerializer.Serialize(requestAuth);

            XPathNavigator authNavigator = authXml
                .AsXPathNavigator()
                .SelectSingleNode("child::hmac-data");

            string algName = authNavigator
                .GetAttribute("algName", "");
            string hmacDataValue = authNavigator.Value;

            Assert.AreEqual("someAlg", algName);
            Assert.AreEqual("someValue", hmacDataValue);
        }
    }
}
